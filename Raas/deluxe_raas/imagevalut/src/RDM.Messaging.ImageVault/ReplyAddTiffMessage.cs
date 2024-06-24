using System;
using Newtonsoft.Json;
using RDM.Core;
using RDM.Model.Itms;

namespace RDM.Messaging.ImageVault
{
    public class ReplyAddTiffMessage : IMessage, IEquatable<ReplyAddTiffMessage>
    {
        internal const string RabbitExchange = "imagevault.exchange";
        internal const string RabbitQueue = "imagevault.addtiff.queue";
        internal const string RabbitKey = "imagevault.addtiff.reply";

        /// <summary>
        /// Initializes a new instance of the <see cref="ReplyAddTiffMessage"/> class
        /// with the supplied information.
        /// </summary>
        /// <param name="status">The status of the attempt to store the image.</param>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="imageId"/> is <c>null</c> and <paramref name="status"/> is <c>Success</c>.
        /// </exception>
        public ReplyAddTiffMessage(AddImageStatus status)
            : this(status, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReplyAddTiffMessage"/> class
        /// with the supplied information.
        /// </summary>
        /// <param name="status">The status of the attempt to store the image.</param>
        /// <param name="imageId">The identifier of the image that was stored.</param>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="imageId"/> is <c>null</c> and <paramref name="status"/> is <c>Success</c>.
        /// </exception>
        [JsonConstructor]
        public ReplyAddTiffMessage(AddImageStatus status, ImageId imageId)
        {
            Contract.Requires<ArgumentException>(status != AddImageStatus.Success || (status == AddImageStatus.Success && imageId != null), "Status success but no image id.");

            Status = status;
            ImageId = imageId;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReplyAddTiffMessage"/> class
        /// suitable for use by an Activator.
        /// </summary>
        public ReplyAddTiffMessage()
        {
        }

        /// <summary>
        /// The status of the attempt to store the image.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public AddImageStatus Status { get; }

        /// <summary>
        /// The identifier of the image that was stored.
        /// </summary>
        public ImageId ImageId { get; }

        /// <inheritdoc/>
        public string DefaultQueueName
        {
            get
            {
                return RabbitQueue;
            }
        }

        /// <inheritdoc/>
        public string ExchangeName
        {
            get
            {
                return RabbitExchange;
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
        public bool Equals(ReplyAddTiffMessage other)
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
                Equals(ImageId, other.ImageId);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return Equals(obj as ReplyAddTiffMessage);
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
                result = (result * 31) + (ImageId?.GetHashCode() ?? 0);

                return result;
            }
        }
    }
}
