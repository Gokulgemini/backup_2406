using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Newtonsoft.Json.Linq;
using RDM.Core.Configuration;

namespace RDM.Webservice.ImageViewerAPI
{
    /// <summary>
    /// The configured settings for the service.
    /// </summary>
    internal sealed class ServiceOptions
    {
        private static readonly List<ProviderHelper> HelperList = new List<ProviderHelper>();

        /// <summary>
        /// Provides a helper method to retrieve the configuration from a json file.
        /// </summary>
        /// <param name="filename">A JSON file containing the settings to load.</param>
        /// <returns>
        /// Returns the configured settings from the specified file if found,
        /// otherwise returns <c>null</c>.
        /// </returns>
        public static string Load(string filename)
        {
            // read template json
            var json = LoadFile(filename);
            if (string.IsNullOrWhiteSpace(json))
            {
                return null;
            }

            // load template into memory
            var templateJson = JObject.Parse(json);

            // parse for provider to use to replace
            ProviderHelper.RecursiveParse(HelperList, templateJson);

            // Setup Providers
            var configurationProviders = Factory.GetConfigurationProviders(filename);

            // find and update template
            foreach (var providerHelper in HelperList)
            {
                try
                {
                    var value = configurationProviders[providerHelper.Provider]
                        .GetString(providerHelper.ProviderSearchPath);
                    templateJson.SelectToken(providerHelper.Path).Replace(value);
                }
                catch (KeyNotFoundException)
                {
                    //This is fine, it means the value is not coming from a provider but is hard coded
                }
            }

            return templateJson.ToString();
        }

        private static string LoadFile(string filename)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var configFolder = AppDomain.CurrentDomain.BaseDirectory;
            if (!File.Exists(Path.Combine(configFolder, filename)))
            {
                configFolder = Environment.CurrentDirectory;
                if (!File.Exists(Path.Combine(configFolder, filename)))
                {
                    configFolder = Path.Combine(configFolder, "src");
                    configFolder = Path.Combine(configFolder, typeof(ServiceOptions).GetTypeInfo().Assembly.GetName().Name);
                    if (!File.Exists(Path.Combine(configFolder, filename)))
                    {
                        configFolder = @".\";
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(environment))
            {
                var newFilename = Path.GetFileNameWithoutExtension(filename) + "." + environment +
                                  Path.GetExtension(filename);

                if (File.Exists(Path.Combine(configFolder, newFilename)))
                {
                    filename = newFilename;
                }
            }

            var targetFile = Path.Combine(configFolder, filename);
            if (!File.Exists(targetFile))
            {
                return null;
            }

            using (var r = new StreamReader(targetFile))
            {
                var json = r.ReadToEnd();

                return json;
            }
        }
    }
}
