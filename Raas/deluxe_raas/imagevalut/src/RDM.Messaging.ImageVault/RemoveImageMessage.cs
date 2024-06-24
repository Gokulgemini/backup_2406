using System;
using Newtonsoft.Json;
using RDM.Core;
using RDM.Model.Itms;

namespace RDM.Messaging.ImageVault
{
    /// <summary>
    /// Provides a structured payload for requesting the removal of an image from the vault.
    /// </summary>
    public class RemoveImageMessage : IMessage, IEquatable<RemoveImageMessage>
    {
        internal const string RabbitExchange = "imagevault.exchange";
        internal const string RabbitQueue = "imagevault.removeimage.queue";
        internal const string RabbitKey = "imagevault.removeimage";

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoveImageMessage"/> class
        /// with the supplied options.
        /// </summary>
        /// <param name="requestId">The request Id to associate with the log.</param>
        /// <param name="imageId">The identifier of the image to remove.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="requestId"/> or <paramref name="imageId"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="requestId"/> is <c>RequestIdentifier.Empty</c>
        /// or when <paramref name="imageId"/> is <c>ImageId.Empty</c>.
        /// </exception>
        [JsonConstructor]
        public RemoveImageMessage(RequestIdentifier requestId, ImageId imageId)
        {
            Contract.Requires<ArgumentNullException>(requestId != null, nameof(requestId));
            Contract.Requires<ArgumentException>(requestId != RequestIdentifier.Empty, nameof(requestId));
            Contract.Requires<ArgumentNullException>(imageId != null, nameof(imageId));
            Contract.Requires<ArgumentException>(imageId != ImageId.Empty, nameof(imageId));

            RequestId = requestId;
            ImageId = imageId;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoveImageMessage"/> class
        /// suitable for use by an Activator.
        /// </summary>
        public RemoveImageMessage()
        {
        }

        /// <summary>
        /// The request Id to associate with the log.
        /// </summary>
        public RequestIdentifier RequestId { get; }

        /// <summary>
        /// The identifier of the image to remove.
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
        public bool Equals(RemoveImageMessage other)
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
                Equals(RequestId, other.RequestId) &&
                Equals(ImageId, other.ImageId);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return Equals(obj as RemoveImageMessage);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                var result = RabbitExchange.GetHashCode();
                result = (result * 31) + RabbitQueue.GetHashCode();
                result = (result * 31) + RabbitKey.GetHashCode();
                result = (result * 31) + RequestId.GetHashCode();
                result = (result * 31) + ImageId.GetHashCode();

                return result;
            }
        }
    }
}
