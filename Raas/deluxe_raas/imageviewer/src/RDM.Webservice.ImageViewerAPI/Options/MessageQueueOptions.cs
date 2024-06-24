using System.Collections.Generic;
using RDM.Messaging.RabbitMQ;

namespace RDM.Webservice.ImageViewerAPI.Options
{
    public class MessageQueueOptions
    {
        /// <summary>
        /// The address of the message queue host
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// The username to access the message host with
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// The password to access the message host with
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Provides a helper to convert the configured settings to a dictionary
        /// suitable for consumption by a RabbitQueue constructor.
        /// </summary>
        /// <returns>
        /// Returns a dictionary containing the configured values associated with
        /// the correct options key.
        /// </returns>
        public IDictionary<string, string> ToDictionary()
        {
            var result = new Dictionary<string, string>
            {
                { RabbitOption.RabbitHost, Host },
                { RabbitOption.Username, Username },
                { RabbitOption.Password, Password }
            };

            return result;
        }
    }
}
