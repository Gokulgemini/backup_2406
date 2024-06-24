using System;
using Newtonsoft.Json;
using RDM.Core;
using RDM.Model.Itms;

namespace RDM.Messaging.ImageVault
{
    public class ReplyGetImageByIrnMessageLeg1 : IMessage, IEquatable<ReplyGetImageByIrnMessageLeg1>
    {
        internal const string RabbitExchange = "imagevault.exchange";
        internal const string RabbitQueue = "imagevault.getimagebyirnleg1.queue";
        internal const string RabbitKey = "imagevault.getimagebyirnleg1.reply";

        /// <summary>
        /// Initializes a new instance of the <see cref="ReplyGetImageByIrnMessageLeg1"/> class
        /// with the supplied information.
        /// </summary>
        /// <param name="status">The status of the request to retrieve the image.</param>
        /// <param name="image">The image retrieved, if found.</param>
        /// <exception cref="ArgumentException">
        /// Thrown when when <paramref name="image"/> is <c>null</c> and <paramref name="status"/> is <c>Success</c>.
        /// </exception>
        [JsonConstructor]
        public ReplyGetImageByIrnMessageLeg1(GetImageStatus status, Image image)
        {
            Contract.Requires<ArgumentException>(
                status != GetImageStatus.Success || (status == GetImageStatus.Success && image != null),
                "Success without image.");

            Status = status;
            Image = image;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReplyGetImageByIrnMessageLeg1"/> class
        /// suitable for use by an Activator.
        /// </summary>
        public ReplyGetImageByIrnMessageLeg1()
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
        public bool Equals(ReplyGetImageByIrnMessageLeg1 other)
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
            return Equals(obj as ReplyGetImageByIrnMessageLeg1);
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