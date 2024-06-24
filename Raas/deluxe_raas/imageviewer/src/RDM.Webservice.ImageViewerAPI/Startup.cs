using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RDM.Client.ImageVault;
using RDM.Client.Tracker;
using RDM.DataTransferObjects.ImageViewerAPI;
using RDM.Maps.ImageViewerAPI;
using RDM.Messaging;
using RDM.Messaging.RabbitMQ;
using RDM.Middleware;
using RDM.Middleware.ItmsApi;
using RDM.Models.ImageViewerAPI;
using RDM.Services.ImageViewerAPI;
using RDM.Statistician;
using RDM.Statistician.PerformanceLog;
using RDM.Webservice.ImageViewerAPI.Factories;
using RDM.Webservice.ImageViewerAPI.Options;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Sinks.Graylog;
using Enyim.Caching;
using RDM.Model.Itms;
using Microsoft.Extensions.Logging;
using RDM.Data.ImageViewer;
using RDM.Data.ImageViewer.SqlServer;

namespace RDM.Webservice.ImageViewerAPI
{
    public class Startup
    {
        public const string ApiRoot = "itms/";
        public const string ApplicationName = "ImageViewer";
        public const string ForwardedProtoHeader = "X-Forwarded-Proto";
        public const string HttpContextPathBase = "/imageviewer";

        private readonly LoggingLevelSwitch _levelSwitch;
        private readonly IMessageQueue _rabbitQueue;
        private readonly GrpcOptions _grpcOptions;
        private readonly string _corsPolicy = "CorsPolicy";

        /// <summary>
        /// Initializes a new instance of the <see cref="Startup" /> class.
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="env">A hosting environment instance.</param>
        /// <param name="configuration">A configuration instance.</param>
        public Startup(IWebHostEnvironment env, IConfiguration configuration)
        {
            Configuration = configuration;

            if (!Enum.TryParse(
                Configuration.GetSection("Logging").GetSection("LogLevel").GetSection("Default").Value,
                out LogEventLevel minimumLogLevel))
            {
                // If it fails to get a level go with warning
                minimumLogLevel = LogEventLevel.Warning;
            }

            _levelSwitch = new LoggingLevelSwitch { MinimumLevel = minimumLogLevel };

            var queueOptions = Configuration.GetSection<MessageQueueOptions>("MessageQueueOptions");
            _rabbitQueue = new RabbitQueue(queueOptions.ToDictionary());

            _grpcOptions = Configuration.GetSection<GrpcOptions>("GrpcOptions");

            var graylogOptions = Configuration.GetSection<GraylogOptions>("GraylogSinkOptions");

            // Use the real logger when not running in a dev environment
            if (!env.IsDevelopment())
            {
                Log.Logger = new LoggerConfiguration()
                    .Enrich.FromLogContext()
                    .WriteTo.Graylog(new GraylogSinkOptions
                    {
                        HostnameOrAddress = graylogOptions.HostnameOrAddress,
                        Port = graylogOptions.Port,
                        TransportType = graylogOptions.TransportType,
                        Host = ApplicationName
                    })
                    .MinimumLevel.ControlledBy(_levelSwitch)
                    .CreateLogger();
            }

            Log.Logger.Information("Service Started");
        }

        public IConfiguration Configuration { get; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services">A service collection instance.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
                options.Providers.Add<GzipCompressionProvider>();
            });

            services.AddCors(options =>
            {
                options.AddPolicy(
                    _corsPolicy,
                    builder => builder.SetIsOriginAllowedToAllowWildcardSubdomains()
                                      .AllowAnyOrigin()
                                      .AllowAnyMethod()
                                      .AllowAnyHeader()
                                      .WithExposedHeaders("SUToken"));
            });

            services.AddControllers(options => options.RespectBrowserAcceptHeader = true)
                    .ConfigureApiBehaviorOptions(options =>
                    {
                        options.SuppressMapClientErrors = true;
                        options.SuppressModelStateInvalidFilter = true;
                    })
                    .AddNewtonsoftJson(jsonOptions => jsonOptions.SerializerSettings.AddDefaultJsonSerializerSettings());

            services.AddOptions();
            services.Configure<MockMiddlewareOptions>(Configuration.GetSection("MockOptions"));
            services.Configure<AuthorizationMiddlewareOptions>(Configuration.GetSection("AuthorizationOptions"));
            services.Configure<RepositoryOptions>(Configuration.GetSection("RepositoryOptions"));
            services.Configure<MessageQueueOptions>(Configuration.GetSection("MessageQueueOptions"));

            // Register application services.
            services.AddSingleton<ITrackerClient, TrackerClient>();
            services.AddSingleton<IImageVaultClient>(i => new ImageVaultClient(_rabbitQueue, _grpcOptions.Host, _grpcOptions.Deadline));
            services.AddSingleton<IMonitorFactory, MonitorFactory>();
            services.AddSingleton<IRequestDataFactory, RequestDataFactory>();

