using System;
using RDM.Core;
using RDM.Model.Itms;
using RDM.Statistician.PerformanceTimer;

namespace RDM.Webservice.ImageViewerAPI
{
    /// <summary>
    /// A lightweight implementation of <see cref="IRequestDataAccessor"/> suitable for consumption
    /// by services that are not shared betwen request contexts and that know nothing about
    /// ASP.NET Core's dependency injection container.
    /// </summary>
    public class ServiceRequestDataAccessor : IRequestDataAccessor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceRequestDataAccessor"/> class
        /// with the supplied information.
        /// </summary>
        /// <param name="requestId">The request id to associate with the logs.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="requestId"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="requestId"/> is empty.</exception>
        public ServiceRequestDataAccessor(RequestIdentifier requestId)
        {
            Contract.Requires<ArgumentNullException>(requestId != null, nameof(requestId));
            Contract.Requires<ArgumentException>(requestId != RequestIdentifier.Empty, nameof(requestId));

            RequestId = requestId;
        }

        /// <inheritdoc />
        public IPerformanceTimer PerformanceMonitor { get; set; }

        /// <inheritdoc />
        public RequestIdentifier RequestId { get; set; }
    }
}
