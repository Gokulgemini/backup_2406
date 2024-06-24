using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace RDM.Messaging.ImageVault
{
    /// <summary>
    /// Indicates the result of writing the requested image to WebClient.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum WriteImageToWebClientStatus
    {
        /// <summary>
        /// The image was successfully saved to file system.
        /// </summary>
        Success,

        /// <summary>
        /// Unable to save the image to the share drive.
        /// </summary>
        Failure
    }
}
