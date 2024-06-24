using System;
using System.Linq;
using Microsoft.Extensions.Options;
using RDM.Client.ImageVault;
using RDM.Core;
using RDM.Legacy.Itms;
using RDM.Model.Itms;
using RDM.Services.ImageViewerAPI;
using RDM.Webservice.ImageViewerAPI.Options;
using Serilog;

namespace RDM.Webservice.ImageViewerAPI.Factories
{
    public class ItmsItemServiceFactory : IItmsItemServiceFactory
    {
        private readonly RepositoryOptions _repoOptions;
        private readonly IMonitorFactory _monitorFactory;
        private readonly IImageVaultClient _imageVaultClient;
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ItmsItemServiceFactory" /> class.
        /// </summary>
        /// <param name="repoOptions">Provides the initialization and runtime options for the service</param>
        /// <param name="logger">The instance of the system logging mechanism.</param>
        /// <param name="monitorFactory">The monitor factory.</param>
        /// <param name="imageVaultClient">Access to ImageVault.</param>
        public ItmsItemServiceFactory(
            IOptions<RepositoryOptions> repoOptions,
            ILogger logger,
            IMonitorFactory monitorFactory,
            IImageVaultClient imageVaultClient)
        {
            Contract.Requires<ArgumentNullException>(repoOptions != null, nameof(repoOptions));
            Contract.Requires<ArgumentNullException>(logger != null, nameof(logger));
            Contract.Requires<ArgumentNullException>(monitorFactory != null, nameof(monitorFactory));
            Contract.Requires<ArgumentNullException>(imageVaultClient != null, nameof(imageVaultClient));

            _repoOptions = repoOptions.Value;
            _logger = logger;
            _monitorFactory = monitorFactory;
            _imageVaultClient = imageVaultClient;
        }

        public IItmsItemService Create(IRequestData requestData)
        {
            Contract.Requires<ArgumentNullException>(requestData != null, nameof(requestData));

            var requestDataAccessor = new RequestDataAccessor
            {
                PerformanceMonitor = _monitorFactory.Get(),
                RequestId = requestData.RequestId
            };

            var imageConverter = new ImageConverter(requestDataAccessor, _logger);

            var options = _repoOptions.Repositories.Where(r => r.Repository == ItmsItemRepository.ConfigName)
                .Select(r => r.Settings)
                .First();

            var itemItmsRepository = new ItmsItemRepository(requestDataAccessor, options);

            return new ItmsItemService(
                requestDataAccessor,
                _logger,
                _imageVaultClient,
                itemItmsRepository,
                imageConverter);
        }
    }
}
