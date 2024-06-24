using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTracing.Contrib.Grpc.Interceptors;
using RDM.Data.ImageVault;
using RDM.Data.ImageVault.Legacy;
using RDM.Data.ImageVault.SqlServer;
using RDM.Data.ImageVault.SqlServer.Legacy;
using RDM.Extensions.Configuration.Vault;
using RDM.Imaging;
using RDM.Messaging;
using RDM.Messaging.RabbitMQ;
using RDM.Model.Itms;
using RDM.Services.ImageVault;
using RDM.Statistician;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Sinks.Graylog;
using Serilog.Sinks.Graylog.Core.Transport;

namespace RDM.Service.ImageVault
{
    /// <summary>
    /// The ImageVault Windows service implementation
    /// </summary>
    public class Program
    {
        public static readonly string ServiceLoggingName = "ImageVault";
        private static LoggingLevelSwitch levelSwitch;

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            // Initialize the configuration provider
            builder.Configuration.AddVaultFromConfiguration(builder.Configuration);

            IntializeLogging(builder);
            ConfigureComponents(builder);
            RegisterServices(builder);
            InitializeRepository(builder);

            var app = builder.Build();
            app.MapGrpcService<GrpcService>();
            app.Run();
        }

        private static void IntializeLogging(WebApplicationBuilder builder)
        {
            // Route Microsoft logging to Serilog.
            builder.Logging.ClearProviders();
            builder.Logging.AddSerilog();

            // Determine how to log
            levelSwitch = new LoggingLevelSwitch
            {
                MinimumLevel = (LogEventLevel)Enum.Parse(typeof(LogEventLevel),
                    builder.Configuration.GetValue<string>("Logging:LogLevel:Default", "Debug"))
            };

            if (builder.Environment.IsDevelopment())
            {
                Log.Logger = new LoggerConfiguration()
                    .Enrich.FromLogContext()
                    .WriteTo.Console()
                    .MinimumLevel.ControlledBy(levelSwitch)
                    .CreateLogger();
            }
            else
            {
                var port = builder.Configuration.GetValue<int>("GraylogSinkOptions:Port", 0);
                if (port == 0)
                {
                    throw new ArgumentNullException("GraylogSinkOptions Port.");
                }

                var transport = builder.Configuration.GetValue<string>("GraylogSinkOptions:TransportType", null);
                if (transport == null || !Enum.TryParse(transport, out TransportType transportType))
                {
                    throw new ArgumentNullException("GraylogSinkOptions TransportType.");
                }

                var host = builder.Configuration.GetValue<string>("GraylogSinkOptions:HostnameOrAddress", null);
                if (host == null)
                {
                    throw new ArgumentNullException("GraylogSinkOptions HostnameOrAddress.");
                }

                Log.Logger = new LoggerConfiguration()
                    .Enrich.FromLogContext()
                    .WriteTo.Graylog(new GraylogSinkOptions
                    {
                        HostnameOrAddress = host,
                        Port = port,
                        TransportType = transportType,
                        Host = ServiceLoggingName
                    })
                    .MinimumLevel.ControlledBy(levelSwitch)
                    .CreateLogger();
            }
        }

        private static void ConfigureComponents(WebApplicationBuilder builder)
        {
            // Initialize the performance factory
            var performance = builder.Configuration.GetSection("Performance").Get<Dictionary<string, string>>();

            // Initialize Statistician
            Statistician.Factory.Initialize(Log.Logger, performance, ServiceLoggingName);
        }

