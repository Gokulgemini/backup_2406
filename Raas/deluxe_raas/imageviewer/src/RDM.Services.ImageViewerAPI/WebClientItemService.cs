using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RDM.Client.ImageVault;
using RDM.Client.Tracker;
using RDM.Client.Tracker.DTO;
using RDM.Core;
using RDM.Legacy.WebClientDb;
using RDM.Model.Itms;
using RDM.Models.ImageViewerAPI;
using Serilog;

namespace RDM.Services.ImageViewerAPI
{
    /// <summary>
    /// The service called on by the ImagesController to perform operations in WebClient host and ImageVaultClient.
    /// </summary>
    /// <seealso cref="WebClientItemService" />
    public class WebClientItemService : ItemService, IWebClientItemService
    {
        private readonly IRequestDataAccessor _requestDataAccessor;
        private readonly ILogger _logger;
        private readonly IWebClientItemRepository _itemWebClientRepository;
        private readonly IImageConverter _imageConverter;
        private readonly ITrackerClient _trackerClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebClientItemService"/> class.
        /// </summary>
        /// <param name="requestDataAccessor">The request data accessor.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="imageVaultClient">The image vault client.</param>
        /// <param name="itemWebClientRepository">Access to legacy item level details (webClient host)</param>
        /// <param name="imageConverter">Conversion logic to use when converting images to different formats.</param>
        /// <param name="trackerClient">Provides access to tracker for audits.</param>
        public WebClientItemService(
            IRequestDataAccessor requestDataAccessor,
            ILogger logger,
            IImageVaultClient imageVaultClient,
            IWebClientItemRepository itemWebClientRepository,
            IImageConverter imageConverter,
            ITrackerClient trackerClient)
            : base(requestDataAccessor, logger, imageVaultClient, TargetHost.WebClient)
        {
            Contract.Requires<ArgumentNullException>(requestDataAccessor != null, nameof(requestDataAccessor));
            Contract.Requires<ArgumentNullException>(logger != null, nameof(logger));
            Contract.Requires<ArgumentNullException>(itemWebClientRepository != null, nameof(itemWebClientRepository));
            Contract.Requires<ArgumentNullException>(imageConverter != null, nameof(imageConverter));
            Contract.Requires<ArgumentNullException>(trackerClient != null, nameof(trackerClient));

            _requestDataAccessor = requestDataAccessor;
            _logger = logger;
            _itemWebClientRepository = itemWebClientRepository;
            _imageConverter = imageConverter;
            _trackerClient = trackerClient;
        }

