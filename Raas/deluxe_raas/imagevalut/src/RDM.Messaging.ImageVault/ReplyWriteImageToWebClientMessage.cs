using System;
using Newtonsoft.Json;
using RDM.Core;
using RDM.Model.ImageVault;

namespace RDM.Messaging.ImageVault
{
    public class ReplyWriteImageToWebClientMessage : IMessage, IEquatable<ReplyWriteImageToWebClientMessage>
    {
        internal const string RabbitExchange = "imagevault.exchange";
        internal const string RabbitQueue = "imagevault.writetifftoshare.queue";
        internal const string RabbitKey = "imagevault.writetifftoshare.reply";

        /// <summary>
        /// Initializes a new instance of the <see cref="ReplyWriteImageToWebClientMessage"/> class
        /// with the supplied information.
        /// </summary>
        /// <param name="status">The status of the request to retrieve the image.</param>
        /// <param name="imageTiffInfo">The imageId of the image to be saved to Tiff file.</param>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="imageTiffInfo"/> is <c>null</c> and <paramref name="status"/> is <c>Success</c>.
        /// </exception>
        [JsonConstructor]
        public ReplyWriteImageToWebClientMessage(WriteImageToWebClientStatus status, ImageTiffInfo imageTiffInfo)
        {
            Contract.Requires<ArgumentException>(status != WriteImageToWebClientStatus.Success || (status == WriteImageToWebClientStatus.Success && imageTiffInfo != null), "Status Success but no TiffImageInfo.");

            Status = status;
            ImageTiffInfo = imageTiffInfo;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReplyWriteImageToWebClientMessage"/> class
        /// suitable for use by an Activator.
        /// </summary>
        public ReplyWriteImageToWebClientMessage()
        {
        }

        /// <summary>
        /// The status of the request to write the tiff to the share drive.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public WriteImageToWebClientStatus Status { get; }

        /// <summary>
        /// Info on the image, if found.
        /// </summary>
        public ImageTiffInfo ImageTiffInfo { get; }

        /// <inheritdoc/>
        public string ExchangeName
        {
            get
            {
                return RabbitExchange;
            }
        }

        /// <inheritdoc/>
        public string DefaultQueueName
        {
            get
            {
                return RabbitQueue;
            }
        }

        /// <inheritdoc/>
        public string RoutingKey
        {
            get
            {
                return RabbitKey;
            }
        }

        /// <inheritdoc/>
        public bool Equals(ReplyWriteImageToWebClientMessage other)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }

            if (ReferenceEquals(other, this))
            {
                return true;
            }

            return
                Status == other.Status &&
                Equals(ImageTiffInfo, other.ImageTiffInfo);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return Equals(obj as ReplyWriteImageToWebClientMessage);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                var result = RabbitQueue.GetHashCode();
                result = (result * 31) + RabbitExchange.GetHashCode();
                result = (result * 31) + RabbitKey.GetHashCode();
                result = (result * 31) + Status.GetHashCode();
                result = (result * 31) + (ImageTiffInfo?.GetHashCode() ?? 0);

                return result;
            }
        }
    }
}
