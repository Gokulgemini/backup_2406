using System;
using System.Collections.Generic;
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
using RDM.Webservice.ImageViewerAPI.Dtos;
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
    public class GeneralDocumentControllerUnitTests
    {
        private readonly ILogger _logger;
        private readonly IMapper<GeneralDocument, GeneralDocumentDto> _mapper;
        private readonly IRequestData _requestData;
        private readonly IRequestDataFactory _requestDataFactory;
        private readonly IItmsItemService _itmsService;
        private readonly IItmsItemServiceFactory _itmsItemServiceFactory;
        private readonly IWebClientItemService _webClientService;
        private readonly IWebClientItemServiceFactory _webClientItemServiceFactory;
        private readonly GeneralDocumentController _controller;
        private readonly IPermissionService _permissionService;

        public GeneralDocumentControllerUnitTests()
        {
            _permissionService = Substitute.For<IPermissionService>();
            _logger = Substitute.For<ILogger>();
            _mapper = Substitute.For<IMapper<GeneralDocument, GeneralDocumentDto>>();
            _requestData = Substitute.For<IRequestData>();
            _requestData.RequestId.Returns(new RequestIdentifier("1"));
            _requestDataFactory = Substitute.For<IRequestDataFactory>();
            _requestDataFactory.Create().Returns(_requestData);

            _itmsService = Substitute.For<IItmsItemService>();
            _itmsItemServiceFactory = Substitute.For<IItmsItemServiceFactory>();
            _itmsItemServiceFactory.Create(_requestData).Returns(_itmsService);

            _webClientService = Substitute.For<IWebClientItemService>();
            _webClientItemServiceFactory = Substitute.For<IWebClientItemServiceFactory>();
            _webClientItemServiceFactory.Create(_requestData).Returns(_webClientService);

            _controller = new GeneralDocumentController(
                _logger,
                _mapper,
                _requestDataFactory,
                _itmsItemServiceFactory,
                _webClientItemServiceFactory,
                _permissionService);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async void RetrieveImages_ItmsItemServiceReturnsSuccess_Returns200()
        {
            // Arrange
            var irn = new IrnId("testirn");
            var seqNum = 1;

            SetUp_ItmsItemService_GeneralDocument(GeneralDocument);
            SetUp_Mapper_Map(new GeneralDocumentDto());

            // Act
            var result = await _controller.RetrieveImages(irn.ToString(), TargetHost.Itms.ToString(), seqNum);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async void RetrieveImages_ItmsItemServiceReturnsNotFound_ReturnsNotFound()
        {
            // Arrange
            var irn = new IrnId("testirn");
            var seqNum = 1;

            SetUp_ItmsItemService_GeneralDocument(new NotFound());

            // Act
            var result = await _controller.RetrieveImages(irn.ToString(), TargetHost.Itms.ToString(), seqNum);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async void RetrieveImages_ItmsItemServiceReturnsError_Returns500()
        {
            // Arrange
            var irn = new IrnId("testirn");
            var seqNum = 1;

            SetUp_ItmsItemService_GeneralDocument(new Error("Bad"));

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

            SetUp_WebClientService_GeneralDocument(GeneralDocument);
            SetUp_Mapper_Map(new GeneralDocumentDto());

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

            SetUp_WebClientService_GeneralDocument(new NotFound());

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

            SetUp_WebClientService_GeneralDocument(new Error("Bad"));

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

        [Fact]
        [Trait("Category", "Unit")]
        public async void UpdateDocumentName_NoPermission_Returns403()
        {
            // Arrange
            var irn = new IrnId("testirn");
            var updateDocumentNameDto = new UpdateDocumentNameDto { DocumentName = "Doc Name", Module = "Scan" };
            _permissionService.CanUpdateGeneralDocumentName(_requestData.UserId, new DateTime()).Returns(false);

            SetUp_WebClientService_UpdateGeneralDocumentName(new Error("Bad"));

            // Act
            var result = await _controller.UpdateDocumentName(irn.ToString(), updateDocumentNameDto);

            // Assert
            Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(403, ((StatusCodeResult)result).StatusCode);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async void UpdateDocumentName_InvalidIrn_ReturnsBadRequest()
        {
            // Arrange
            var updateDocumentNameDto = new UpdateDocumentNameDto();
            _permissionService.CanUpdateGeneralDocumentName(_requestData.UserId, new DateTime()).Returns(true);

            // Act
            var result = await _controller.UpdateDocumentName("12345678901234567890123456", updateDocumentNameDto);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async void UpdateDocumentName_DocumentNameGreaterThan50Characters_ReturnsBadRequest()
        {
            // Arrange
            var irn = new IrnId("testirn");
            var updateDocumentNameDto = new UpdateDocumentNameDto
            {
                DocumentName = "Document Document Document Document Document Document Document Document Document"
            };
            _permissionService.CanUpdateGeneralDocumentName(_requestData.UserId, new DateTime()).Returns(true);

            // Act
            var result = await _controller.UpdateDocumentName(irn.Value, updateDocumentNameDto);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async void UpdateDocumentName_DocumentNameIsNull_ReturnsBadRequest()
        {
            // Arrange
            var irn = new IrnId("testirn");
            var updateDocumentNameDto = new UpdateDocumentNameDto { DocumentName = null };
            _permissionService.CanUpdateGeneralDocumentName(_requestData.UserId, new DateTime()).Returns(true);

            // Act
            var result = await _controller.UpdateDocumentName(irn.Value, updateDocumentNameDto);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async void UpdateDocumentName_ModuleIsNull_ReturnsBadRequest()
        {
            // Arrange
            var irn = new IrnId("testirn");
            var updateDocumentNameDto = new UpdateDocumentNameDto { DocumentName = "Doc Name", Module = null };
            _permissionService.CanUpdateGeneralDocumentName(_requestData.UserId, new DateTime()).Returns(true);

            // Act
            var result = await _controller.UpdateDocumentName(irn.Value, updateDocumentNameDto);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async void UpdateDocumentName_ModuleIsEmpty_ReturnsBadRequest()
        {
            // Arrange
            var irn = new IrnId("testirn");
            var updateDocumentNameDto = new UpdateDocumentNameDto { DocumentName = "Doc Name", Module = string.Empty };
            _permissionService.CanUpdateGeneralDocumentName(_requestData.UserId, new DateTime()).Returns(true);

            // Act
            var result = await _controller.UpdateDocumentName(irn.Value, updateDocumentNameDto);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async void UpdateDocumentName_ModuleHasOnlyWhiteSpace_ReturnsBadRequest()
        {
            // Arrange
            var irn = new IrnId("testirn");
            var updateDocumentNameDto = new UpdateDocumentNameDto { DocumentName = "Doc Name", Module = "     " };
            _permissionService.CanUpdateGeneralDocumentName(_requestData.UserId, new DateTime()).Returns(true);

            // Act
            var result = await _controller.UpdateDocumentName(irn.Value, updateDocumentNameDto);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async void UpdateDocumentName_InvalidModule_ReturnsBadRequest()
        {
            // Arrange
            var irn = new IrnId("testirn");
            var updateDocumentNameDto = new UpdateDocumentNameDto { DocumentName = "Doc Name", Module = "Invalid" };
            _permissionService.CanUpdateGeneralDocumentName(_requestData.UserId, new DateTime()).Returns(true);

            // Act
            var result = await _controller.UpdateDocumentName(irn.Value, updateDocumentNameDto);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async void UpdateDocumentName_GeneralDocumentServiceReturnsSuccess_Returns200()
        {
            // Arrange
            var irn = new IrnId("testirn");
            var updateDocumentNameDto = new UpdateDocumentNameDto { DocumentName = "Doc Name", Module = "Scan" };
            _requestData.UserId = new UserId(123);
            _permissionService.CanUpdateGeneralDocumentName(_requestData.UserId, new DateTime()).Returns(true);

            SetUp_WebClientService_UpdateGeneralDocumentName(true);

            // Act
            var result = await _controller.UpdateDocumentName(irn.ToString(), updateDocumentNameDto);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async void UpdateDocumentName_GeneralDocumentServiceReturnsNotFound_ReturnsNotFound()
        {
            // Arrange
            var irn = new IrnId("testirn");
            var updateDocumentNameDto = new UpdateDocumentNameDto { DocumentName = "Doc Name", Module = "Scan" };
            _requestData.UserId = new UserId(123);
            _permissionService.CanUpdateGeneralDocumentName(_requestData.UserId, new DateTime()).Returns(true);

            SetUp_WebClientService_UpdateGeneralDocumentName(new NotFound());

            // Act
            var result = await _controller.UpdateDocumentName(irn.ToString(), updateDocumentNameDto);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async void UpdateDocumentName_GeneralDocumentServiceReturnsError_Returns500()
        {
            // Arrange
            var irn = new IrnId("testirn");
            var updateDocumentNameDto = new UpdateDocumentNameDto { DocumentName = "Doc Name", Module = "Scan" };
            _requestData.UserId = new UserId(123);
            _permissionService.CanUpdateGeneralDocumentName(_requestData.UserId, new DateTime()).Returns(true);

            SetUp_WebClientService_UpdateGeneralDocumentName(new Error("Bad"));

            // Act
            var result = await _controller.UpdateDocumentName(irn.ToString(), updateDocumentNameDto);

            // Assert
            Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(500, ((StatusCodeResult)result).StatusCode);
        }

        #region Substitution Helpers

        private GeneralDocument GeneralDocument =>
            new GeneralDocument(
                "testDoc",
                new List<GeneralDocumentPage>
                {
                    new GeneralDocumentPage(
                        0,
                        new Image(new byte[] { 1, 2, 3, 4 }, "image/png", 10, 20),
                        new Image(new byte[] { 5, 6, 7, 8 }, "image/png", 30, 40))
                });

        private void SetUp_ItmsItemService_GeneralDocument(Result<Error, GeneralDocument> result)
        {
            _itmsService.GetGeneralDocumentByIrnAsync(Arg.Any<IrnId>(), Arg.Any<int>(), Arg.Any<UserId>(), Arg.Any<string>(), Arg.Any<DateTime>(), Arg.Any<DateTime>())
                        .Returns(result);
        }

        private void SetUp_WebClientService_GeneralDocument(Result<Error, GeneralDocument> result)
        {
            _webClientService.GetGeneralDocumentByIrnAsync(Arg.Any<IrnId>(), Arg.Any<UserId>(), Arg.Any<string>())
                             .Returns(result);
        }

        private void SetUp_WebClientService_UpdateGeneralDocumentName(Result<Error, bool> result)
        {
            _webClientService.UpdateDocumentNameByIrnAsync(
                                 Arg.Any<IrnId>(),
                                 Arg.Any<string>(),
                                 Arg.Any<UserId>(),
                                 Arg.Any<Module>())
                             .Returns(result);
        }

        private void SetUp_Mapper_Map(GeneralDocumentDto dto)
        {
            _mapper.Map(Arg.Any<GeneralDocument>()).Returns(dto);
        }

        #endregion
    }
}