        public async Task<Result<Error, bool>> UpdateDocumentNameByIrnAsync(
            IrnId irn,
            string documentName,
            UserId userId,
            Module module)
        {
            Contract.Requires<ArgumentNullException>(irn != null, "An IRN must be provided.");
            Contract.Requires<ArgumentException>(irn != IrnId.Empty, nameof(irn));
            Contract.Requires<ArgumentNullException>(userId != null, nameof(userId));
            Contract.Requires<ArgumentException>(userId != UserId.InvalidUser, nameof(userId));
            Contract.Requires<ArgumentNullException>(documentName != null, "Document name must be provided.");

            var requestId = _requestDataAccessor.RequestId;
            var monitor = _requestDataAccessor.PerformanceMonitor;

            monitor?.Start("WebClientItemService.UpdateDocumentNameByIrnAsync");

            try
            {
                _logger.Debug(
                    "UpdateDocumentNameByIrnAsync called with IRN '{IRN}', documentName '{documentName}', UserId '{UserId}', module '{module}'. RequestId '{RequestId}'.",
                    irn,
                    documentName,
                    userId.Value,
                    module.ToString(),
                    requestId);

                var userIdValue = int.Parse(userId.Value);

                var originalDetails = await _itemWebClientRepository.GetGeneralDocumentDetailsByIrnAsync(irn, userIdValue);
                if (!originalDetails.HasValue)
                {
                    _logger.Information(
                        "UpdateDocumentNameByIrnAsync failed to find document details for IRN '{IRN}', documentName '{documentName}', UserId '{UserId}', RequestId '{RequestId}'.",
                        irn,
                        documentName,
                        userId.Value,
                        requestId);
                    return new NotFound($"Document not found for IRN:{irn}");
                }

                var result = await _itemWebClientRepository.UpdateDocumentNameByIrnAsync(irn, userIdValue, documentName);
                if (result.IsFailure)
                {
                    _logger.Information(
                        $"UpdateDocumentNameByIrnAsync failed to update document name for IRN '{{IRN}}', documentName '{{documentName}}', UserId '{{UserId}}', RequestId '{{RequestId}}'. Failure code '{result.FailureValue}', Failure Message '{result.FailureValue.Message}'.",
                        irn,
                        documentName,
                        userId.Value,
                        requestId);

                    return result.FailureValue;
                }

                await _trackerClient.ChangeDocumentNameActivityAsync(
                    requestId,
                    new ChangeDocumentNameActivity(
                        DateTimeOffset.UtcNow,
                        userId,
                        originalDetails.Value.AccountId,
                        irn,
                        originalDetails.Value.DocumentName ?? string.Empty,
                        documentName,
                        module.ToString()));

                return result.Value;
            }
            catch (Exception e)
            {
                _logger.Error(
                    e,
                    "UpdateDocumentNameByIrnAsync ran into an exception when updating document name associated with IRN '{IRN}', documentName '{documentName}', UserId '{UserId}'. Please contact deployment about this issue. RequestId '{RequestId}'.",
                    irn,
                    documentName,
                    userId.Value,
                    requestId);

                return new ServiceFailure("The service ran into an exception when attempting to update document name.");
            }
            finally
            {
                monitor?.Stop("WebClientItemService.UpdateDocumentNameByIrnAsync");
            }
        }

        public async Task<Result<Error, Cheque>> GetChequeByIrnAsync(IrnId irn, UserId userId, string tenantId)
        {
            Contract.Requires<ArgumentNullException>(irn != null, "An IRN must be provided.");
            Contract.Requires<ArgumentException>(irn != IrnId.Empty, nameof(irn));
            Contract.Requires<ArgumentNullException>(userId != null, nameof(userId));
            Contract.Requires<ArgumentException>(userId != UserId.InvalidUser, nameof(userId));
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(tenantId), nameof(tenantId));

            var requestId = _requestDataAccessor.RequestId;
            var monitor = _requestDataAccessor.PerformanceMonitor;

            monitor?.Start("WebClientItemService.GetChequeByIrnAsync");

