using System;
using System.Collections;
using System.IO;
using Newtonsoft.Json;
using RDM.Core;
using RDM.Model.Itms;

namespace RDM.Messaging.ImageVault
{
    public class RequestAddImageMessage : IMessage, IEquatable<RequestAddImageMessage>
    {
        internal const string RabbitExchange = "imagevault.exchange";
        internal const string RabbitQueue = "imagevault.addimage.queue";
        internal const string RabbitKey = "imagevault.addimage.request";

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestAddImageMessage"/> class
        /// with the supplied information.
        /// </summary>
        /// <param name="requestId">The request Id to associate with the log.</param>
        /// <param name="content">The byte content of the image in a byte array.</param>
        /// <param name="mimeType">The mime type of the image being stored.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="requestId"/> or <paramref name="content"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="requestId"/> is <c>RequestIdentifier.Empty</c>
        /// or when <paramref name="mimeType"/> is <c>null</c> or whitespace.
        /// </exception>
        [JsonConstructor]
        public RequestAddImageMessage(RequestIdentifier requestId, byte[] content, string mimeType)
        {
            Contract.Requires<ArgumentNullException>(requestId != null, nameof(requestId));
            Contract.Requires<ArgumentException>(requestId != RequestIdentifier.Empty, nameof(requestId));
            Contract.Requires<ArgumentNullException>(content != null, nameof(content));
            Contract.Requires<ArgumentException>(content.Length > 0, "Image content must contain data.");
            Contract.Requires<ArgumentException>(!string.IsNullOrWhiteSpace(mimeType), nameof(mimeType));

            RequestId = requestId;
            Content = content;
            MimeType = mimeType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestAddImageMessage"/> class
        /// suitable for use by an Activator.
        /// </summary>
        public RequestAddImageMessage()
        {
        }

        /// <summary>
        /// The request Id to associate with the log.
        /// </summary>
        public RequestIdentifier RequestId { get; }

        /// <summary>
        /// The byte content of the image in a byte array.
        /// </summary>
        public byte[] Content { get; }

        /// <summary>
        /// The mime type of the image being stored.
        /// </summary>
        public string MimeType { get; }

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
        public bool Equals(RequestAddImageMessage other)
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
                StructuralComparisons.StructuralEqualityComparer.Equals(Content, other.Content) &&
                string.CompareOrdinal(MimeType, other.MimeType) == 0;
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return Equals(obj as RequestAddImageMessage);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                int p = 139;
                int result = 13;
                var content = new byte[Content.Length];
                Content.CopyTo(content, 0);

                for (int i = 0; i < content.Length; i++)
                {
                    result = (result ^ content[i]) * p;
                }

                result = (result * 31) + RabbitQueue.GetHashCode();
                result = (result * 31) + RabbitExchange.GetHashCode();
                result = (result * 31) + RabbitKey.GetHashCode();
                result = (result * 31) + RequestId.GetHashCode();
                result = (result * 31) + MimeType.GetHashCode();

                return result;
            }
        }
    }
}
