using System;
using Newtonsoft.Json;
using RDM.Core;
using RDM.Model.Itms;

namespace RDM.Messaging.ImageVault
{
    public class RequestWriteImageToWebClientMessage : IMessage, IEquatable<RequestWriteImageToWebClientMessage>
    {
        internal const string RabbitExchange = "imagevault.exchange";
        internal const string RabbitQueue = "imagevault.writetifftoshare.queue";
        internal const string RabbitKey = "imagevault.writetifftoshare.request";

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestWriteImageToWebClientMessage"/> class
        /// with the supplied information.
        /// </summary>
        /// <param name="requestId">The request Id to associate with the log.</param>
        /// <param name="tenantId">The identifier of the tenant to access data from</param>
        /// <param name="imageId">The identifier of the image being requested.</param>
        /// <param name="filepath">The path to where the file should be stored when saving.</param>
        /// <param name="filename">The name the image file to use when saving.</param>
        [JsonConstructor]
        public RequestWriteImageToWebClientMessage(
            RequestIdentifier requestId,
            string tenantId,
            ImageId imageId,
            string filepath,
            string filename)
        {
            Contract.Requires<ArgumentNullException>(requestId != null, nameof(requestId));
            Contract.Requires<ArgumentException>(requestId != RequestIdentifier.Empty, nameof(requestId));
            Contract.Requires<ArgumentNullException>(imageId != null, nameof(imageId));
            Contract.Requires<ArgumentException>(imageId != ImageId.Empty, nameof(imageId));
            Contract.Requires<ArgumentException>(!string.IsNullOrWhiteSpace(tenantId), nameof(tenantId));
            Contract.Requires<ArgumentException>(!string.IsNullOrWhiteSpace(filepath), nameof(filepath));
            Contract.Requires<ArgumentException>(!string.IsNullOrWhiteSpace(filename), nameof(filename));

            RequestId = requestId;
            TenantId = tenantId;
            ImageId = imageId;
            Filepath = filepath;
            Filename = filename;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestWriteImageToWebClientMessage"/> class
        /// suitable for use by an Activator.
        /// </summary>
        public RequestWriteImageToWebClientMessage()
        {
        }

        /// <summary>
        /// The request Id to associate with the log.
        /// </summary>
        public RequestIdentifier RequestId { get; }

        /// <summary>
        /// The identifier of the tentant to access data from
        /// </summary>
        public string TenantId { get; }

        /// <summary>
        /// The identifier of the image being requested.
        /// </summary>
        public ImageId ImageId { get; }

        /// <summary>
        /// The path to where the file should be stored when saving.
        /// </summary>
        public string Filepath { get; }

        /// <summary>
        /// The name the image file to use when saving.
        /// </summary>
        public string Filename { get; }

        /// <inheritdoc/>
        public string DefaultQueueName => RabbitQueue;

        /// <inheritdoc/>
        public string ExchangeName => RabbitExchange;

        /// <inheritdoc/>
        public string RoutingKey => RabbitKey;

        /// <inheritdoc/>
        public bool Equals(RequestWriteImageToWebClientMessage other)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }

            if (ReferenceEquals(other, this))
            {
                return true;
            }

            return RequestId.Equals(other.RequestId)
                && string.CompareOrdinal(TenantId, other.TenantId) == 0
                && ImageId.Equals(other.ImageId)
                && string.CompareOrdinal(Filepath, other.Filepath) == 0
                && string.CompareOrdinal(Filename, other.Filename) == 0;
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return Equals(obj as RequestWriteImageToWebClientMessage);
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
                result = (result * 31) + TenantId.GetHashCode();
                result = (result * 31) + ImageId.GetHashCode();
                result = (result * 31) + Filepath.GetHashCode();
                result = (result * 31) + Filename.GetHashCode();

                return result;
            }
        }
    }
}