            try
            {
                _logger.Debug(
                    "GetChequeByIrnAsync called with IRN '{IRN}', UserId '{UserId}', TenantId '{TenantId}' RequestId '{RequestId}'.",
                    irn,
                    userId.Value,
                    tenantId,
                    requestId);

                var imageElementDetailsResult = await _itemWebClientRepository.GetImageElementDetailsByIrnAsync(irn);
                if (!imageElementDetailsResult.HasValue)
                {
                    _logger.Information(
                        "GetChequeByIrnAsync failed to find image for IRN '{IRN}', UserId '{UserId}', TenantId '{TenantId}', RequestId '{RequestId}'.",
                        irn,
                        userId.Value,
                        tenantId,
                        requestId);

                    return new NotFound($"Details could not be found for cheque with IRN {irn}.");
                }

                _logger.Debug(
                    "GetChequeByIrnAsync found image for IRN '{IRN}', UserId '{UserId}', TenantId '{TenantId}', RequestId '{RequestId}'.",
                    irn,
                    userId.Value,
                    tenantId,
                    requestId);

                var imageElementDetails = imageElementDetailsResult.Value;
                var page = imageElementDetails[0].Page;
                var seqNum = imageElementDetails[0].SeqNum;

                var frontImageResult = await GetFrontImageFromVaultAsync(irn, seqNum, page, userId, tenantId);
                if (frontImageResult.IsFailure)
                {
                    _logger.Information(
                        $"GetChequeByIrnAsync failed to get front image for IRN '{{IRN}}', SeqNum '{{SeqNum}}', page '{page}', UserId '{{UserId}}', TenantId '{{TenantId}}', RequestId '{{RequestId}}'. Failure code '{frontImageResult.FailureValue}', Failure Message '{frontImageResult.FailureValue.Message}'.",
                        irn,
                        seqNum,
                        userId.Value,
                        tenantId,
                        requestId);

                    return frontImageResult.FailureValue;
                }

                var convertedFront = await _imageConverter.ConvertImageToPng(frontImageResult.Value);

                if (imageElementDetails.Any(d => d.Surface == ImageSurface.Back))
                {
                    var backImageResult = await GetBackImageFromVaultAsync(irn, seqNum, page, userId, tenantId);
                    if (backImageResult.IsFailure)
                    {
                        _logger.Information(
                            $"GetChequeByIrnAsync failed to get rear image for IRN '{{IRN}}', SeqNum '{{SeqNum}}', page '{page}', UserId '{{UserId}}', TenantId '{{TenantId}}', RequestId '{{RequestId}}'. Failure code '{backImageResult.FailureValue}', Failure Message '{backImageResult.FailureValue.Message}'.",
                            irn,
                            seqNum,
                            userId.Value,
                            tenantId,
                            requestId);

                        return backImageResult.FailureValue;
                    }

                    var convertedBack = await _imageConverter.ConvertImageToPng(backImageResult.Value);

                    return convertedFront.OnSuccess(
                        frontImage => convertedBack.OnSuccess(backImage => Cheque.TwoSided(frontImage, backImage)));
                }

                return convertedFront.OnSuccess(Cheque.OneSided);
            }
            catch (Exception e)
            {
                _logger.Error(
                    e,
                    "GetChequeByIrnAsync ran into an exception when looking for images associated with IRN '{IRN}' for UserId '{UserId}', TenantId '{TenantId}'. Please contact a developer about this issue. RequestId '{RequestId}'.",
                    irn,
                    userId.Value,
                    tenantId,
                    requestId);

                return new ServiceFailure("The service ran into an exception when attempting to get the image.");
            }
            finally
            {
                monitor?.Stop("WebClientItemService.GetChequeByIrnAsync");
            }
        }

        public async Task<Result<Error, Remittance>> GetRemittanceByIrnAsync(IrnId irn, UserId userId, string tenantId)
        {
            Contract.Requires<ArgumentNullException>(irn != null, "An IRN must be provided.");
            Contract.Requires<ArgumentException>(irn != IrnId.Empty, nameof(irn));
            Contract.Requires<ArgumentNullException>(userId != null, nameof(userId));
            Contract.Requires<ArgumentException>(userId != UserId.InvalidUser, nameof(userId));
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(tenantId), nameof(tenantId));

            var requestId = _requestDataAccessor.RequestId;
            var monitor = _requestDataAccessor.PerformanceMonitor;

            monitor?.Start("WebClientItemService.GetRemittanceByIrnAsync");

