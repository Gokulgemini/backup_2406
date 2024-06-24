using System;
using Newtonsoft.Json;
using RDM.Core;
using RDM.Model.Itms;

namespace RDM.Messaging.ImageVault
{
    public class RequestGetImageMessage : IMessage, IEquatable<RequestGetImageMessage>
    {
        internal const string RabbitExchange = "imagevault.exchange";
        internal const string RabbitQueue = "imagevault.getimage.queue";
        internal const string RabbitKey = "imagevault.getimage.request";

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestGetImageMessage"/> class
        /// with the supplied information.
        /// </summary>
        /// <param name="requestId">The request Id to associate with the log.</param>
        /// <param name="imageId">The identifier of the image being requested.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="imageId"/> or <paramref name="requestId"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="requestId"/> is <c>RequestIdentifier.Empty</c>
        /// or when <paramref name="imageId"/> is <c>ImageId.Empty</c>.
        /// </exception>"
        public RequestGetImageMessage(RequestIdentifier requestId, ImageId imageId)
            : this(requestId, imageId, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestGetImageMessage"/> class
        /// with the supplied information.
        /// </summary>
        /// <param name="requestId">The request Id to associate with the log.</param>
        /// <param name="imageId">The identifier of the image being requested.</param>
        /// <param name="width">The desired width of the returned image.  If not present, the original width is preserved.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="requestId"/> or <paramref name="imageId"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="requestId"/> is <c>RequestIdentifier.Empty</c>,
        /// or when <paramref name="imageId"/> is <c>ImageId.Empty</c>,
        /// when the width is supplied and it is not greater than zero.
        /// </exception>"
        [JsonConstructor]
        public RequestGetImageMessage(RequestIdentifier requestId, ImageId imageId, int? width)
        {
            Contract.Requires<ArgumentNullException>(requestId != null, nameof(requestId));
            Contract.Requires<ArgumentException>(requestId != RequestIdentifier.Empty, nameof(requestId));
            Contract.Requires<ArgumentNullException>(imageId != null, nameof(imageId));
            Contract.Requires<ArgumentException>(imageId != ImageId.Empty, nameof(imageId));
            Contract.Requires<ArgumentException>(!width.HasValue || width.Value > 0, nameof(width));

            RequestId = requestId;
            ImageId = imageId;
            Width = width;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestGetImageMessage"/> class
        /// suitable for use by an Activator.
        /// </summary>
        public RequestGetImageMessage()
        {
        }

        /// <summary>
        /// The request Id to associate with the log.
        /// </summary>
        public RequestIdentifier RequestId { get; }

        /// <summary>
        /// The identifier of the image being requested.
        /// </summary>
        public ImageId ImageId { get; }

        /// <summary>
        /// The desired width of the returned image.  If not present, the original width is preserved.
        /// </summary>
        public int? Width { get; }

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
        public bool Equals(RequestGetImageMessage other)
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
                RequestId.Equals(other.RequestId) &&
                ImageId.Equals(other.ImageId) &&
                Equals(Width, other.Width);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return Equals(obj as RequestGetImageMessage);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                var result = RabbitQueue.GetHashCode();
                result = (result * 31) + RabbitExchange.GetHashCode();
                result = (result * 31) + RabbitKey.GetHashCode();
                result = (result * 31) + RequestId.GetHashCode();
                result = (result * 31) + ImageId.GetHashCode();
                result = (result * 31) + (Width?.GetHashCode() ?? 0);

                return result;
            }
        }
    }
}
