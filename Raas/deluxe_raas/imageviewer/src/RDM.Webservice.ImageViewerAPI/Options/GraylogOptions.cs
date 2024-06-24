using Serilog.Sinks.Graylog.Core.Transport;

namespace RDM.Webservice.ImageViewerAPI.Options
{
    public class GraylogOptions
    {
        /// <summary>
        /// The address of the message queue host
        /// </summary>
        public string HostnameOrAddress { get; set; }

        /// <summary>
        /// The username to access the message host with
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// The password to access the message host with
        /// </summary>
        public TransportType TransportType { get; set; }
    }
}
