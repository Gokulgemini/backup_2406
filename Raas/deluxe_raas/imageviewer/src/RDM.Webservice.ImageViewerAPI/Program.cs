using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RDM.Webservice.ImageViewerAPI
{
    public class Program
    {
        private const string SettingsFile = "appsettings.json";

        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        private static IWebHost BuildWebHost(string[] args)
        {
            var options = ServiceOptions.Load(SettingsFile);

            return new WebHostBuilder()
                .UseKestrel(options => options.AllowSynchronousIO = true)
                .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureAppConfiguration((hostingContext, configuration) =>
                {
                    var env = hostingContext.HostingEnvironment;

                    // Setup the configuration (file paths, environment variables)
                    configuration.SetBasePath(env.ContentRootPath)
                        .AddJsonStream(GenerateStreamFromString(options))
                        .AddEnvironmentVariables();
                })
                .ConfigureLogging((hostingContext, logging) =>
                {
                    var env = hostingContext.HostingEnvironment;

                    // Configure logging
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));

                    if (env.IsDevelopment())
                    {
                        logging.AddConsole();
                        logging.AddDebug();
                    }
                })
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();
        }

        private static Stream GenerateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}
