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
    public class RemittanceController : Controller
    {
        public const string ControllerName = "remittance";

        private readonly ILogger _logger;
        private readonly IMapper<Remittance, RemittanceDto> _mapper;
        private readonly IRequestDataFactory _requestDataFactory;
        private readonly IItmsItemServiceFactory _itmsItemServiceFactory;
        private readonly IWebClientItemServiceFactory _webClientItemServiceFactory;

        public RemittanceController(
            ILogger logger,
            IMapper<Remittance, RemittanceDto> mapper,
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

            Result<Error, Remittance> remittanceImagesResult;

            if (TargetHost.Itms == targetHostEnum)
            {
                remittanceImagesResult = await RetrieveImagesFromItmsAsync(requestData, irnId, seqNum.Value);
            }
            else
            {
                remittanceImagesResult = await RetrieveImagesFromWebClientAsync(requestData, irnId);
            }

            if (remittanceImagesResult.IsFailure)
            {
                var errorMessage =
                    $"RemittanceController failed to retrieve the images with error '{remittanceImagesResult.FailureValue.Message}'.";
                switch (remittanceImagesResult.FailureValue)
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

            var remittanceDto = _mapper.Map(remittanceImagesResult.Value);

            _logger.Debug("RemittanceController returning images. RequestId '{RequestId}'.", requestData.RequestId);

            return Ok(remittanceDto);
        }

        private async Task<Result<Error, Remittance>> RetrieveImagesFromItmsAsync(IRequestData requestData, IrnId irnId, int seqNum)
        {
            var itmsService = _itmsItemServiceFactory.Create(requestData);

            _logger.Debug(
                "RemittanceController.RetrieveImagesFromItmsAsync retrieving images for remittance with IRN '{IRN}', SeqNum '{SeqNum}'. RequestId '{RequestId}'.",
                irnId.Value,
                seqNum,
                requestData.RequestId);

            return await itmsService.GetRemittanceByIrnAsync(irnId, seqNum, requestData.UserId, requestData.TenantId, DateTime.Now.AddMonths(-26), DateTime.Now);
        }

        private async Task<Result<Error, Remittance>> RetrieveImagesFromWebClientAsync(IRequestData requestData, IrnId irnId)
        {
            var webClientService = _webClientItemServiceFactory.Create(requestData);

            _logger.Debug(
                "RemittanceController.RetrieveImagesFromWebClientAsync retrieving images for remittance with IRN '{IRN}'. RequestId '{RequestId}'.",
                irnId.Value,
                requestData.RequestId);

            return await webClientService.GetRemittanceByIrnAsync(irnId, requestData.UserId, requestData.TenantId);
        }
    }
}
