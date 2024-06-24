using System;
using Newtonsoft.Json;
using RDM.Core;
using RDM.Model.Itms;

namespace RDM.Messaging.ImageVault
{
    public class ReplyGetImageByIrnMessage : IMessage, IEquatable<ReplyGetImageByIrnMessage>
    {
        internal const string RabbitExchange = "imagevault.exchange";
        internal const string RabbitQueue = "imagevault.getimagebyirn.queue";
        internal const string RabbitKey = "imagevault.getimagebyirn.reply";

        /// <summary>
        /// Initializes a new instance of the <see cref="ReplyGetImageByIrnMessage"/> class
        /// with the supplied information.
        /// </summary>
        /// <param name="status">The status of the request to retrieve the image.</param>
        /// <param name="image">The image retrieved, if found.</param>
        /// <exception cref="ArgumentException">
        /// Thrown when when <paramref name="image"/> is <c>null</c> and <paramref name="status"/> is <c>Success</c>.
        /// </exception>
        [JsonConstructor]
        public ReplyGetImageByIrnMessage(GetImageStatus status, Image image)
        {
            Contract.Requires<ArgumentException>(
                status != GetImageStatus.Success || (status == GetImageStatus.Success && image != null),
                "Success without image.");

            Status = status;
            Image = image;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReplyGetImageByIrnMessage"/> class
        /// suitable for use by an Activator.
        /// </summary>
        public ReplyGetImageByIrnMessage()
        {
        }

        /// <summary>
        /// The status of the request to retrieve the image.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public GetImageStatus Status { get; }

        /// <summary>
        /// The image retrieved, if found.
        /// </summary>
        public Image Image { get; }

        /// <inheritdoc/>
        public string ExchangeName => RabbitExchange;

        /// <inheritdoc/>
        public string DefaultQueueName => RabbitQueue;

        /// <inheritdoc/>
        public string RoutingKey => RabbitKey;

        /// <inheritdoc/>
        public bool Equals(ReplyGetImageByIrnMessage other)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }

            if (ReferenceEquals(other, this))
            {
                return true;
            }

            return Status == other.Status && Equals(Image, other.Image);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return Equals(obj as ReplyGetImageByIrnMessage);
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
                result = (result * 31) + (Image?.GetHashCode() ?? 0);

                return result;
            }
        }
    }
}
