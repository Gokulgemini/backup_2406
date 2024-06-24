using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace RDM.Messaging.ImageVault
{
    /// <summary>
    /// Indicates the status of the attempt to retrieve an image from the vault.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum GetImageStatus
    {
        /// <summary>
        /// The image was successfully retrieved.
        /// </summary>
        Success,

        /// <summary>
        /// The requested image could not be found.
        /// </summary>
        NotFound,

        /// <summary>
        /// The requested image could not be retrieved.
        /// </summary>
        Failure
    }
}