            try
            {
                _logger.Debug(
                    "GetRemittanceByIrnAsync called with IRN '{IRN}', UserId '{UserId}', TenantId '{TenantId}', RequestId '{RequestId}'.",
                    irn,
                    userId.Value,
                    tenantId,
                    requestId);

                var imageElementDetailsResult = await _itemWebClientRepository.GetImageElementDetailsByIrnAsync(irn);
                if (!imageElementDetailsResult.HasValue)
                {
                    // We have no reliable way to determine a Virtual remit, so we have to assume if we
                    // can't find images on a remit is must be virtual ¯\_(ツ)_/¯
                    _logger.Information(
                         "GetRemittanceByIrnAsync failed to find image for IRN '{IRN}', UserId '{UserId}', TenantId '{TenantId}', RequestId '{RequestId}'.",
                         irn,
                         userId.Value,
                         tenantId,
                         requestId);

                    return Result<Error, Remittance>.Success(Remittance.Virtual(GetVirtualRemittance()));
                }

                _logger.Debug(
                    "GetRemittanceByIrnAsync found image for IRN '{IRN}', UserId '{UserId}', TenantId '{TenantId}', RequestId '{RequestId}'.",
                    irn,
                    userId.Value,
                    tenantId,
                    requestId);

                var imageElementDetails = imageElementDetailsResult.Value;
                var page = imageElementDetails[0].Page;
                var seqNum = imageElementDetails[0].SeqNum;

                var frontImageResult = await GetFrontImageFromVaultAsync(irn, seqNum, page, userId, tenantId);
                if (frontImageResult.IsFailure)
                {
                    _logger.Information(
                        $"GetRemittanceByIrnAsync failed to get front image for IRN '{{IRN}}', SeqNum '{{SeqNum}}', page '{page}', UserId '{{UserId}}', TenantId '{{TenantId}}', RequestId '{{RequestId}}'. Failure code '{frontImageResult.FailureValue}', Failure Message '{frontImageResult.FailureValue.Message}'.",
                        irn,
                        seqNum,
                        userId.Value,
                        tenantId,
                        requestId);
                    return frontImageResult.FailureValue;
                }

                var convertedFront = await _imageConverter.ConvertImageToPng(frontImageResult.Value);
                if (convertedFront.IsFailure)
                {
                    _logger.Information(
                        $"GetRemittanceByIrnAsync failed to convert front image to PNG for IRN '{{IRN}}', SeqNum '{{SeqNum}}', page '{page}', UserId '{{UserId}}', TenantId '{{TenantId}}', RequestId '{{RequestId}}'. Failure code '{convertedFront.FailureValue}', Failure Message '{convertedFront.FailureValue.Message}'.",
                        irn,
                        seqNum,
                        userId.Value,
                        tenantId,
                        requestId);

                    return convertedFront.FailureValue;
                }

                if (imageElementDetails.Any(d => d.Surface == ImageSurface.Back))
                {
                    var backImageResult = await GetBackImageFromVaultAsync(irn, seqNum, page, userId, tenantId);
                    if (backImageResult.IsFailure)
                    {
                        _logger.Information(
                            $"GetRemittanceByIrnAsync failed to get rear image for IRN '{{IRN}}', SeqNum '{{SeqNum}}', page '{page}', UserId '{{UserId}}', TenantId '{{TenantId}}', RequestId '{{RequestId}}'. Failure code '{backImageResult.FailureValue}', Failure Message '{backImageResult.FailureValue.Message}'.",
                            irn,
                            seqNum,
                            userId.Value,
                            tenantId,
                            requestId);
                        return backImageResult.FailureValue;
                    }

                    var convertedBack = await _imageConverter.ConvertImageToPng(backImageResult.Value);
                    if (convertedBack.IsFailure)
                    {
                        _logger.Information(
                            $"GetRemittanceByIrnAsync failed to convert rear image to PNG for IRN '{{IRN}}', SeqNum '{{SeqNum}}', page '{page}', UserId '{{UserId}}', TenantId '{{TenantId}}', RequestId '{{RequestId}}'. Failure code '{convertedBack.FailureValue}', Failure Message '{convertedBack.FailureValue.Message}'.",
                            irn,
                            seqNum,
                            userId.Value,
                            tenantId,
                            requestId);

                        return convertedBack.FailureValue;
                    }

                    return convertedFront.OnSuccess(
                        frontImage => convertedBack.OnSuccess(backImage => Remittance.TwoSided(frontImage, backImage)));
                }

                return convertedFront.OnSuccess(Remittance.OneSided);
            }
            catch (Exception e)
            {
                _logger.Error(
                     e,
                     "GetRemittanceByIrnAsync ran into an exception when looking for images associated with IRN '{IRN}' for UserId '{UserId}', TenantId '{TenantId}'. Please contact deployment about this issue. RequestId '{RequestId}'.",
                     irn,
                     userId.Value,
                     tenantId,
                     requestId);

                return new ServiceFailure("The service ran into an exception when attempting to get the image.");
            }
            finally
            {
                monitor?.Stop("WebClientItemService.GetRemittanceByIrnAsync");
            }
        }

        public async Task<Result<Error, GeneralDocument>> GetGeneralDocumentByIrnAsync(IrnId irn, UserId userId, string tenantId)
        {
            Contract.Requires<ArgumentNullException>(irn != null, "An IRN must be provided.");
            Contract.Requires<ArgumentException>(irn != IrnId.Empty, nameof(irn));
            Contract.Requires<ArgumentNullException>(userId != null, nameof(userId));
            Contract.Requires<ArgumentException>(userId != UserId.InvalidUser, nameof(userId));
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(tenantId), nameof(tenantId));

            var requestId = _requestDataAccessor.RequestId;
            var monitor = _requestDataAccessor.PerformanceMonitor;

            monitor?.Start("WebClientItemService.GetGeneralDocumentByIrnAsync");

            try
            {
                _logger.Debug(
                   "GetGeneralDocumentByIrnAsync called with IRN '{IRN}', UserId '{UserId}', TenantId '{TenantId}' RequestId '{RequestId}'.",
                   irn,
                   userId.Value,
                   tenantId,
                   requestId);

                var userIdValue = int.Parse(userId.Value);

                var itemGeneral = await _itemWebClientRepository.GetGeneralDocumentDetailsByIrnAsync(irn, userIdValue);

                if (!itemGeneral.HasValue)
                {
                    _logger.Information(
                        "GetGeneralDocumentByIrnAsync failed to find document details for IRN '{IRN}', UserId '{UserId}', TenantId '{TenantId}', RequestId '{RequestId}'.",
                        irn,
                        userId.Value,
                        tenantId,
                        requestId);
                    return new NotFound($"Details could not be found for general document with IRN {irn}.");
                }

                var imageElementDetailsResult = await _itemWebClientRepository.GetImageElementDetailsByIrnAsync(irn);
                if (!imageElementDetailsResult.HasValue)
                {
                    _logger.Information(
                        "GetGeneralDocumentByIrnAsync failed to find image for IRN '{IRN}', UserId '{UserId}', TenantId '{TenantId}', RequestId '{RequestId}'.",
                        irn,
                        userId.Value,
                        tenantId,
                        requestId);
                    return new NotFound($"Details could not be found for general document with IRN {irn}.");
                }

                _logger.Debug(
                    "GetGeneralDocumentByIrnAsync found image for IRN '{IRN}', UserId '{UserId}', TenantId '{TenantId}', RequestId '{RequestId}'.",
                    irn,
                    userId.Value,
                    tenantId,
                    requestId);

                var imageElementDetails = imageElementDetailsResult
                                          .Value.GroupBy(details => details.Page)
                                          .Select(group => new
                                          {
                                              Page = group.Key,
                                              Surfaces = group.Select(d => d.Surface),
                                              SeqNum = group.Select(d => d.SeqNum).FirstOrDefault()
                                          })
                                          .ToList();

                var generalDocumentPages = new List<GeneralDocumentPage>();
                foreach (var details in imageElementDetails)
                {
                    var frontImageResult = await GetFrontImageFromVaultAsync(irn, details.SeqNum, details.Page, userId, tenantId);
                    if (frontImageResult.IsFailure)
                    {
                        _logger.Information(
                            $"GetGeneralDocumentByIrnAsync failed to get front image for IRN '{{IRN}}', SeqNum '{{SeqNum}}', page '{details.Page}', UserId '{{UserId}}', TenantId '{{TenantId}}', RequestId '{{RequestId}}'. Failure code '{frontImageResult.FailureValue}', Failure Message '{frontImageResult.FailureValue.Message}'.",
                            irn,
                            details.SeqNum,
                            userId.Value,
                            tenantId,
                            requestId);
                        return frontImageResult.FailureValue;
                    }

                    var convertedFront = await _imageConverter.ConvertImageToPng(frontImageResult.Value);
                    if (convertedFront.IsFailure)
                    {
                        _logger.Information(
                            $"GetGeneralDocumentByIrnAsync failed to convert front image to PNG for IRN '{{IRN}}', SeqNum '{{SeqNum}}', page '{details.Page}', UserId '{{UserId}}', TenantId '{{TenantId}}', RequestId '{{RequestId}}'. Failure code '{convertedFront.FailureValue}', Failure Message '{convertedFront.FailureValue.Message}'.",
                            irn,
                            details.SeqNum,
                            userId.Value,
                            tenantId,
                            requestId);

                        return convertedFront.FailureValue;
                    }

                    if (details.Surfaces.Any(surface => surface == ImageSurface.Back))
                    {
                        var backImageResult = await GetBackImageFromVaultAsync(irn, details.SeqNum, details.Page, userId, tenantId);
                        if (backImageResult.IsFailure)
                        {
                            _logger.Information(
                                $"GetGeneralDocumentByIrnAsync failed to get rear image for IRN '{{IRN}}', SeqNum '{{SeqNum}}', page '{details.Page}', UserId '{{UserId}}', TenantId '{{TenantId}}', RequestId '{{RequestId}}'. Failure code '{backImageResult.FailureValue}', Failure Message '{backImageResult.FailureValue.Message}'.",
                                irn,
                                details.SeqNum,
                                userId.Value,
                                tenantId,
                                requestId);

                            return backImageResult.FailureValue;
                        }

                        var convertedBack = await _imageConverter.ConvertImageToPng(backImageResult.Value);

                        if (convertedBack.IsFailure)
                        {
                            _logger.Information(
                                $"GetGeneralDocumentByIrnAsync failed to convert rear image to PNG for IRN '{{IRN}}', SeqNum '{{SeqNum}}', page '{details.Page}', UserId '{{UserId}}', TenantId '{{TenantId}}', RequestId '{{RequestId}}'. Failure code '{convertedBack.FailureValue}', Failure Message '{convertedBack.FailureValue.Message}'.",
                                irn,
                                details.SeqNum,
                                userId.Value,
                                tenantId,
                                requestId);

                            return convertedBack.FailureValue;
                        }

                        var page = convertedFront.OnSuccess(
                            frontImage => convertedBack.OnSuccess(
                                backImage => GeneralDocumentPage.TwoSided(
                                    details.Page,
                                    convertedFront.Value,
                                    convertedBack.Value)));
                        generalDocumentPages.Add(page.Value);
                    }
                    else
                    {
                        var page = convertedFront.OnSuccess(
                            frontImage => GeneralDocumentPage.OneSided(details.Page, convertedFront.Value));
                        generalDocumentPages.Add(page.Value);
                    }
                }

                return new GeneralDocument(itemGeneral.Value.DocumentName ?? string.Empty, generalDocumentPages);
            }
            catch (Exception e)
            {
                _logger.Error(
                     e,
                     "GetGeneralDocumentByIrnAsync ran into an exception when looking for images associated with IRN '{IRN}' for UserId '{UserId}', TenantId '{TenantId}'. Please contact deployment about this issue. RequestId '{RequestId}'.",
                     irn,
                     userId.Value,
                     tenantId,
                     requestId);

                return new ServiceFailure("The service ran into an exception when attempting to get the image.");
            }
            finally
            {
                monitor?.Stop("WebClientItemService.GetGeneralDocumentByIrnAsync");
            }
        }
    }
}
