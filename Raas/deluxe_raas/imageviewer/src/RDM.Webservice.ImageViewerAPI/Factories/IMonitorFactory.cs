using RDM.Statistician.PerformanceTimer;

namespace RDM.Webservice.ImageViewerAPI.Factories
{
    /// <summary>
    /// Provides access to the performance monitor for this requrest.
    /// </summary>
    public interface IMonitorFactory
    {
        /// <summary>
        /// Gets or creates a performance monitor.
        /// </summary>
        /// <returns>
        /// Returns the newly created performance monitor if not previously accessed,
        /// otherwise returns the previously created performance monitor.
        /// </returns>
        IPerformanceTimer Get();
    }
}
