using System.Collections.Generic;

namespace RDM.Webservice.ImageViewerAPI.Options
{
    /// <summary>
    /// Defines various repository options and initialization
    /// </summary>
    public class RepositoryOptions
    {
        /// <summary>
        /// The array of the repositories
        /// </summary>
        public RepositoryEntry[] Repositories { get; set; }

        /// <summary>
        /// Provides the repository information
        /// </summary>
        public class RepositoryEntry
        {
            /// <summary>
            /// The name of the repository
            /// </summary>
            public string Repository { get; set; }

            /// <summary>
            /// The settings of the repository
            /// </summary>
            public Dictionary<string, string> Settings { get; set; }
        }
    }
}
