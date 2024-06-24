using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RDM.Core;
using RDM.DataTransferObjects.ImageViewerAPI;
using RDM.Maps.ImageViewerAPI;
using RDM.Model.Itms;
using RDM.Models.ImageViewerAPI;
using RDM.Services.ImageViewerAPI;
using RDM.Webservice.ImageViewerAPI.Dtos;
using RDM.Webservice.ImageViewerAPI.Factories;
using Serilog;

namespace RDM.Webservice.ImageViewerAPI.Controllers
{
    [Route(Startup.ApiRoot + ControllerName)]
    public class GeneralDocumentController : Controller
    {
        public const string ControllerName = "generaldocument";

        private readonly ILogger _logger;
        private readonly IMapper<GeneralDocument, GeneralDocumentDto> _mapper;
        private readonly IRequestDataFactory _requestDataFactory;
        private readonly IItmsItemServiceFactory _itmsItemServiceFactory;
        private readonly IWebClientItemServiceFactory _webClientItemServiceFactory;
        private readonly IPermissionService _permissionService;

        public GeneralDocumentController(
            ILogger logger,
            IMapper<GeneralDocument, GeneralDocumentDto> mapper,
            IRequestDataFactory requestDataFactory,
            IItmsItemServiceFactory itmsItemServiceFactory,
            IWebClientItemServiceFactory webClientItemServiceFactory,
            IPermissionService permissionService)
        {
            Contract.Requires<ArgumentNullException>(logger != null, nameof(logger));
            Contract.Requires<ArgumentNullException>(mapper != null, nameof(mapper));
            Contract.Requires<ArgumentNullException>(requestDataFactory != null, nameof(requestDataFactory));
            Contract.Requires<ArgumentNullException>(itmsItemServiceFactory != null, nameof(itmsItemServiceFactory));
            Contract.Requires<ArgumentNullException>(webClientItemServiceFactory != null, nameof(webClientItemServiceFactory));
            Contract.Requires<ArgumentNullException>(permissionService != null, nameof(permissionService));

            _logger = logger;
            _mapper = mapper;
            _requestDataFactory = requestDataFactory;
            _itmsItemServiceFactory = itmsItemServiceFactory;
            _webClientItemServiceFactory = webClientItemServiceFactory;
            _permissionService = permissionService;
        }

        [Route("{irn}")]
        [HttpGet]
        public async Task<IActionResult> RetrieveImages(string irn, string targetHost, int? seqNum)
        {
            if (!IrnId.IsValid(irn) ||
                !Enum.TryParse(targetHost, out TargetHost targetHostEnum) ||
                (TargetHost.Itms == targetHostEnum && seqNum == null))
            {
                return BadRequest();
            }

            var irnId = new IrnId(irn);
            var requestData = _requestDataFactory.Create();

            var genDocImagesResult = TargetHost.Itms == targetHostEnum
                ? await RetrieveImagesFromItmsAsync(requestData, irnId, seqNum.Value)
                : await RetrieveImagesFromWebClientAsync(requestData, irnId);

            if (genDocImagesResult.IsFailure)
            {
                var errorMessage =
                    $"GeneralDocumentController failed to retrieve the images with error '{genDocImagesResult.FailureValue.Message}'.";
                switch (genDocImagesResult.FailureValue)
                {
                    case NotFound _:
                    {
                        _logger.Debug(errorMessage + " RequestId '{RequestId}'.", requestData.RequestId);

                        return NotFound();
                    }

                    default:
                    {
                        _logger.Error(errorMessage + " RequestId '{RequestId}'.", requestData.RequestId);

                        return new StatusCodeResult(500);
                    }
                }
            }

            var genDocDto = _mapper.Map(genDocImagesResult.Value);

            _logger.Debug("GeneralDocumentController returning images. RequestId '{RequestId}'.", requestData.RequestId);

            return Ok(genDocDto);
        }

        [Route("{irn}")]
        [HttpPut]
        public async Task<IActionResult> UpdateDocumentName(string irn, [FromBody] UpdateDocumentNameDto updateDocumentName)
        {
            if (!IrnId.IsValid(irn) ||
                updateDocumentName?.DocumentName == null ||
                !Enum.TryParse(updateDocumentName.Module, true, out Module moduleEnum))
            {
                return BadRequest();
            }

            updateDocumentName.DocumentName = updateDocumentName.DocumentName.Trim();
            if (updateDocumentName.DocumentName.Length > 50)
            {
                return BadRequest();
            }

            var requestData = _requestDataFactory.Create();
            if (!(await _permissionService.CanUpdateGeneralDocumentName(requestData.UserId, requestData.AuthenticationDateTimeUtc)))
            {
                return new StatusCodeResult(403); // Return Forbid() does not work for some reason
            }

            var irnId = new IrnId(irn);
            var webClientService = _webClientItemServiceFactory.Create(requestData);

            _logger.Debug(
                "GeneralDocumentController updating document name with IRN '{IRN}', DocumentName '{DocumentName}', Module '{Module}'. RequestId '{RequestId}'.",
                irn,
                updateDocumentName.DocumentName,
                updateDocumentName.Module,
                requestData.RequestId);

            var genDocResult = await webClientService.UpdateDocumentNameByIrnAsync(
                irnId,
                updateDocumentName.DocumentName,
                requestData.UserId,
                moduleEnum);

            if (genDocResult.IsFailure)
            {
                var errorMessage = $"GeneralDocumentController failed to update document name with error '{genDocResult.FailureValue.Message}'.";

                switch (genDocResult.FailureValue)
                {
                    case NotFound _:
                    {
                        _logger.Debug(errorMessage + " RequestId '{RequestId}'.", requestData.RequestId);

                        return NotFound();
                    }

                    default:
                    {
                        _logger.Error(errorMessage + " RequestId '{RequestId}'.", requestData.RequestId);

                        return new StatusCodeResult(500);
                    }
                }
            }

            _logger.Debug("GeneralDocumentController returning success after updating document name. RequestId '{RequestId}'.", requestData.RequestId);

            return Ok();
        }

        private async Task<Result<Error, GeneralDocument>> RetrieveImagesFromItmsAsync(IRequestData requestData, IrnId irnId, int seqNum)
        {
            var itmsService = _itmsItemServiceFactory.Create(requestData);

            _logger.Debug(
                "GeneralDocumentController.RetrieveImagesFromItmsAsync retrieving images for general document with IRN '{IRN}', SeqNum '{SeqNum}'. RequestId '{RequestId}'.",
                irnId.Value,
                seqNum,
                requestData.RequestId);

            return await itmsService.GetGeneralDocumentByIrnAsync(irnId, seqNum, requestData.UserId, requestData.TenantId, DateTime.Now.AddMonths(-26), DateTime.Now);
        }

        private async Task<Result<Error, GeneralDocument>> RetrieveImagesFromWebClientAsync(IRequestData requestData, IrnId irnId)
        {
            var webClientService = _webClientItemServiceFactory.Create(requestData);

            _logger.Debug(
                "GeneralDocumentController.RetrieveImagesFromWebClientAsync retrieving images for general document with IRN '{IRN}'. RequestId '{RequestId}'.",
                irnId.Value,
                requestData.RequestId);

            return await webClientService.GetGeneralDocumentByIrnAsync(irnId, requestData.UserId, requestData.TenantId);
        }
    }
}
