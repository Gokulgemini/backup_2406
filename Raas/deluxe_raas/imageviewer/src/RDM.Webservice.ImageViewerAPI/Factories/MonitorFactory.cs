using Microsoft.AspNetCore.Http;
using RDM.Statistician.PerformanceTimer;

namespace RDM.Webservice.ImageViewerAPI.Factories
{
    /// <inheritdoc/>
    public class MonitorFactory : IMonitorFactory
    {
        private readonly IHttpContextAccessor _accessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="MonitorFactory"/> class.
        /// </summary>
        /// <param name="accessor">Provides the current HttpContext instance.</param>
        public MonitorFactory(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }

        /// <inheritdoc/>
        public IPerformanceTimer Get()
        {
            var context = _accessor.HttpContext;
            if (context.Items.ContainsKey("PerformanceMonitor"))
            {
                return (IPerformanceTimer)context.Items["PerformanceMonitor"];
            }

            var monitor = new PerformanceTimer();
            context.Items.Add("PerformanceMonitor", monitor);

            return monitor;
        }
    }
}
