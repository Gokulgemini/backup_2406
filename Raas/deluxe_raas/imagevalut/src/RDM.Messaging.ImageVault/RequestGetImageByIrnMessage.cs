using System;
using Newtonsoft.Json;
using RDM.Core;
using RDM.Model.Itms;

namespace RDM.Messaging.ImageVault
{
    public class RequestGetImageByIrnMessage : IMessage, IEquatable<RequestGetImageByIrnMessage>
    {
        internal const string RabbitExchange = "imagevault.exchange";
        internal const string RabbitQueue = "imagevault.getimagebyirn.queue";
        internal const string RabbitKey = "imagevault.getimagebyirn.request";

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestGetImageByIrnMessage"/> class
        /// with the supplied information.
        /// </summary>
        /// <param name="requestId">The request Id to associate with the log.</param>
        /// <param name="tenantId">The identifier of the tentant to access data from</param>
        /// <param name="irnId">The identifier for the item</param>
        /// <param name="surface">The surface of the item images</param>
        /// <param name="page">The page of the item's images</param>
        [JsonConstructor]
        public RequestGetImageByIrnMessage(RequestIdentifier requestId, string tenantId, IrnId irnId, ImageSurface surface, int page)
        {
            Contract.Requires<ArgumentNullException>(requestId != null, nameof(requestId));
            Contract.Requires<ArgumentNullException>(irnId != null, nameof(irnId));
            Contract.Requires<ArgumentException>(requestId != RequestIdentifier.Empty, nameof(requestId));
            Contract.Requires<ArgumentException>(irnId != IrnId.Empty, nameof(irnId));

            RequestId = requestId;
            TenantId = tenantId;
            IrnId = irnId;
            Surface = surface;
            Page = page;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestGetImageByIrnMessage"/> class
        /// suitable for use by an Activator.
        /// </summary>
        public RequestGetImageByIrnMessage()
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
        /// The identifier of the item.
        /// </summary>
        public IrnId IrnId { get; }

        /// <summary>
        /// The surface of the item image.
        /// </summary>
        public ImageSurface Surface { get; }

        /// <summary>
        /// The desired page of the item's images.
        /// </summary>
        public int Page { get; }

        /// <inheritdoc/>
        public string DefaultQueueName => RabbitQueue;

        /// <inheritdoc/>
        public string ExchangeName => RabbitExchange;

        /// <inheritdoc/>
        public string RoutingKey => RabbitKey;

        /// <inheritdoc/>
        public bool Equals(RequestGetImageByIrnMessage other)
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
                && IrnId.Equals(other.IrnId)
                && Surface.Equals(other.Surface)
                && Equals(Page, other.Page);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return Equals(obj as RequestGetImageByIrnMessage);
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
                result = (result * 31) + IrnId.GetHashCode();
                result = (result * 31) + Surface.GetHashCode();
                result = (result * 31) + Page.GetHashCode();

                return result;
            }
        }
    }
}
