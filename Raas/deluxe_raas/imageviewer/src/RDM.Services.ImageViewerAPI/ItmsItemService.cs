using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RDM.Client.ImageVault;
using RDM.Core;
using RDM.Legacy.Itms;
using RDM.Model.Itms;
using RDM.Models.ImageViewerAPI;
using Serilog;

namespace RDM.Services.ImageViewerAPI
{
    /// <summary>
    /// The service called on by the ImagesController to perform operations in ITMS host and ImageVaultClient.
    /// </summary>
    /// <seealso cref="IItmsItemService" />
    public class ItmsItemService : ItemService, IItmsItemService
    {
        private readonly IRequestDataAccessor _requestDataAccessor;
        private readonly ILogger _logger;
        private readonly IItmsItemRepository _itemItmsRepository;
        private readonly IImageConverter _imageConverter;

        /// <summary>
        /// Initializes a new instance of the <see cref="ItmsItemService"/> class.
        /// </summary>
        /// <param name="requestDataAccessor">The request data accessor.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="imageVaultClient">The image vault client.</param>
        /// <param name="itemItmsRepository">Access to legacy item level details (itms host)</param>
        /// <param name="imageConverter">Conversion logic to use when converting images to different formats.</param>
        public ItmsItemService(
            IRequestDataAccessor requestDataAccessor,
            ILogger logger,
            IImageVaultClient imageVaultClient,
            IItmsItemRepository itemItmsRepository,
            IImageConverter imageConverter) : base(requestDataAccessor, logger, imageVaultClient, TargetHost.Itms)
        {
            Contract.Requires<ArgumentNullException>(requestDataAccessor != null, nameof(requestDataAccessor));
            Contract.Requires<ArgumentNullException>(logger != null, nameof(logger));
            Contract.Requires<ArgumentNullException>(itemItmsRepository != null, nameof(itemItmsRepository));
            Contract.Requires<ArgumentNullException>(imageConverter != null, nameof(imageConverter));

            _requestDataAccessor = requestDataAccessor;
            _logger = logger;
            _itemItmsRepository = itemItmsRepository;
            _imageConverter = imageConverter;
        }

        public async Task<Result<Error, Cheque>> GetChequeByIrnAsync(IrnId irn, int seqNum, UserId userId, string tenantId, DateTime startInsertTime, DateTime endInsertTime)
        {
            Contract.Requires<ArgumentNullException>(irn != null, "An IRN must be provided.");
            Contract.Requires<ArgumentException>(irn != IrnId.Empty, nameof(irn));
            Contract.Requires<ArgumentNullException>(userId != null, nameof(userId));
            Contract.Requires<ArgumentException>(userId != UserId.InvalidUser, nameof(userId));
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(tenantId), nameof(tenantId));

            var requestId = _requestDataAccessor.RequestId;
            var monitor = _requestDataAccessor.PerformanceMonitor;

            monitor?.Start("ItmsItemService.GetChequeByIrnAsync");

