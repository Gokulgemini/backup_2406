using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RDM.Core;
using RDM.DataTransferObjects.ImageViewerAPI;
using RDM.Maps.ImageViewerAPI;
using RDM.Model.Itms;
using RDM.Models.ImageViewerAPI;
using RDM.Webservice.ImageViewerAPI.Factories;
using Serilog;

namespace RDM.Webservice.ImageViewerAPI.Controllers
{
    [Route(Startup.ApiRoot + ControllerName)]
    public class ChequeController : Controller
    {
        public const string ControllerName = "check";

        private readonly ILogger _logger;
        private readonly IMapper<Cheque, ChequeDto> _mapper;
        private readonly IRequestDataFactory _requestDataFactory;
        private readonly IItmsItemServiceFactory _itmsItemServiceFactory;
        private readonly IWebClientItemServiceFactory _webClientItemServiceFactory;

        public ChequeController(
            ILogger logger,
            IMapper<Cheque, ChequeDto> mapper,
            IRequestDataFactory requestDataFactory,
            IItmsItemServiceFactory itmsItemServiceFactory,
            IWebClientItemServiceFactory webClientItemServiceFactory)
        {
            Contract.Requires<ArgumentNullException>(logger != null, nameof(logger));
            Contract.Requires<ArgumentNullException>(mapper != null, nameof(mapper));
            Contract.Requires<ArgumentNullException>(requestDataFactory != null, nameof(requestDataFactory));
            Contract.Requires<ArgumentNullException>(itmsItemServiceFactory != null, nameof(itmsItemServiceFactory));
            Contract.Requires<ArgumentNullException>(webClientItemServiceFactory != null, nameof(webClientItemServiceFactory));

            _logger = logger;
            _mapper = mapper;
            _requestDataFactory = requestDataFactory;
            _itmsItemServiceFactory = itmsItemServiceFactory;
            _webClientItemServiceFactory = webClientItemServiceFactory;
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

            Result<Error, Cheque> chequeImagesResult;

            if (TargetHost.Itms == targetHostEnum)
            {
                chequeImagesResult = await RetrieveImagesFromItmsAsync(requestData, irnId, seqNum.Value);
            }
            else
            {
                chequeImagesResult = await RetrieveImagesFromWebClientAsync(requestData, irnId);
            }

            if (chequeImagesResult.IsFailure)
            {
                var errorMessage =
                    $"ChequeController failed to retrieve the images with error '{chequeImagesResult.FailureValue.Message}'.";
                switch (chequeImagesResult.FailureValue)
                {
                    case NotFound _:
                    {
                        _logger.Information(errorMessage + " IRN '{IRN}', RequestId '{RequestId}'.", irn, requestData.RequestId);

                        return NotFound();
                    }

                    default:
                    {
                        _logger.Error(errorMessage + " IRN '{IRN}', RequestId '{RequestId}'.", irn, requestData.RequestId);

                        return new StatusCodeResult(500);
                    }
                }
            }

            var chequeDto = _mapper.Map(chequeImagesResult.Value);

            _logger.Debug("ChequeController returning images. IRN '{IRN}', RequestId '{RequestId}'.", irn, requestData.RequestId);

            return Ok(chequeDto);
        }

        private async Task<Result<Error, Cheque>> RetrieveImagesFromItmsAsync(IRequestData requestData, IrnId irnId, int seqNum)
        {
            var itmsService = _itmsItemServiceFactory.Create(requestData);

            _logger.Debug(
                "ChequeController.RetrieveImagesFromItmsAsync retrieving images for cheque with IRN '{IRN}', UserId '{UserId}', SeqNum '{SeqNum}', TenantId '{TenantId}'. RequestId '{RequestId}'.",
                irnId.Value,
                requestData.UserId.Value,
                seqNum,
                requestData.TenantId,
                requestData.RequestId);

            return await itmsService.GetChequeByIrnAsync(irnId, seqNum, requestData.UserId, requestData.TenantId, DateTime.Now.AddMonths(-26), DateTime.Now);
        }

        private async Task<Result<Error, Cheque>> RetrieveImagesFromWebClientAsync(IRequestData requestData, IrnId irnId)
        {
            var webClientService = _webClientItemServiceFactory.Create(requestData);

            _logger.Debug(
                "ChequeController.RetrieveImagesFromWebClientAsync retrieving images for cheque with IRN '{IRN}', UserId '{UserId}', TenantId '{TenantId}'. RequestId '{RequestId}'.",
                irnId.Value,
                requestData.UserId.Value,
                requestData.TenantId,
                requestData.RequestId);

            return await webClientService.GetChequeByIrnAsync(irnId, requestData.UserId, requestData.TenantId);
        }
    }
}
