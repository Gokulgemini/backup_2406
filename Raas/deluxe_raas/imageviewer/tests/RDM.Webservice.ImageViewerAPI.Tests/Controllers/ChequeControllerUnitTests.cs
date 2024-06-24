using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using RDM.Core;
using RDM.DataTransferObjects.ImageViewerAPI;
using RDM.Maps.ImageViewerAPI;
using RDM.Model.Itms;
using RDM.Models.ImageViewerAPI;
using RDM.Services.ImageViewerAPI;
using RDM.Webservice.ImageViewerAPI.Controllers;
using RDM.Webservice.ImageViewerAPI.Factories;
using Serilog;
using Xunit;

namespace RDM.Webservice.ImageViewerAPI.Tests.Controllers
{
    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Test files are not styled.")]
    [SuppressMessage("StyleCop.CSharp.LayoutRules", "*", Justification = "Test files are not styled.")]
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "*", Justification = "Test files are not styled.")]
    [SuppressMessage("StyleCop.CSharp.OrderingRules", "*", Justification = "Test files are not styled.")]
    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "*", Justification = "Test files are not styled.")]
    [SuppressMessage("StyleCop.CSharp.SpacingRules", "*", Justification = "Test files are not styled.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "Test files are not styled.")]
    public class ChequeControllerUnitTests
    {
        private readonly ILogger _logger;
        private readonly IMapper<Cheque, ChequeDto> _mapper;
        private readonly IRequestData _requestData;
        private readonly IRequestDataFactory _requestDataFactory;
        private readonly IItmsItemService _itmsService;
        private readonly IItmsItemServiceFactory _itmstServiceFactory;
        private readonly IWebClientItemService _webClientService;
        private readonly IWebClientItemServiceFactory _webcClientServiceFactory;
        private readonly ChequeController _controller;

        public ChequeControllerUnitTests()
        {
            _logger = Substitute.For<ILogger>();
            _mapper = Substitute.For<IMapper<Cheque, ChequeDto>>();
            _requestData = Substitute.For<IRequestData>();
            _requestData.RequestId.Returns(new RequestIdentifier("1"));
            _requestData.UserId.Returns(new UserId("1"));
            _requestData.TenantId.Returns("Default");
            _requestDataFactory = Substitute.For<IRequestDataFactory>();
            _requestDataFactory.Create().Returns(_requestData);

            _itmsService = Substitute.For<IItmsItemService>();
            _itmstServiceFactory = Substitute.For<IItmsItemServiceFactory>();
            _itmstServiceFactory.Create(_requestData).Returns(_itmsService);

            _webClientService = Substitute.For<IWebClientItemService>();
            _webcClientServiceFactory = Substitute.For<IWebClientItemServiceFactory>();
            _webcClientServiceFactory.Create(_requestData).Returns(_webClientService);

            _controller = new ChequeController(_logger, _mapper, _requestDataFactory, _itmstServiceFactory, _webcClientServiceFactory);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async void RetrieveImages_ItmsItemServiceReturnsSuccess_Returns200()
        {
            // Arrange
            var irn = new IrnId("testirn");
            var seqNum = 1;

            SetUp_ItmsItemService_Cheque(Cheque);
            SetUp_Mapper_Map(new ChequeDto());

            // Act
            var result = await _controller.RetrieveImages(irn.ToString(), TargetHost.Itms.ToString(), seqNum);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async void RetrieveImages_ItmsItemServiceFromItmsReturnsNotFound_ReturnsNotFound()
        {
            // Arrange
            var irn = new IrnId("testirn");
            var seqNum = 1;

            SetUp_ItmsItemService_Cheque(new NotFound());

            // Act
            var result = await _controller.RetrieveImages(irn.ToString(), TargetHost.Itms.ToString(), seqNum);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async void RetrieveImages_ItmsItemServiceFromItmsReturnsError_Returns500()
        {
            // Arrange
            var irn = new IrnId("testirn");
            var seqNum = 1;

            SetUp_ItmsItemService_Cheque(new Error("Bad"));

            // Act
            var result = await _controller.RetrieveImages(irn.ToString(), TargetHost.Itms.ToString(), seqNum);

            // Assert
            Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(500, ((StatusCodeResult)result).StatusCode);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async void RetrieveImages_WebClientServiceReturnsSuccess_Returns200()
        {
            // Arrange
            var irn = new IrnId("testirn");

            SetUp_WebClientService_Cheque(Cheque);
            SetUp_Mapper_Map(new ChequeDto());

            // Act
            var result = await _controller.RetrieveImages(irn.ToString(), TargetHost.WebClient.ToString(), null);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async void RetrieveImages_WebClientServiceReturnsNotFound_ReturnsNotFound()
        {
            // Arrange
            var irn = new IrnId("testirn");

            SetUp_WebClientService_Cheque(new NotFound());

            // Act
            var result = await _controller.RetrieveImages(irn.ToString(), TargetHost.WebClient.ToString(), null);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async void RetrieveImages_WebClientServiceReturnsError_Returns500()
        {
            // Arrange
            var irn = new IrnId("testirn");

            SetUp_WebClientService_Cheque(new Error("Bad"));

            // Act
            var result = await _controller.RetrieveImages(irn.ToString(), TargetHost.WebClient.ToString(), null);

            // Assert
            Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(500, ((StatusCodeResult)result).StatusCode);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async void RetrieveImages_InvalidIrn_ReturnsBadRequest()
        {
            // Arrange

            // Act
            var result = await _controller.RetrieveImages("12345678901234567890123456", TargetHost.Itms.ToString(), 1);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async void RetrieveImages_InvalidTargetHost_ReturnsBadRequest()
        {
            // Arrange

            // Act
            var result = await _controller.RetrieveImages("1234567890123456789012345", "BadTarget", 1);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        private Cheque Cheque =>
            Cheque.TwoSided(
                new Image(new byte[] { 1, 2, 3, 4 }, "image/png", 10, 20),
                new Image(new byte[] { 5, 6, 7, 8 }, "image/png", 30, 40));

        private void SetUp_ItmsItemService_Cheque(Result<Error, Cheque> result)
        {
            _itmsService.GetChequeByIrnAsync(Arg.Any<IrnId>(), Arg.Any<int>(), Arg.Any<UserId>(), Arg.Any<string>(), Arg.Any<DateTime>(), Arg.Any<DateTime>())
                                  .Returns(result);
        }

        private void SetUp_WebClientService_Cheque(Result<Error, Cheque> result)
        {
            _webClientService.GetChequeByIrnAsync(Arg.Any<IrnId>(), Arg.Any<UserId>(), Arg.Any<string>())
                .Returns(result);
        }

        private void SetUp_Mapper_Map(ChequeDto dto)
        {
            _mapper.Map(Arg.Any<Cheque>()).Returns(dto);
        }
    }
}