            services.AddScoped<IMessageQueueFactory, RabbitMessageQueueFactory>();
            services.AddScoped<IItmsItemServiceFactory, ItmsItemServiceFactory>();
            services.AddScoped<IWebClientItemServiceFactory, WebClientItemServiceFactory>();

            services.AddSingleton<IMapper<Cheque, ChequeDto>, ChequeMapper>();
            services.AddSingleton<IMapper<Remittance, RemittanceDto>, RemittanceMapper>();
            services.AddSingleton<IMapper<GeneralDocument, GeneralDocumentDto>, GeneralDocumentMapper>();
            services.AddSingleton<IRequestDataAccessor, ServiceRequestDataAccessor>(
                 provider =>
                 {
                     var requestDataFactory = provider.GetRequiredService<IRequestDataFactory>();
                     var monitorFactory = provider.GetRequiredService<IMonitorFactory>();
                     var requestDataAccessor = new ServiceRequestDataAccessor(requestDataFactory.Create().RequestId)
                     {
                         PerformanceMonitor = monitorFactory.Get()
                     };
                     return requestDataAccessor;
                 });
            services.AddSingleton<IRolePermissionsRepository>(
                provider => new RolePermissionsRepository(
                    provider.GetRequiredService<IRequestDataAccessor>(),
                    Configuration.GetSection("PermissionsRepository").Get<Dictionary<string, string>>(),
                    provider.GetRequiredService<ILogger<RolePermissionsRepository>>()));
            services.AddSingleton<IPermissionService, PermissionsService>();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // Logger and Operations Service
            services.AddSingleton(Log.Logger);
            services.AddSingleton(_levelSwitch);
            services.AddSingleton(_rabbitQueue);
            services.AddSingleton<OperationsService>();

            // Initialize the performance factory
            Factory.Initialize(Log.Logger, Configuration.GetSection<Dictionary<string, string>>("Performance"), ApplicationName);

            services.AddOpenTracing();
            services.AddSingleton(Factory.PerformanceLogger);
            services.AddSingleton(Factory.TraceLogger.Tracer);

            // Memcached
            services.AddEnyimMemcached(Configuration.GetSection("enyimMemcached"));

            // Add the Serilog logger
            services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog());
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app">An application builder instance.</param>
        /// <param name="env">A hosting environment instance.</param>
        /// <param name="appLifetime">An application lifetime instance.</param>
        /// <param name="monitorFactory">A monitor factory instance.</param>
        /// <param name="performanceLogger">A performance monitor instance.</param>
        public void Configure(
            IApplicationBuilder app,
            IWebHostEnvironment env,
            IHostApplicationLifetime appLifetime,
            IMonitorFactory monitorFactory,
            IPerformanceLogger performanceLogger)
        {
            app.UseOperationsService();

            appLifetime.ApplicationStopped.Register(Log.CloseAndFlush);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.Use(async (context, next) =>
                {
                    // Due to our switch to nginx we must do some things manually that iis did automatically.
                    // We lose the original scheme after going through the proxy and need to carry it over
                    // through the X-Forwarded-Proto header. The scheme we're trying to work with here is HTTPS.
                    if (context.Request.Headers.ContainsKey(ForwardedProtoHeader))
                    {
                        // Multiple protocols could be appended to the header and must be split into an array.
                        var protocols = context.Request.Headers[ForwardedProtoHeader]
                                               .ToString()
                                               .Split(',')
                                               .Select(p => p.Trim().ToLower());

                        // If all of the previously appended protocols match the desired HTTPS scheme then
                        // we can safely set it manually here. Otherwise it will remain HTTP.
                        if (protocols.All(p => string.CompareOrdinal(p, "https") == 0))
                        {
                            context.Request.Scheme = "https";
                        }
                    }

                    // The first bit of the path is also ripped out. Add it back as the path base in the context.
                    context.Request.PathBase = HttpContextPathBase;

                    var monitor = monitorFactory.Get();
                    monitor.Start("TOTAL_TIME");
                    await next.Invoke();
                    monitor.Stop("TOTAL_TIME");
                    performanceLogger.Log(monitor, context.Request.Path, context.TraceIdentifier);
                });

                app.UseExceptionHandler(new ExceptionHandlerOptions
                {
                    ExceptionHandler = async context =>
                    {
                        await new ExceptionHandler().Invoke(context, monitorFactory);
                    }
                });
            }

            app.UseCors(_corsPolicy);

            app.UseMiddleware<MockApiMiddleware>();
            app.UseMiddleware<AuthorizationMiddleware>();

            app.UseResponseCompression();
            app.UseEnyimMemcached();
            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
