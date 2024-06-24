using System;
using Newtonsoft.Json;
using Serilog.Events;

namespace RDM.Messaging.ImageVault
{
    public class SetApplicationLogLevelMessage : IMessage, IEquatable<SetApplicationLogLevelMessage>
    {
        internal const string RabbitExchange = "imagevault.operationsexchange";
        internal const string RabbitQueue = "imagevault.applicationloglevel.queue";
        internal const string RabbitKey = "imagevault.applicationloglevel";

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

        #region IMessage
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
        #endregion

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
