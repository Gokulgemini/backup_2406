using System;
using System.Collections.Generic;
using RDM.Model.Itms;

namespace RDM.Webservice.ImageViewerAPI
{
    /// <summary>
    /// Provides configuration information from the request context.
    /// </summary>
    public interface IRequestData
    {
        /// <summary>
        /// The user id for the currently authenticated user.
        /// </summary>
        UserId UserId { get; set; }

        /// <summary>
        /// The name of the application making the request.
        /// </summary>
        string ApplicationName { get; }

        /// <summary>
        /// The version of the application making the request.
        /// </summary>
        string ApplicationVersion { get; }

        /// <summary>
        /// The identifier of the tenant to access data from.
        /// </summary>
        string TenantId { get; }

        /// <summary>
        /// The request id to associate with the logs.
        /// </summary>
        RequestIdentifier RequestId { get; }

        /// <summary>
        /// The time that the user was last authenticated.
        /// </summary>
        DateTime AuthenticationDateTimeUtc { get; }
    }
}
