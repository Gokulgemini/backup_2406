using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace RDM.Messaging.ImageVault
{
    /// <summary>
    /// Indicates the result of the add image attempt.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum AddImageStatus
    {
        /// <summary>
        /// The image was successfully added to the vault.
        /// </summary>
        Success,

        /// <summary>
        /// The image could not be added to the vault.
        /// </summary>
        Failure
    }
}
