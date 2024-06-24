using System;
using Newtonsoft.Json;
using Serilog.Events;

namespace RDM.Messaging.ImageViewerAPI
{
    public class SetApplicationLogLevelMessage : IMessage, IEquatable<SetApplicationLogLevelMessage>
    {
        internal const string RabbitExchange = "imageviewer.operationsexchange";
        internal const string RabbitQueue = "imageviewer.applicationloglevel.queue";
        internal const string RabbitKey = "imageviewer.applicationloglevel";

        /// <summary>
        /// Initializes a new instance of the <see cref="SetApplicationLogLevelMessage"/> class.
        /// </summary>
        /// <param name="newLevel">The new logging level.</param>
        /// <remarks>
        /// If the message is sent wrong the level will be set to Verbose.
        /// <see cref="LogEventLevel"/> is an enum that has a default of Verbose which is a valid option.
        /// </remarks>
        [JsonConstructor]
        public SetApplicationLogLevelMessage(LogEventLevel newLevel)
        {
            NewLevel = newLevel;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SetApplicationLogLevelMessage"/> class.
        /// </summary>
        public SetApplicationLogLevelMessage()
        {
        }

        /// <summary>
        /// The desired application logging level
        /// </summary>
        public LogEventLevel NewLevel { get; set; }

        /// <inheritdoc/>
        public string ExchangeName => RabbitExchange;

        /// <inheritdoc/>
        public string DefaultQueueName => RabbitQueue;

        /// <inheritdoc/>
        public string RoutingKey => RabbitKey;

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return Equals(obj as SetApplicationLogLevelMessage);
        }

        /// <inheritdoc/>
        public bool Equals(SetApplicationLogLevelMessage other)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }

            if (ReferenceEquals(other, this))
            {
                return true;
            }

            return NewLevel == other.NewLevel;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                var result = RabbitKey.GetHashCode();
                result = (result * 31) + RabbitExchange.GetHashCode();
                result = (result * 31) + RabbitQueue.GetHashCode();
                result = (result * 31) + NewLevel.GetHashCode();

                return result;
            }
        }
    }
}
