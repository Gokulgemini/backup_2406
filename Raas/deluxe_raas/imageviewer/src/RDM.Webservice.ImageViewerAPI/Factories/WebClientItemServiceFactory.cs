using System;
using System.Linq;
using Microsoft.Extensions.Options;
using RDM.Client.ImageVault;
using RDM.Client.Tracker;
using RDM.Core;
using RDM.Legacy.WebClientDb;
using RDM.Model.Itms;
using RDM.Services.ImageViewerAPI;
using RDM.Webservice.ImageViewerAPI.Options;
using Serilog;

namespace RDM.Webservice.ImageViewerAPI.Factories
{
    public class WebClientItemServiceFactory : IWebClientItemServiceFactory
    {
        private readonly RepositoryOptions _repoOptions;
        private readonly IMonitorFactory _monitorFactory;
        private readonly IImageVaultClient _imageVaultClient;
        private readonly ILogger _logger;
        private readonly ITrackerClient _trackerClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebClientItemServiceFactory" /> class.
        /// </summary>
        /// <param name="repoOptions">Provides the initialization and runtime options for the service</param>
        /// <param name="logger">The instance of the system logging mechanism.</param>
        /// <param name="monitorFactory">The monitor factory.</param>
        /// <param name="imageVaultClient">Access to ImageVault.</param>
        /// <param name="trackerClient">Provides access to tracker for audits.</param>
        public WebClientItemServiceFactory(
            IOptions<RepositoryOptions> repoOptions,
            ILogger logger,
            IMonitorFactory monitorFactory,
            IImageVaultClient imageVaultClient,
            ITrackerClient trackerClient)
        {
            Contract.Requires<ArgumentNullException>(repoOptions != null, nameof(repoOptions));
            Contract.Requires<ArgumentNullException>(logger != null, nameof(logger));
            Contract.Requires<ArgumentNullException>(monitorFactory != null, nameof(monitorFactory));
            Contract.Requires<ArgumentNullException>(imageVaultClient != null, nameof(imageVaultClient));
            Contract.Requires<ArgumentNullException>(trackerClient != null, nameof(trackerClient));

            _repoOptions = repoOptions.Value;
            _logger = logger;
            _monitorFactory = monitorFactory;
            _imageVaultClient = imageVaultClient;
            _trackerClient = trackerClient;
        }

        public IWebClientItemService Create(IRequestData requestData)
        {
            Contract.Requires<ArgumentNullException>(requestData != null, nameof(requestData));

            var requestDataAccessor = new RequestDataAccessor
            {
                PerformanceMonitor = _monitorFactory.Get(),
                RequestId = requestData.RequestId
            };

            var imageConverter = new ImageConverter(requestDataAccessor, _logger);

            var options = _repoOptions
                          .Repositories
                          .Where(r => r.Repository == WebClientItemRepository.ConfigName + "_" + requestData.TenantId)
                          .Select(r => r.Settings)
                          .First();

            var itemRepository = new WebClientItemRepository(requestDataAccessor, options);

            return new WebClientItemService(
                requestDataAccessor,
                _logger,
                _imageVaultClient,
                itemRepository,
                imageConverter,
                _trackerClient);
        }
    }
}
