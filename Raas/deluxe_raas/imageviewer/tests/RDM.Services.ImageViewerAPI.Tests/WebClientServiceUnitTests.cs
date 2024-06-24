using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Grpc.Core;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using RDM.Client.ImageVault;
using RDM.Client.Tracker;
using RDM.Core;
using RDM.Legacy.Share;
using RDM.Legacy.WebClientDb;
using RDM.Messaging.ImageVault;
using RDM.Model.Itms;
using RDM.Models.ImageViewerAPI;
using Serilog;
using Xunit;
using Module = RDM.Models.ImageViewerAPI.Module;

namespace RDM.Services.ImageViewerAPI.Tests
{
    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Test files are not styled.")]
    [SuppressMessage("StyleCop.CSharp.LayoutRules", "*", Justification = "Test files are not styled.")]
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "*", Justification = "Test files are not styled.")]
    [SuppressMessage("StyleCop.CSharp.OrderingRules", "*", Justification = "Test files are not styled.")]
    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "*", Justification = "Test files are not styled.")]
    [SuppressMessage("StyleCop.CSharp.SpacingRules", "*", Justification = "Test files are not styled.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "Test files are not styled.")]
    public class WebClientServiceUnitTests
    {
        private readonly IRequestDataAccessor _requestDataAccessor;
        private readonly ILogger _logger;
        private readonly IImageVaultClient _imageVaultClient;
        private readonly IWebClientItemRepository _itemRepository;
        private readonly IWebClientItemService _service;
        private readonly IImageConverter _imageConverter;
        private readonly ITrackerClient _trackerClient;

        public WebClientServiceUnitTests()
        {
            _requestDataAccessor = Substitute.For<IRequestDataAccessor>();
            _requestDataAccessor.RequestId.Returns(new RequestIdentifier("1"));
            _logger = Substitute.For<ILogger>();
            _imageVaultClient = Substitute.For<IImageVaultClient>();
            _itemRepository = Substitute.For<IWebClientItemRepository>();
            _imageConverter = Substitute.For<IImageConverter>();
            _trackerClient = Substitute.For<ITrackerClient>();

            _service = new WebClientItemService(
                _requestDataAccessor,
                _logger,
                _imageVaultClient,
                _itemRepository,
                _imageConverter,
                _trackerClient);
        }

        public static IEnumerable<object[]> GetImagesByIrnAsyncInvalidArguments()
        {
            var irn = new IrnId("EYMG6NTRKU80C04SW4CWWGKW8");
            var userId = new UserId(123);
            var tenantId = "Default1";

            return new List<object[]>
            {
                new object[] { IrnId.Empty, userId, tenantId },
                new object[] { null, userId, tenantId },
                new object[] { irn, null, tenantId },
                new object[] { irn, UserId.InvalidUser, tenantId },
                new object[] { irn, userId, null },
                new object[] { irn, userId, "    " }
            };
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async void GetChequeByIrnAsync_FrontAndBackImagesFound_ReturnsTwoSidedCheque()
        {
            // Arrange
            var tenantId = "Default";
            var irn = new IrnId("testirn");
            var userId = new UserId(12345);

            var expectedCheque = Cheque.TwoSided(FrontPngImage, BackPngImage);

            SetUp_ItemRepository_ImageElementDetailsAsync(_twoSidedItemDetails);
            SetUp_ImageVault_FrontImageAsync(FrontPngImage);
            SetUp_ImageVault_BackImageAsync(BackPngImage);
            SetUp_ImageConverter_ConvertToPng(FrontPngImage, FrontPngImage);
            SetUp_ImageConverter_ConvertToPng(BackPngImage, BackPngImage);

            // Act
            var result = await _service.GetChequeByIrnAsync(irn, userId, tenantId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(expectedCheque, result.Value);
            await _imageConverter.Received(2).ConvertImageToPng(Arg.Any<Image>());
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async void GetRemittanceByIrnAsync_FrontImagesFound_ReturnsOneSidedCheque()
        {
            // Arrange
            var tenantId = "Default";
            var irn = new IrnId("testirn");
            var userId = new UserId(12345);

            var expectedCheque = Cheque.OneSided(FrontPngImage);

            SetUp_ItemRepository_ImageElementDetailsAsync(_oneSidedItemDetails);
            SetUp_ImageVault_FrontImageAsync(FrontPngImage);
            SetUp_ImageConverter_ConvertToPng(FrontPngImage, FrontPngImage);

            // Act
            var result = await _service.GetChequeByIrnAsync(irn, userId, tenantId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(expectedCheque, result.Value);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async void GetChequeByIrnAsync_ImageDetailsMissing_ReturnsNotFound()
        {
            // Arrange
            var tenantId = "Default";
            var irn = new IrnId("testirn");
            var userId = new UserId(12345);

            SetUp_ItemRepository_ImageElementDetailsAsync(_emptyDetails);

            // Act
            var result = await _service.GetChequeByIrnAsync(irn, userId, tenantId);

            // Assert
            Assert.True(result.IsFailure);
            Assert.IsType<NotFound>(result.FailureValue);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async void GetChequeByIrnAsync_FrontImageMissing_ReturnsNotFound()
        {
            // Arrange
            var tenantId = "Default";
            var irn = new IrnId("testirn");
            var userId = new UserId(12345);

            SetUp_ItemRepository_ImageElementDetailsAsync(_twoSidedItemDetails);
            SetUp_ImageVault_FrontImageMissingAsync();
            SetUp_ImageVault_BackImageAsync(BackPngImage);
            SetUp_ImageConverter_ConvertToPng(BackPngImage, BackPngImage);

            // Act
            var result = await _service.GetChequeByIrnAsync(irn, userId, tenantId);

            // Assert
            Assert.True(result.IsFailure);
            Assert.IsType<NotFound>(result.FailureValue);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async void GetChequeByIrnAsync_BackImageMissing_ReturnsNotFound()
        {
            // Arrange
            var tenantId = "Default";
            var irn = new IrnId("testirn");
            var userId = new UserId(12345);

            SetUp_ItemRepository_ImageElementDetailsAsync(_twoSidedItemDetails);
            SetUp_ImageVault_FrontImageAsync(FrontPngImage);
            SetUp_ImageConverter_ConvertToPng(FrontPngImage, FrontPngImage);
            SetUp_ImageVault_BackImageMissingAsync();

            // Act
            var result = await _service.GetChequeByIrnAsync(irn, userId, tenantId);

            // Assert
            Assert.True(result.IsFailure);
            Assert.IsType<NotFound>(result.FailureValue);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async void GetChequeByIrnAsync_ErrorConvertingImage_ReturnsError()
        {
            // Arrange
            var tenantId = "Default";
            var irn = new IrnId("testirn");
            var userId = new UserId(12345);

            SetUp_ItemRepository_ImageElementDetailsAsync(_twoSidedItemDetails);
            SetUp_ImageVault_FrontImageAsync(FrontPngImage);
            SetUp_ImageVault_BackImageAsync(BackPngImage);
            SetUp_ImageConverter_ConvertToPng(FrontPngImage, new Error("Bad stuff happened in that image converter."));
            SetUp_ImageConverter_ConvertToPng(BackPngImage, BackPngImage);

            // Act
            var result = await _service.GetChequeByIrnAsync(irn, userId, tenantId);

            // Assert
            Assert.True(result.IsFailure);
            Assert.IsType<Error>(result.FailureValue);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async void GetChequeByIrnAsync_ExceptionDuringExecution_ReturnsServiceFailure()
        {
            // Arrange
            var tenantId = "Default";
            var irn = new IrnId("testirn");
            var userId = new UserId(12345);

            _itemRepository.GetImageElementDetailsByIrn(Arg.Any<IrnId>()).Throws(new Exception("Service failure."));

            // Act
            var result = await _service.GetChequeByIrnAsync(irn, userId, tenantId);

            // Assert
            Assert.True(result.IsFailure);
            Assert.IsType<ServiceFailure>(result.FailureValue);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async void GetChequeByIrnAsync_SeqNumberMissing_ReturnsNotFound()
        {
            // Arrange
            var tenantId = "Default";
            var irn = new IrnId("testirn");
            var userId = new UserId(12345);

            SetUp_ItemRepository_ImageElementDetailsAsync(_twoSidedItemDetails);
            SetUp_ImageVault_FrontImageMissingAsync();

            // Act
            var result = await _service.GetChequeByIrnAsync(irn, userId, tenantId);

            // Assert
            Assert.True(result.IsFailure);
            Assert.IsType<NotFound>(result.FailureValue);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async void GetChequeByIrnAsync_TargetHostSetToWebClient_ImageVaultCallsWebClient()
        {
            // Arrange
            var tenantId = "Default";
            var service = new WebClientItemService(
                _requestDataAccessor,
                _logger,
                _imageVaultClient,
                _itemRepository,
                _imageConverter,
                _trackerClient);
            var irn = new IrnId("testirn");
            var seqNum = 1;
            var userId = new UserId(12345);

            SetUp_ImageVault_FrontImageMissingAsync();

            // Act
            await service.GetImageFromVaultAsync(irn, seqNum, ImageSurface.Front, 0, userId, tenantId);

            // Assert
            await _imageVaultClient.Received(1)
                                   .GetImageForLegacy(
                                       Arg.Any<RequestIdentifier>(),
                                       LegacyTarget.WebClient,
                                       Arg.Any<string>(),
                                       Arg.Any<UserId>(),
                                       Arg.Any<IrnId>(),
                                       Arg.Any<int>(),
                                       Arg.Any<ImageSurface>(),
                                       Arg.Any<int>());
        }

        [Theory]
        [MemberData(nameof(GetImagesByIrnAsyncInvalidArguments))]
        [Trait("Category", "Unit")]
        public async Task GetChequeByIrnAsync_InvalidArguments_ThrowsException(
            IrnId irn, UserId userId, string tenantId)
        {
            // Arrange, Act, Assert
            await Assert.ThrowsAnyAsync<Exception>(
                () => _service.GetChequeByIrnAsync(irn, userId, tenantId));
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async void GetRemittanceByIrnAsync_FrontAndBackImagesFound_ReturnsTwoSidedRemittance()
        {
            // Arrange
            var tenantId = "Default";
            var irn = new IrnId("testirn");
            var userId = new UserId(12345);

            var expectedRemittance = Remittance.TwoSided(FrontPngImage, BackPngImage);

            SetUp_ItemRepository_ImageElementDetailsAsync(_twoSidedItemDetails);
            SetUp_ImageVault_FrontImageAsync(FrontPngImage);
            SetUp_ImageVault_BackImageAsync(BackPngImage);
            SetUp_ImageConverter_ConvertToPng(FrontPngImage, FrontPngImage);
            SetUp_ImageConverter_ConvertToPng(BackPngImage, BackPngImage);

            // Act
            var result = await _service.GetRemittanceByIrnAsync(irn, userId, tenantId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(expectedRemittance, result.Value);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async void GetRemittanceByIrnAsync_FrontImagesFound_ReturnsOneSidedRemittance()
        {
            // Arrange
            var tenantId = "Default";
            var irn = new IrnId("testirn");
            var userId = new UserId(12345);

            var expectedRemittance = Remittance.OneSided(FrontPngImage);

            SetUp_ItemRepository_ImageElementDetailsAsync(_oneSidedItemDetails);
            SetUp_ImageVault_FrontImageAsync(FrontPngImage);
            SetUp_ImageConverter_ConvertToPng(FrontPngImage, FrontPngImage);

            // Act
            var result = await _service.GetRemittanceByIrnAsync(irn, userId, tenantId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(expectedRemittance, result.Value);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async void GetRemittanceByIrnAsync_ImageDetailsMissing_ReturnsVirtualRemit()
        {
            // Arrange
            var tenantId = "Default";
            var irn = new IrnId("testirn");
            var userId = new UserId(12345);
            var service = new WebClientItemService(
                _requestDataAccessor,
                _logger,
                _imageVaultClient,
                _itemRepository,
                _imageConverter,
                _trackerClient);

            var expectedRemittance = Remittance.Virtual(service.GetVirtualRemittance());

            SetUp_ItemRepository_ImageElementDetailsAsync(_emptyDetails);

            // Act
            var result = await _service.GetRemittanceByIrnAsync(irn, userId, tenantId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(expectedRemittance, result.Value);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async void GetRemittanceByIrnAsync_FrontImageMissing_ReturnsNotFound()
        {
            // Arrange
            var tenantId = "Default";
            var irn = new IrnId("testirn");
            var userId = new UserId(12345);

            SetUp_ItemRepository_ImageElementDetailsAsync(_twoSidedItemDetails);
            SetUp_ImageVault_FrontImageMissingAsync();
            SetUp_ImageVault_BackImageAsync(BackPngImage);
            SetUp_ImageConverter_ConvertToPng(BackPngImage, BackPngImage);

            // Act
            var result = await _service.GetRemittanceByIrnAsync(irn, userId, tenantId);

            // Assert
            Assert.True(result.IsFailure);
            Assert.IsType<NotFound>(result.FailureValue);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async void GetRemittanceByIrnAsync_BackImageMissing_ReturnsNotFound()
        {
            // Arrange
            var tenantId = "Default";
            var irn = new IrnId("testirn");
            var userId = new UserId(12345);

            SetUp_ItemRepository_ImageElementDetailsAsync(_twoSidedItemDetails);
            SetUp_ImageVault_FrontImageAsync(FrontPngImage);
            SetUp_ImageConverter_ConvertToPng(FrontPngImage, FrontPngImage);
            SetUp_ImageVault_BackImageMissingAsync();

            // Act
            var result = await _service.GetRemittanceByIrnAsync(irn, userId, tenantId);

            // Assert
            Assert.True(result.IsFailure);
            Assert.IsType<NotFound>(result.FailureValue);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async void GetRemittanceByIrnAsync_ErrorConvertingImage_ReturnsError()
        {
            // Arrange
            var tenantId = "Default";
            var irn = new IrnId("testirn");
            var userId = new UserId(12345);

            SetUp_ItemRepository_ImageElementDetailsAsync(_twoSidedItemDetails);
            SetUp_ImageVault_FrontImageAsync(FrontPngImage);
            SetUp_ImageVault_BackImageAsync(BackPngImage);
            SetUp_ImageConverter_ConvertToPng(FrontPngImage, new Error("Bad stuff happened in that image converter."));
            SetUp_ImageConverter_ConvertToPng(BackPngImage, BackPngImage);

            // Act
            var result = await _service.GetRemittanceByIrnAsync(irn, userId, tenantId);

            // Assert
            Assert.True(result.IsFailure);
            Assert.IsType<Error>(result.FailureValue);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async void GetRemittanceByIrnAsync_ExceptionDuringExecution_ReturnsServiceFailure()
        {
            // Arrange
            var tenantId = "Default";
            var irn = new IrnId("testirn");
            var userId = new UserId(12345);

            _itemRepository.GetImageElementDetailsByIrn(Arg.Any<IrnId>()).Throws(new Exception("Service failure."));

            // Act
            var result = await _service.GetRemittanceByIrnAsync(irn, userId, tenantId);

            // Assert
            Assert.True(result.IsFailure);
            Assert.IsType<ServiceFailure>(result.FailureValue);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async void GetRemittanceByIrnAsync_SeqNumberMissing_ReturnsNotFound()
        {
            // Arrange
            var tenantId = "Default";
            var irn = new IrnId("testirn");
            var userId = new UserId(12345);

            SetUp_ItemRepository_ImageElementDetailsAsync(_twoSidedItemDetails);
            SetUp_ImageVault_FrontImageMissingAsync();

            // Act
            var result = await _service.GetRemittanceByIrnAsync(irn, userId, tenantId);

            // Assert
            Assert.True(result.IsFailure);
            Assert.IsType<NotFound>(result.FailureValue);
        }

        [Theory]
        [MemberData(nameof(GetImagesByIrnAsyncInvalidArguments))]
        [Trait("Category", "Unit")]
        public async Task GetRemittanceByIrnAsync_InvalidArguments_ThrowsException(
            IrnId irn, UserId userId, string tenantId)
        {
            // Arrange, Act, Assert
            await Assert.ThrowsAnyAsync<Exception>(() => _service.GetRemittanceByIrnAsync(irn, userId, tenantId));
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async void GetGeneralDocumentByIrnAsync_MultiPageDocumentWithFrontAndBackImagesFound_ReturnsExpectedDocument()
        {
            // Arrange
            var tenantId = "Default";
            var irn = new IrnId("testirn");
            var userId = new UserId(12345);
            var pages = new List<GeneralDocumentPage>
            {
                new GeneralDocumentPage(0, FrontPngImage, BackPngImage),
                new GeneralDocumentPage(1, FrontPngImage, BackPngImage),
                new GeneralDocumentPage(2, FrontPngImage, BackPngImage)
            };

            var itemGeneral = new GeneralDocumentItemDetails(1, "testDoc", null, "Owner");

            var expectedGeneralDocument = new GeneralDocument(itemGeneral.DocumentName, pages);

            SetUp_ItemRepository_ImageElementDetailsAsync(_multiPageTwoSidedItemDetails);
            SetUp_ItemRepository_DocumentNameAsync(itemGeneral);
            SetUp_ImageVault_FrontImageAsync(FrontPngImage);
            SetUp_ImageVault_BackImageAsync(BackPngImage);
            SetUp_ImageConverter_ConvertToPng(FrontPngImage, FrontPngImage);
            SetUp_ImageConverter_ConvertToPng(BackPngImage, BackPngImage);

            // Act
            var result = await _service.GetGeneralDocumentByIrnAsync(irn, userId, tenantId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(expectedGeneralDocument, result.Value);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async void GetGeneralDocumentByIrnAsync_MultiPageDocumentWithFrontImagesFound_ReturnsExpectedDocument()
        {
            // Arrange
            var tenantId = "Default";
            var irn = new IrnId("testirn");
            var userId = new UserId(12345);
            var pages = new List<GeneralDocumentPage>
            {
                new GeneralDocumentPage(0, FrontPngImage, Maybe<Image>.Empty()),
                new GeneralDocumentPage(1, FrontPngImage, Maybe<Image>.Empty()),
                new GeneralDocumentPage(2, FrontPngImage, Maybe<Image>.Empty())
            };

            var itemGeneral = new GeneralDocumentItemDetails(1, "testDoc", null, "Owner");

            var expectedGeneralDocument = new GeneralDocument(itemGeneral.DocumentName, pages);

            SetUp_ItemRepository_ImageElementDetailsAsync(_multiPageOneSidedItemDetails);
            SetUp_ItemRepository_DocumentNameAsync(itemGeneral);
            SetUp_ImageVault_FrontImageAsync(FrontPngImage);
            SetUp_ImageConverter_ConvertToPng(FrontPngImage, FrontPngImage);

            // Act
            var result = await _service.GetGeneralDocumentByIrnAsync(irn, userId, tenantId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(expectedGeneralDocument, result.Value);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async void GetGeneralDocumentByIrnAsync_MultiPageDocumentWithAssortedFrontAndBackImages_ReturnsExpectedDocument()
        {
            // Arrange
            var tenantId = "Default";
            var irn = new IrnId("testirn");
            var userId = new UserId(12345);
            var pages = new List<GeneralDocumentPage>
            {
                new GeneralDocumentPage(0, FrontPngImage, BackPngImage),
                new GeneralDocumentPage(1, FrontPngImage, Maybe<Image>.Empty()),
                new GeneralDocumentPage(2, FrontPngImage, Maybe<Image>.Empty())
            };

            var itemGeneral = new GeneralDocumentItemDetails(1, "testDoc", null, "Owner");

            var expectedGeneralDocument = new GeneralDocument(itemGeneral.DocumentName, pages);

            SetUp_ItemRepository_ImageElementDetailsAsync(_multiPageAssortedItemDetails);
            SetUp_ItemRepository_DocumentNameAsync(itemGeneral);
            SetUp_ImageVault_FrontImageAsync(FrontPngImage);
            SetUp_ImageVault_BackImageAsync(BackPngImage);
            SetUp_ImageConverter_ConvertToPng(FrontPngImage, FrontPngImage);
            SetUp_ImageConverter_ConvertToPng(BackPngImage, BackPngImage);

            // Act
            var result = await _service.GetGeneralDocumentByIrnAsync(irn, userId, tenantId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(expectedGeneralDocument, result.Value);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async void GetGeneralDocumentByIrnAsync_MultiPageDocumentWithNoDocumentName_ReturnsExpectedDocument()
        {
            // Arrange
            var tenantId = "Default";
            var irn = new IrnId("testirn");
            var userId = new UserId(12345);
            var pages = new List<GeneralDocumentPage>
            {
                new GeneralDocumentPage(0, FrontPngImage, BackPngImage),
                new GeneralDocumentPage(1, FrontPngImage, BackPngImage),
                new GeneralDocumentPage(2, FrontPngImage, BackPngImage)
            };

            var itemGeneral = new GeneralDocumentItemDetails(1, string.Empty, null, "Owner");

            var expectedGeneralDocument = new GeneralDocument(itemGeneral.DocumentName, pages);

            SetUp_ItemRepository_ImageElementDetailsAsync(_multiPageTwoSidedItemDetails);
            SetUp_ItemRepository_DocumentNameAsync(itemGeneral);
            SetUp_ImageVault_FrontImageAsync(FrontPngImage);
            SetUp_ImageVault_BackImageAsync(BackPngImage);
            SetUp_ImageConverter_ConvertToPng(FrontPngImage, FrontPngImage);
            SetUp_ImageConverter_ConvertToPng(BackPngImage, BackPngImage);

            // Act
            var result = await _service.GetGeneralDocumentByIrnAsync(irn, userId, tenantId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(expectedGeneralDocument, result.Value);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async void GetGeneralDocumentByIrnAsync_NotAGeneralDocument_ReturnsNotFound()
        {
            // Arrange
            var tenantId = "Default";
            var irn = new IrnId("testirn");
            var userId = new UserId(12345);

            SetUp_ItemRepository_DocumentNameAsync(Maybe<GeneralDocumentItemDetails>.Empty());

            // Act
            var result = await _service.GetGeneralDocumentByIrnAsync(irn, userId, tenantId);

            // Assert
            Assert.True(result.IsFailure);
            Assert.IsType<NotFound>(result.FailureValue);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async void GetGeneralDocumentByIrnAsync_PageIsMissingFrontImage_ReturnsNotFound()
        {
            // Arrange
            var tenantId = "Default";
            var irn = new IrnId("testirn");
            var userId = new UserId(12345);
            var itemGeneral = new GeneralDocumentItemDetails(1, string.Empty, null, "Owner");

            SetUp_ItemRepository_DocumentNameAsync(itemGeneral);
            SetUp_ItemRepository_ImageElementDetailsAsync(_multiPageTwoSidedItemDetails);
            SetUp_ImageVault_FrontImageMissingAsync();

            // Act
            var result = await _service.GetGeneralDocumentByIrnAsync(irn, userId, tenantId);

            // Assert
            Assert.True(result.IsFailure);
            Assert.IsType<NotFound>(result.FailureValue);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async void GetGeneralDocumentByIrnAsync_PageIsMissingBackImage_ReturnsNotFound()
        {
            // Arrange
            var tenantId = "Default";
            var irn = new IrnId("testirn");
            var userId = new UserId(12345);
            var itemGeneral = new GeneralDocumentItemDetails(1, string.Empty, null, "Owner");

            SetUp_ItemRepository_DocumentNameAsync(itemGeneral);
            SetUp_ItemRepository_ImageElementDetailsAsync(_multiPageTwoSidedItemDetails);
            SetUp_ImageVault_FrontImageAsync(FrontPngImage);
            SetUp_ImageConverter_ConvertToPng(FrontPngImage, FrontPngImage);
            SetUp_ImageVault_BackImageMissingAsync();

            // Act
            var result = await _service.GetGeneralDocumentByIrnAsync(irn, userId, tenantId);

            // Assert
            Assert.True(result.IsFailure);
            Assert.IsType<NotFound>(result.FailureValue);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async void GetGeneralDocumentByIrnAsync_ImageDetailsMissing_ReturnsNotFound()
        {
            // Arrange
            var tenantId = "Default";
            var irn = new IrnId("testirn");
            var userId = new UserId(12345);
            var itemGeneral = new GeneralDocumentItemDetails(1, string.Empty, null, "Owner");

            SetUp_ItemRepository_DocumentNameAsync(itemGeneral);
            SetUp_ItemRepository_ImageElementDetailsAsync(_emptyDetails);

            // Act
            var result = await _service.GetGeneralDocumentByIrnAsync(irn, userId, tenantId);

            // Assert
            Assert.True(result.IsFailure);
            Assert.IsType<NotFound>(result.FailureValue);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async void GetGeneralDocumentByIrnAsync_ErrorConvertingImage_ReturnsError()
        {
            // Arrange
            var tenantId = "Default";
            var irn = new IrnId("testirn");
            var userId = new UserId(12345);
            var itemGeneral = new GeneralDocumentItemDetails(1, string.Empty, null, "Owner");

            SetUp_ItemRepository_DocumentNameAsync(itemGeneral);
            SetUp_ItemRepository_ImageElementDetailsAsync(_multiPageTwoSidedItemDetails);
            SetUp_ImageVault_FrontImageAsync(FrontPngImage);
            SetUp_ImageVault_BackImageAsync(BackPngImage);
            SetUp_ImageConverter_ConvertToPng(FrontPngImage, new Error("Conversion failure."));
            SetUp_ImageConverter_ConvertToPng(BackPngImage, BackPngImage);

            // Act
            var result = await _service.GetGeneralDocumentByIrnAsync(irn, userId, tenantId);

            // Assert
            Assert.True(result.IsFailure);
            Assert.IsType<Error>(result.FailureValue);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async void GetGeneralDocumentByIrnAsync_SeqNumberMissing_ReturnsNotFound()
        {
            // Arrange
            var tenantId = "Default";
            var irn = new IrnId("testirn");
            var userId = new UserId(12345);
            var itemGeneral = new GeneralDocumentItemDetails(1, string.Empty, null, "Owner");

            SetUp_ItemRepository_DocumentNameAsync(itemGeneral);
            SetUp_ItemRepository_ImageElementDetailsAsync(_twoSidedItemDetails);
            SetUp_ImageVault_FrontImageMissingAsync();

            // Act
            var result = await _service.GetGeneralDocumentByIrnAsync(irn, userId, tenantId);

            // Assert
            Assert.True(result.IsFailure);
            Assert.IsType<NotFound>(result.FailureValue);
        }

        [Theory]
        [MemberData(nameof(GetImagesByIrnAsyncInvalidArguments))]
        [Trait("Category", "Unit")]
        public async Task GetGeneralDocumentByIrnAsync_InvalidArguments_ThrowsException(
            IrnId irn, UserId userId, string tenantId)
        {
            // Arrange, Act, Assert
            await Assert.ThrowsAnyAsync<Exception>(() => _service.GetGeneralDocumentByIrnAsync(irn, userId, tenantId));
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async void UpdateDocumentNameByIrn_DocumentNameFound_DocumentNameUpdated()
        {
            // Arrange
            var irn = new IrnId("testirn");
            var docName = "Doc Name";
            var userId = new UserId(123);
            var module = Module.Scan;
            var itemGeneral = new GeneralDocumentItemDetails(1, "testDoc", null, "Owner");

            SetUp_ItemRepository_DocumentNameAsync(itemGeneral);
            SetUp_ItemRepository_UpdateDocumentNameAsync(true);

            // Act
            var result = await _service.UpdateDocumentNameByIrnAsync(irn, docName, userId, module);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.True(result.Value);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async void UpdateDocumentNameByIrnAsync_DocumentNameMissing_ReturnsNotFound()
        {
            // Arrange
            var irn = new IrnId("testirn");
            var docName = "Doc Name";
            var userId = new UserId(123);
            var module = Module.Scan;
            var itemGeneral = new GeneralDocumentItemDetails(1, "testDoc", null, "Owner");

            SetUp_ItemRepository_DocumentNameAsync(itemGeneral);
            SetUp_ItemRepository_UpdateDocumentNameAsync(new NotFound());

            // Act
            var result = await _service.UpdateDocumentNameByIrnAsync(irn, docName, userId, module);

            // Assert
            Assert.True(result.IsFailure);
            Assert.IsType<NotFound>(result.FailureValue);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async void UpdateDocumentNameByIrnAsync_ErrorUpdatingDocumentName_ReturnsError()
        {
            // Arrange
            var irn = new IrnId("testirn");
            var docName = "Doc Name";
            var userId = new UserId(123);
            var module = Module.Scan;

            _itemRepository.UpdateDocumentNameByIrnAsync(Arg.Any<IrnId>(), Arg.Any<int>(), Arg.Any<string>())
                           .Throws(new Exception("Service failure."));

            // Act
            var result = await _service.UpdateDocumentNameByIrnAsync(irn, docName, userId, module);

            // Assert
            Assert.True(result.IsFailure);
            Assert.IsType<ServiceFailure>(result.FailureValue);
        }

        public static IEnumerable<object[]> UpdateDocumentNameAsyncInvalidArguments()
        {
            var irn = new IrnId("EYMG6NTRKU80C04SW4CWWGKW8");
            var documentName = "Document Name";
            var userId = new UserId(123);

            return new List<object[]>
            {
                new object[] { IrnId.Empty, documentName, userId },
                new object[] { null, documentName, userId },
                new object[] { irn, null, userId },
                new object[] { irn, documentName, null },
                new object[] { irn, documentName, UserId.InvalidUser }
            };
        }

        [Theory]
        [MemberData(nameof(UpdateDocumentNameAsyncInvalidArguments))]
        [Trait("Category", "Unit")]
        public async Task UpdateDocumentNameAsyncInvalidArgumentsAsync_InvalidArguments_ThrowsException(
            IrnId irn, string documentName, UserId userId)
        {
            // Arrange, Act, Assert
            await Assert.ThrowsAnyAsync<Exception>(
                () => _service.UpdateDocumentNameByIrnAsync(irn, documentName, userId, Module.Authorize));
        }

        #region Substitution Helpers

        private Image FrontPngImage => new Image(new byte[] { 1, 2, 3, 4 }, "image/png", 1252, 555);

        private Image BackPngImage => new Image(new byte[] { 5, 6, 7, 8 }, "image/png", 1250, 556);

        private readonly Maybe<IList<ImageElementDetails>> _emptyDetails = Maybe<IList<ImageElementDetails>>.Empty();

        private readonly List<ImageElementDetails> _twoSidedItemDetails = new List<ImageElementDetails>
        {
            new ImageElementDetails(0, ImageSurface.Front, 1),
            new ImageElementDetails(0, ImageSurface.Back, 1)
        };

        private readonly List<ImageElementDetails> _oneSidedItemDetails = new List<ImageElementDetails>
        {
            new ImageElementDetails(0, ImageSurface.Front, 1)
        };

        private readonly List<ImageElementDetails> _multiPageTwoSidedItemDetails = new List<ImageElementDetails>
        {
            new ImageElementDetails(0, ImageSurface.Front, 1),
            new ImageElementDetails(0, ImageSurface.Back, 1),
            new ImageElementDetails(1, ImageSurface.Front, 1),
            new ImageElementDetails(1, ImageSurface.Back, 1),
            new ImageElementDetails(2, ImageSurface.Front, 1),
            new ImageElementDetails(2, ImageSurface.Back, 1)
        };

        private readonly List<ImageElementDetails> _multiPageOneSidedItemDetails = new List<ImageElementDetails>
        {
            new ImageElementDetails(0, ImageSurface.Front, 1),
            new ImageElementDetails(1, ImageSurface.Front, 1),
            new ImageElementDetails(2, ImageSurface.Front, 1)
        };

        private readonly List<ImageElementDetails> _multiPageAssortedItemDetails = new List<ImageElementDetails>
        {
            new ImageElementDetails(0, ImageSurface.Front, 1),
            new ImageElementDetails(0, ImageSurface.Back, 1),
            new ImageElementDetails(1, ImageSurface.Front, 1),
            new ImageElementDetails(2, ImageSurface.Front, 1)
        };

        private void SetUp_ItemRepository_ImageElementDetailsAsync(Maybe<IList<ImageElementDetails>> result)
        {
            _itemRepository.GetImageElementDetailsByIrnAsync(Arg.Any<IrnId>()).Returns(result);
        }

        private void SetUp_ItemRepository_UpdateDocumentNameAsync(Result<Error, bool> result)
        {
            _itemRepository.UpdateDocumentNameByIrnAsync(Arg.Any<IrnId>(), Arg.Any<int>(), Arg.Any<string>()).Returns(result);
        }

        private void SetUp_ItemRepository_DocumentNameAsync(Maybe<GeneralDocumentItemDetails> result)
        {
            _itemRepository.GetGeneralDocumentDetailsByIrnAsync(Arg.Any<IrnId>(), Arg.Any<int>()).Returns(result);
        }

        private void SetUp_ImageVault_FrontImageAsync(Image image)
        {
            _imageVaultClient.GetImageForLegacy(
                                 Arg.Any<RequestIdentifier>(),
                                 Arg.Any<LegacyTarget>(),
                                 Arg.Any<string>(),
                                 Arg.Any<UserId>(),
                                 Arg.Any<IrnId>(),
                                 Arg.Any<int>(),
                                 ImageSurface.Front,
                                 Arg.Any<int>())
                             .Returns(image);
        }

        private void SetUp_ImageVault_FrontImageMissingAsync()
        {
            _imageVaultClient.GetImageForLegacy(
                    Arg.Any<RequestIdentifier>(),
                    Arg.Any<LegacyTarget>(),
                    Arg.Any<string>(),
                    Arg.Any<UserId>(),
                    Arg.Any<IrnId>(),
                    Arg.Any<int>(),
                    ImageSurface.Front,
                    Arg.Any<int>())
                .Throws(new RpcException(new Status(StatusCode.NotFound, "Item Not Found")));
        }

        private void SetUp_ImageVault_BackImageAsync(Image image)
        {
            _imageVaultClient.GetImageForLegacy(
                                 Arg.Any<RequestIdentifier>(),
                                 Arg.Any<LegacyTarget>(),
                                 Arg.Any<string>(),
                                 Arg.Any<UserId>(),
                                 Arg.Any<IrnId>(),
                                 Arg.Any<int>(),
                                 ImageSurface.Back,
                                 Arg.Any<int>())
                             .Returns(image);
        }

        private void SetUp_ImageVault_BackImageMissingAsync()
        {
            _imageVaultClient.GetImageForLegacy(
                    Arg.Any<RequestIdentifier>(),
                    Arg.Any<LegacyTarget>(),
                    Arg.Any<string>(),
                    Arg.Any<UserId>(),
                    Arg.Any<IrnId>(),
                    Arg.Any<int>(),
                    ImageSurface.Back,
                    Arg.Any<int>())
                .Throws(new RpcException(new Status(StatusCode.NotFound, "Item Not Found")));
        }

        private void SetUp_ImageConverter_ConvertToPng(Image imageToConvert, Result<Error, Image> result)
        {
            _imageConverter.ConvertImageToPng(imageToConvert).Returns(result);
        }

        #endregion
    }
}