        private static void RegisterServices(WebApplicationBuilder builder)
        {
            var services = builder.Services;

            // Get worker count
            var workerCount = builder.Configuration.GetValue<int>("Execution:WorkerCount", 1);

            // Telemetry registrations
            services.AddOpenTelemetryTracing(
                options =>
                {
                    // Here we define header info for trace messages and we also specify what
                    // tracing info that we'd like to have automatically generated ("instrumented").
                    options
                        .AddSource(ServiceLoggingName)
                        .SetResourceBuilder(
                            ResourceBuilder.CreateDefault()
                                .AddService(serviceName: ServiceLoggingName,
                                    serviceVersion: Assembly.GetExecutingAssembly()
                                        .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                                        ?.InformationalVersion))
                        .AddSqlClientInstrumentation()
                        .AddAspNetCoreInstrumentation();

                    // Here we specify a destination for tracing info (ex. to the Console for development, otherwise to Jaeger or some other display tool (to be added later)).
                    if (builder.Environment.IsDevelopment())
                        options.AddConsoleExporter(); // for Dev testing
                });

            //DI registrations;
            services.AddSingleton<IRequestDataAccessor, RequestDataAccessor>();
            services.AddSingleton(Log.Logger);
            services.AddSingleton(levelSwitch);
            services.AddSingleton<IFailureMode>(provider =>
                 new FailureMode((Mode)Enum.Parse(typeof(Mode),
                 builder.Configuration.GetValue<string>("Execution:FailureMode", "Stop"))));
            services.AddSingleton<IImageFactory, RdmImageFactory>();

            services.AddTransient<IFileSystem, FileSystem>();
            services.AddTransient<IDateTime, DateTimeWrapper>();
            services.AddTransient<IBinaryFileReaderWriter, BinaryFileReaderWriter>();

            services.AddSingleton<ILegacyImageAccess, LegacyImageAccess>();

            // The original code made one instance of this service per worker. We'll register
            // it as transient to maintain similar behaviour.
            services.AddSingleton<IImageRepository>(
                provider => new ImageRepository(
                    provider.GetRequiredService<IRequestDataAccessor>(),
                    builder.Configuration.GetSection("ServiceDatabase").Get<Dictionary<string, string>>(),
                    provider.GetRequiredService<Serilog.ILogger>()
                    ));

            services.AddTransient<IItmsImageRepository>(provider =>
                    new ItmsImageRepository(builder.Configuration.GetSection("ItmsImageRepository").Get<Dictionary<string, string>>(),
                    provider.GetRequiredService<IRequestDataAccessor>())
                );

            services.AddTransient<IDictionary<string, IWebClientImageRepository>>(provider =>
            {
                var webClientImageRepositories = new Dictionary<string, IWebClientImageRepository>();
                foreach (var repository in builder.Configuration.GetSection("WebClientImageRepositories").Get<Dictionary<string, Dictionary<string, string>>>())
                {
                    webClientImageRepositories.Add(
                        repository.Key,
                        new WebClientImageRepository(repository.Value, provider.GetRequiredService<IRequestDataAccessor>()));
                }

                return webClientImageRepositories;
            });

            services.AddSingleton(provider => Factory.PerformanceLogger);
            services.AddTransient<IMessageQueue>(provider => new RabbitQueue(
                builder.Configuration.GetSection("Rabbit").Get<Dictionary<string, string>>()));

            // Register Hosted Services
            for (var i = 0; i < workerCount; i++)
            {
                // Registering multiple instances of the same class is not working when using the AddHostedService extension
                services.AddSingleton<IHostedService, ImageVaultService>();
            }

            // Add services.AddSingleton<GrpcService>() to make sure that one instance is registered
            // to avoid creating a new instance of the service, every time you request it (avoid transient dependency)
            services.AddSingleton<GrpcService>();
            services
                .AddGrpc()
                .AddServiceOptions<GrpcService>(options =>
                {
                    options.MaxReceiveMessageSize = 30 * 1024 * 1024; // 30 MB
                    if (Statistician.Factory.TraceLogger != null)
                    {
                        // Make sure we can receive traces if we have a valid trace provider
                        options.Interceptors.Add<ServerTracingInterceptor>(
                            Statistician.Factory.TraceLogger.Tracer);
                    }
                });
        }

        private static void InitializeRepository(WebApplicationBuilder builder)
        {
            var initializer = new ImageVaultRepositoryInitializer(builder.Configuration.GetSection("ServiceDatabase").Get<Dictionary<string, string>>());
            initializer.Initialize();
        }
    }
}