using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Grpc.Core;
using RDM.Client.ImageVault;
using RDM.Core;
using RDM.Messaging.ImageVault;
using RDM.Model.Itms;
using RDM.Models.ImageViewerAPI;
using Serilog;

namespace RDM.Services.ImageViewerAPI
{
    public abstract class ItemService
    {
        private readonly IRequestDataAccessor _requestDataAccessor;
        private readonly ILogger _logger;
        private readonly IImageVaultClient _imageVaultClient;
        private readonly TargetHost _targetHost;

        private readonly IDictionary<TargetHost, LegacyTarget> _legacyTargetMapping = new Dictionary<TargetHost, LegacyTarget>
        {
            { TargetHost.Itms, LegacyTarget.Itms },
            { TargetHost.WebClient, LegacyTarget.WebClient }
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemService"/> class.
        /// </summary>
        /// <param name="requestDataAccessor">The request data accessor.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="imageVaultClient">The image vault client.</param>
        /// <param name="targetHost">The legacy target.</param>
        protected ItemService(
            IRequestDataAccessor requestDataAccessor,
            ILogger logger,
            IImageVaultClient imageVaultClient,
            TargetHost targetHost)
        {
            Contract.Requires<ArgumentNullException>(requestDataAccessor != null, nameof(requestDataAccessor));
            Contract.Requires<ArgumentNullException>(logger != null, nameof(logger));
            Contract.Requires<ArgumentNullException>(imageVaultClient != null, nameof(imageVaultClient));

            _requestDataAccessor = requestDataAccessor;
            _logger = logger;
            _imageVaultClient = imageVaultClient;
            _targetHost = targetHost;
        }

        public Image GetVirtualRemittance()
        {
            using (var imageStream = typeof(ItmsItemService)
                                     .GetTypeInfo()
                                     .Assembly.GetManifestResourceStream(
                                         "RDM.Services.ImageViewerAPI.resources.VirtualRemit.png"))
            {
                using (var imageMemoryStream = new MemoryStream())
                {
                    imageStream?.CopyTo(imageMemoryStream);

                    return new Image(imageMemoryStream.ToArray(), "image/png", 250, 125);
                }
            }
        }

        public async Task<Result<Error, Image>> GetImageFromVaultAsync(
            IrnId irn,
            int seqNum,
            ImageSurface surface,
            int page,
            UserId userId,
            string tenantId)
        {
            var legacyTarget = _legacyTargetMapping[_targetHost];

            _logger.Debug(
                "Requesting Image for item '{IRN}',Surface {Surface}, SeqNum '{SeqNum}', Page '{Page}'. Call is for UserId '{UserId}', Target '{TargetHost}', TenantId '{TenantId}'. RequestId '{RequestId}'.",
                irn,
                surface,
                seqNum,
                page,
                userId.Value,
                legacyTarget,
                tenantId,
                _requestDataAccessor.RequestId);

            try
            {
                var image = await _imageVaultClient.GetImageForLegacy(
                    _requestDataAccessor.RequestId,
                    legacyTarget,
                    tenantId,
                    userId,
                    irn,
                    seqNum,
                    surface,
                    page);

                return image;
            }
            catch (RpcException exception)
            {
                _logger.Information(
                    $"Could not find a {{Surface}} image for item '{{IRN}}', UserId '{{UserId}}', SeqNum '{{SeqNum}}', Page '{{Page}}', TenantId '{{TenantId}}'. RequestId '{{RequestId}}'. Additional Details: Status code: {exception.StatusCode}, message: {exception.Status.Detail}",
                    surface,
                    irn,
                    userId.Value,
                    seqNum,
                    page,
                    tenantId,
                    _requestDataAccessor.RequestId);

                // if it is anything but NotFound we need to rethrow and let exception handle deal with it
                if (exception.StatusCode != StatusCode.NotFound)
                {
                    throw;
                }

                return new NotFound($"{surface} image on page {page} could not be found.");
            }
        }

        protected async Task<Result<Error, Image>> GetFrontImageFromVaultAsync(
            IrnId irn,
            int seqNum,
            int page,
            UserId userId,
            string tenantId)
        {
            return await GetImageFromVaultAsync(irn, seqNum, ImageSurface.Front, page, userId, tenantId);
        }

        protected async Task<Result<Error, Image>> GetBackImageFromVaultAsync(
            IrnId irn,
            int seqNum,
            int page,
            UserId userId,
            string tenantId)
        {
            return await GetImageFromVaultAsync(irn, seqNum, ImageSurface.Back, page, userId, tenantId);
        }
    }
}
