namespace RDM.Webservice.ImageViewerAPI.Options
{
    public class GrpcOptions
    {
        /// <summary>
        /// The address of the grpc queue host
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// Timeout/deadline for grpc calls
        /// </summary>
        public int? Deadline { get; set; }
    }
}