            try
            {
                _logger.Debug(
                    "GetChequeByIrnAsync called with IRN '{IRN}', SeqNum '{SeqNum}', UserId '{UserId}', TenantId '{TenantId}', RequestId '{RequestId}'.",
                    irn,
                    seqNum,
                    userId.Value,
                    tenantId,
                    requestId);

                var imageElementDetailsResult = await _itemItmsRepository.GetImageElementDetailsByIrnAsync(irn, seqNum, startInsertTime, endInsertTime);
                if (!imageElementDetailsResult.HasValue)
                {
                    _logger.Information(
                        "GetChequeByIrnAsync failed to find image for IRN '{IRN}', SeqNum '{SeqNum}', UserId '{UserId}', TenantId '{TenantId}', RequestId '{RequestId}'.",
                        irn,
                        seqNum,
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
                if (convertedFront.IsFailure)
                {
                    _logger.Information(
                        $"GetChequeByIrnAsync failed to convert front image to PNG for IRN '{{IRN}}', SeqNum '{{SeqNum}}', page '{page}', UserId '{{UserId}}', TenantId '{{TenantId}}', RequestId '{{RequestId}}'. Failure code '{convertedFront.FailureValue}', Failure Message '{convertedFront.FailureValue.Message}'.",
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
                            $"GetChequeByIrnAsync failed to get rear image for IRN '{{IRN}}', SeqNum '{{SeqNum}}', page '{page}', UserId '{{UserId}}', TenantId '{{TenantId}}', RequestId '{{RequestId}}'. Failure code '{backImageResult.FailureValue}', Failure Message '{backImageResult.FailureValue.Message}'.",
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
                            $"GetChequeByIrnAsync failed to convert rear image to PNG for IRN '{{IRN}}', SeqNum '{{SeqNum}}', page '{page}', UserId '{{UserId}}', TenantId '{{TenantId}}', RequestId '{{RequestId}}'. Failure code '{convertedBack.FailureValue}', Failure Message '{convertedBack.FailureValue.Message}'.",
                            irn,
                            seqNum,
                            userId.Value,
                            tenantId,
                            requestId);

                        return convertedBack.FailureValue;
                    }

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
                monitor?.Stop("ItmsItemService.GetChequeByIrnAsync");
            }
        }

        public async Task<Result<Error, Remittance>> GetRemittanceByIrnAsync(IrnId irn, int seqNum, UserId userId, string tenantId, DateTime startInsertTime, DateTime endInsertTime)
        {
            Contract.Requires<ArgumentNullException>(irn != null, "An IRN must be provided.");
            Contract.Requires<ArgumentException>(irn != IrnId.Empty, nameof(irn));
            Contract.Requires<ArgumentNullException>(userId != null, nameof(userId));
            Contract.Requires<ArgumentException>(userId != UserId.InvalidUser, nameof(userId));
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(tenantId), nameof(tenantId));

            var requestId = _requestDataAccessor.RequestId;
            var monitor = _requestDataAccessor.PerformanceMonitor;

            monitor?.Start("ItmsItemService.GetRemittanceByIrnAsync");

            try
            {
                _logger.Debug(
                    "GetRemittanceByIrnAsync called with IRN '{IRN}', SeqNum '{SeqNum}', UserId '{UserId}', TenantId '{TenantId}', RequestId '{RequestId}'.",
                    irn,
                    seqNum,
                    userId.Value,
                    tenantId,
                    requestId);

                var imageElementDetailsResult = await _itemItmsRepository.GetImageElementDetailsByIrnAsync(irn, seqNum, startInsertTime, endInsertTime);
                if (!imageElementDetailsResult.HasValue)
                {
                    // We have no reliable way to determine a Virtual remit, so we have to assume if we
                    // can't find images on a remit is must be virtual ¯\_(ツ)_/¯
                    _logger.Information(
                        "GetRemittanceByIrnAsync failed to find image for IRN '{IRN}', SeqNum '{SeqNum}', UserId '{UserId}', TenantId '{TenantId}', RequestId '{RequestId}'.",
                        irn,
                        seqNum,
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
                monitor?.Stop("ItmsItemService.GetRemittanceByIrnAsync");
            }
        }

        public async Task<Result<Error, GeneralDocument>> GetGeneralDocumentByIrnAsync(IrnId irn, int seqNum, UserId userId, string tenantId, DateTime startInsertTime, DateTime endInsertTime)
        {
            Contract.Requires<ArgumentNullException>(irn != null, "An IRN must be provided.");
            Contract.Requires<ArgumentException>(irn != IrnId.Empty, nameof(irn));
            Contract.Requires<ArgumentNullException>(userId != null, nameof(userId));
            Contract.Requires<ArgumentException>(userId != UserId.InvalidUser, nameof(userId));
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(tenantId), nameof(tenantId));

            var requestId = _requestDataAccessor.RequestId;
            var monitor = _requestDataAccessor.PerformanceMonitor;

            monitor?.Start("ItmsItemService.GetGeneralDocumentByIrnAsync");

            try
            {
                _logger.Debug(
                    "GetGeneralDocumentByIrnAsync called with IRN '{IRN}', SeqNum '{SeqNum}', UserId '{UserId}', TenantId '{TenantId}', RequestId '{RequestId}'.",
                    irn,
                    seqNum,
                    userId.Value,
                    tenantId,
                    requestId);

                var imageElementDetailsResult = await _itemItmsRepository.GetImageElementDetailsByIrnAsync(irn, seqNum, startInsertTime, endInsertTime);
                if (!imageElementDetailsResult.HasValue)
                {
                    _logger.Information(
                        "GetGeneralDocumentByIrnAsync failed to find image for IRN '{IRN}', SeqNum '{SeqNum}', UserId '{UserId}', TenantId '{TenantId}', RequestId '{RequestId}'.",
                        irn,
                        seqNum,
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
                                          .Select(group => new { Page = group.Key, Surfaces = group.Select(d => d.Surface) })
                                          .ToList();

                var generalDocumentPages = new List<GeneralDocumentPage>();
                foreach (var details in imageElementDetails)
                {
                    var frontImageResult = await GetFrontImageFromVaultAsync(irn, seqNum, details.Page, userId, tenantId);
                    if (frontImageResult.IsFailure)
                    {
                        _logger.Information(
                            $"GetGeneralDocumentByIrnAsync failed to get front image for IRN '{{IRN}}', SeqNum '{{SeqNum}}', page '{details.Page}', UserId '{{UserId}}', TenantId '{{TenantId}}', RequestId '{{RequestId}}'. Failure code '{frontImageResult.FailureValue}', Failure Message '{frontImageResult.FailureValue.Message}'.",
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
                            $"GetGeneralDocumentByIrnAsync failed to convert front image to PNG for IRN '{{IRN}}', SeqNum '{{SeqNum}}', page '{details.Page}', UserId '{{UserId}}', TenantId '{{TenantId}}', RequestId '{{RequestId}}'. Failure code '{convertedFront.FailureValue}', Failure Message '{convertedFront.FailureValue.Message}'.",
                            irn,
                            seqNum,
                            userId.Value,
                            tenantId,
                            requestId);

                        return convertedFront.FailureValue;
                    }

                    if (details.Surfaces.Any(surface => surface == ImageSurface.Back))
                    {
                        var backImageResult = await GetBackImageFromVaultAsync(irn, seqNum, details.Page, userId, tenantId);
                        if (backImageResult.IsFailure)
                        {
                            _logger.Information(
                                $"GetGeneralDocumentByIrnAsync failed to get rear image for IRN '{{IRN}}', SeqNum '{{SeqNum}}', page '{details.Page}', UserId '{{UserId}}', TenantId '{{TenantId}}', RequestId '{{RequestId}}'. Failure code '{backImageResult.FailureValue}', Failure Message '{backImageResult.FailureValue.Message}'.",
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
                                $"GetGeneralDocumentByIrnAsync failed to convert rear image to PNG for IRN '{{IRN}}', SeqNum '{{SeqNum}}', page '{details.Page}', UserId '{{UserId}}', TenantId '{{TenantId}}', RequestId '{{RequestId}}'. Failure code '{convertedBack.FailureValue}', Failure Message '{convertedBack.FailureValue.Message}'.",
                                irn,
                                seqNum,
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

                var itemGeneral = await _itemItmsRepository.GetGeneralDocumentDetailsByIrnAsync(irn, seqNum);

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
                monitor?.Stop("ItmsItemService.GetGeneralDocumentByIrnAsync");
            }
        }
    }
}