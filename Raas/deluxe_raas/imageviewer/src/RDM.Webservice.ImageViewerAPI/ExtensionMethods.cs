using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using RDM.Core;

namespace RDM.Webservice.ImageViewerAPI
{
    /// <summary>
    /// Convenience methods for the application.
    /// </summary>
    public static class ExtensionMethods
    {
        public static T GetSection<T>(this IConfiguration configuration, string section) where T : class
        {
            var result = Activator.CreateInstance<T>();
            new ConfigureFromConfigurationOptions<T>(configuration.GetSection(section)).Configure(result);

            return result;
        }

        /// <summary>
        /// Sets default serializer settings for Newtonsoft.Json.
        /// </summary>
        /// <param name="settings">Serializer settings to modify</param>
        public static void AddDefaultJsonSerializerSettings(this JsonSerializerSettings settings)
        {
            settings.ContractResolver = new ConcurrentJsonContractResolver();
            settings.NullValueHandling = NullValueHandling.Ignore;
            settings.DateFormatHandling = DateFormatHandling.IsoDateFormat;

            // Format DateTime and DateTimeOffset as ISO-8601
            settings.DateFormatString = "yyyy-MM-ddTHH:mm:ss.ffzzz";
            settings.TypeNameHandling = TypeNameHandling.None;

            // Make sure all enums are serialized as string
            settings.Converters.Add(new StringEnumConverter());
        }
    }
}
