using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Grpc.Core;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using RDM.Client.ImageVault;
using RDM.Core;
using RDM.Legacy.Itms;
using RDM.Legacy.Share;
using RDM.Messaging.ImageVault;
using RDM.Model.Itms;
using RDM.Models.ImageViewerAPI;
using Serilog;
using Xunit;

namespace RDM.Services.ImageViewerAPI.Tests
{
    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Test files are not styled.")]
    [SuppressMessage("StyleCop.CSharp.LayoutRules", "*", Justification = "Test files are not styled.")]
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "*", Justification = "Test files are not styled.")]
    [SuppressMessage("StyleCop.CSharp.OrderingRules", "*", Justification = "Test files are not styled.")]
    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "*", Justification = "Test files are not styled.")]
    [SuppressMessage("StyleCop.CSharp.SpacingRules", "*", Justification = "Test files are not styled.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "Test files are not styled.")]
    public class ItmsItemServiceUnitTests
    {
        private readonly IRequestDataAccessor _requestDataAccessor;
        private readonly ILogger _logger;
        private readonly IImageVaultClient _imageVaultClient;
        private readonly IItmsItemRepository _itemRepository;
        private readonly IItmsItemService _service;
        private readonly IImageConverter _imageConverter;

        public ItmsItemServiceUnitTests()
        {
            _requestDataAccessor = Substitute.For<IRequestDataAccessor>();
            _requestDataAccessor.RequestId.Returns(new RequestIdentifier("1"));
            _logger = Substitute.For<ILogger>();
            _imageVaultClient = Substitute.For<IImageVaultClient>();
            _itemRepository = Substitute.For<IItmsItemRepository>();
            _imageConverter = Substitute.For<IImageConverter>();

            _service = new ItmsItemService(_requestDataAccessor, _logger, _imageVaultClient, _itemRepository, _imageConverter);
        }

        public static IEnumerable<object[]> GetImagesByIrnAsyncInvalidArguments()
        {
            var irn = new IrnId("EYMG6NTRKU80C04SW4CWWGKW8");
            var userId = new UserId(123);
            var tenantId = "Default1";
            var startInsertTime = DateTime.Now.AddMonths(-26);
            var endInsertTime = DateTime.Now;

            return new List<object[]>
            {
                new object[] { IrnId.Empty, userId, tenantId, startInsertTime, endInsertTime },
                new object[] { null, userId, tenantId, startInsertTime, endInsertTime },
                new object[] { irn, null, tenantId, startInsertTime, endInsertTime },
                new object[] { irn, UserId.InvalidUser, tenantId, startInsertTime, endInsertTime },
                new object[] { irn, userId, null, startInsertTime, endInsertTime },
                new object[] { irn, userId, "    ", startInsertTime, endInsertTime }
            };
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async void GetChequeByIrnAsync_FrontAndBackImagesFound_ReturnsTwoSidedCheque()
        {
            // Arrange
            var tenantId = "Default";
            var irn = new IrnId("testirn");
            var seqNum = 1;
            var userId = new UserId(12345);
            var startInsertTime = DateTime.Now.AddMonths(-26);
            var endInsertTime = DateTime.Now;

            var expectedCheque = Cheque.TwoSided(FrontPngImage, BackPngImage);

            SetUp_ItemRepository_ImageElementDetailsAsync(_twoSidedItemDetails);
            SetUp_ImageVault_FrontImageAsync(FrontPngImage);
            SetUp_ImageVault_BackImageAsync(BackPngImage);
            SetUp_ImageConverter_ConvertToPng(FrontPngImage, FrontPngImage);
            SetUp_ImageConverter_ConvertToPng(BackPngImage, BackPngImage);

            // Act
            var result = await _service.GetChequeByIrnAsync(irn, seqNum, userId, tenantId, startInsertTime, endInsertTime);

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
            var seqNum = 1;
            var userId = new UserId(12345);
            var startInsertTime = DateTime.Now.AddMonths(-26);
            var endInsertTime = DateTime.Now;

            var expectedCheque = Cheque.OneSided(FrontPngImage);

            SetUp_ItemRepository_ImageElementDetailsAsync(_oneSidedItemDetails);
            SetUp_ImageVault_FrontImageAsync(FrontPngImage);
            SetUp_ImageConverter_ConvertToPng(FrontPngImage, FrontPngImage);

            // Act
            var result = await _service.GetChequeByIrnAsync(irn, seqNum, userId, tenantId, startInsertTime, endInsertTime);

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
            var seqNum = 1;
            var userId = new UserId(12345);
            var startInsertTime = DateTime.Now.AddMonths(-26);
            var endInsertTime = DateTime.Now;

            SetUp_ItemRepository_ImageElementDetailsAsync(_emptyDetails);

            // Act
            var result = await _service.GetChequeByIrnAsync(irn, seqNum, userId, tenantId, startInsertTime, endInsertTime);

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
            var seqNum = 1;
            var userId = new UserId(12345);
            var startInsertTime = DateTime.Now.AddMonths(-26);
            var endInsertTime = DateTime.Now;

            SetUp_ItemRepository_ImageElementDetailsAsync(_twoSidedItemDetails);
            SetUp_ImageVault_FrontImageMissingAsync();
            SetUp_ImageVault_BackImageAsync(BackPngImage);
            SetUp_ImageConverter_ConvertToPng(BackPngImage, BackPngImage);

            // Act
            var result = await _service.GetChequeByIrnAsync(irn, seqNum, userId, tenantId, startInsertTime, endInsertTime);

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
            var seqNum = 1;
            var userId = new UserId(12345);
            var startInsertTime = DateTime.Now.AddMonths(-26);
            var endInsertTime = DateTime.Now;

            SetUp_ItemRepository_ImageElementDetailsAsync(_twoSidedItemDetails);
            SetUp_ImageVault_FrontImageAsync(FrontPngImage);
            SetUp_ImageConverter_ConvertToPng(FrontPngImage, FrontPngImage);
            SetUp_ImageVault_BackImageMissingAsync();

            // Act
            var result = await _service.GetChequeByIrnAsync(irn, seqNum, userId, tenantId, startInsertTime, endInsertTime);

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
            var seqNum = 1;
            var userId = new UserId(12345);
            var startInsertTime = DateTime.Now.AddMonths(-26);
            var endInsertTime = DateTime.Now;

            SetUp_ItemRepository_ImageElementDetailsAsync(_twoSidedItemDetails);
            SetUp_ImageVault_FrontImageAsync(FrontPngImage);
            SetUp_ImageVault_BackImageAsync(BackPngImage);
            SetUp_ImageConverter_ConvertToPng(FrontPngImage, new Error("Bad stuff happened in that image converter."));
            SetUp_ImageConverter_ConvertToPng(BackPngImage, BackPngImage);

            // Act
            var result = await _service.GetChequeByIrnAsync(irn, seqNum, userId, tenantId, startInsertTime, endInsertTime);

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
            var seqNum = 1;
            var userId = new UserId(12345);
            var startInsertTime = DateTime.Now.AddMonths(-26);
            var endInsertTime = DateTime.Now;

            _itemRepository.GetImageElementDetailsByIrn(Arg.Any<IrnId>(), Arg.Any<int>()).Throws(new Exception("Service failure."));

            // Act
            var result = await _service.GetChequeByIrnAsync(irn, seqNum, userId, tenantId, startInsertTime, endInsertTime);

            // Assert
            Assert.True(result.IsFailure);
            Assert.IsType<ServiceFailure>(result.FailureValue);
        }

        [Theory]
        [MemberData(nameof(GetImagesByIrnAsyncInvalidArguments))]
        [Trait("Category", "Unit")]
        public async Task GetChequeByIrnAsync_InvalidArguments_ThrowsException(IrnId irn, UserId userId, string tenantId, DateTime startInsertTime, DateTime endInsertTime)
        {
            // Arrange
            var seqNum = 1;

            // Act, Assert
            await Assert.ThrowsAnyAsync<Exception>(() => _service.GetChequeByIrnAsync(irn, seqNum, userId, tenantId, startInsertTime, endInsertTime));
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async void GetRemittanceByIrnAsync_FrontAndBackImagesFound_ReturnsTwoSidedRemittance()
        {
            // Arrange
            var tenantId = "Default";
            var irn = new IrnId("testirn");
            var seqNum = 1;
            var userId = new UserId(12345);
            var startInsertTime = DateTime.Now.AddMonths(-26);
            var endInsertTime = DateTime.Now;

            var expectedRemittance = Remittance.TwoSided(FrontPngImage, BackPngImage);

            SetUp_ItemRepository_ImageElementDetailsAsync(_twoSidedItemDetails);
            SetUp_ImageVault_FrontImageAsync(FrontPngImage);
            SetUp_ImageVault_BackImageAsync(BackPngImage);
            SetUp_ImageConverter_ConvertToPng(FrontPngImage, FrontPngImage);
            SetUp_ImageConverter_ConvertToPng(BackPngImage, BackPngImage);

            // Act
            var result = await _service.GetRemittanceByIrnAsync(irn, seqNum, userId, tenantId, startInsertTime, endInsertTime);

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
            var seqNum = 1;
            var userId = new UserId(12345);
            var startInsertTime = DateTime.Now.AddMonths(-26);
            var endInsertTime = DateTime.Now;

            var expectedRemittance = Remittance.OneSided(FrontPngImage);

            SetUp_ItemRepository_ImageElementDetailsAsync(_oneSidedItemDetails);
            SetUp_ImageVault_FrontImageAsync(FrontPngImage);
            SetUp_ImageConverter_ConvertToPng(FrontPngImage, FrontPngImage);

            // Act
            var result = await _service.GetRemittanceByIrnAsync(irn, seqNum, userId, tenantId, startInsertTime, endInsertTime);

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
            var seqNum = 1;
            var userId = new UserId(12345);
            var startInsertTime = DateTime.Now.AddMonths(-26);
            var endInsertTime = DateTime.Now;
            var service = new ItmsItemService(_requestDataAccessor, _logger, _imageVaultClient, _itemRepository, _imageConverter);

            var expectedRemittance = Remittance.Virtual(service.GetVirtualRemittance());

            SetUp_ItemRepository_ImageElementDetailsAsync(_emptyDetails);

            // Act
            var result = await _service.GetRemittanceByIrnAsync(irn, seqNum, userId, tenantId, startInsertTime, endInsertTime);

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
            var seqNum = 1;
            var userId = new UserId(12345);
            var startInsertTime = DateTime.Now.AddMonths(-26);
            var endInsertTime = DateTime.Now;

            SetUp_ItemRepository_ImageElementDetailsAsync(_twoSidedItemDetails);
            SetUp_ImageVault_FrontImageMissingAsync();
            SetUp_ImageVault_BackImageAsync(BackPngImage);
            SetUp_ImageConverter_ConvertToPng(BackPngImage, BackPngImage);

            // Act
            var result = await _service.GetRemittanceByIrnAsync(irn, seqNum, userId, tenantId, startInsertTime, endInsertTime);

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
            var seqNum = 1;
            var userId = new UserId(12345);
            var startInsertTime = DateTime.Now.AddMonths(-26);
            var endInsertTime = DateTime.Now;

            SetUp_ItemRepository_ImageElementDetailsAsync(_twoSidedItemDetails);
            SetUp_ImageVault_FrontImageAsync(FrontPngImage);
            SetUp_ImageConverter_ConvertToPng(FrontPngImage, FrontPngImage);
            SetUp_ImageVault_BackImageMissingAsync();

            // Act
            var result = await _service.GetRemittanceByIrnAsync(irn, seqNum, userId, tenantId, startInsertTime, endInsertTime);

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
            var seqNum = 1;
            var userId = new UserId(12345);
            var startInsertTime = DateTime.Now.AddMonths(-26);
            var endInsertTime = DateTime.Now;

            SetUp_ItemRepository_ImageElementDetailsAsync(_twoSidedItemDetails);
            SetUp_ImageVault_FrontImageAsync(FrontPngImage);
            SetUp_ImageVault_BackImageAsync(BackPngImage);
            SetUp_ImageConverter_ConvertToPng(FrontPngImage, new Error("Bad stuff happened in that image converter."));
            SetUp_ImageConverter_ConvertToPng(BackPngImage, BackPngImage);

            // Act
            var result = await _service.GetRemittanceByIrnAsync(irn, seqNum, userId, tenantId, startInsertTime, endInsertTime);

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
            var seqNum = 1;
            var userId = new UserId(12345);
            var startInsertTime = DateTime.Now.AddMonths(-26);
            var endInsertTime = DateTime.Now;

            _itemRepository.GetImageElementDetailsByIrn(Arg.Any<IrnId>(), Arg.Any<int>()).Throws(new Exception("Service failure."));

            // Act
            var result = await _service.GetRemittanceByIrnAsync(irn, seqNum, userId, tenantId, startInsertTime, endInsertTime);

            // Assert
            Assert.True(result.IsFailure);
            Assert.IsType<ServiceFailure>(result.FailureValue);
        }

        [Theory]
        [MemberData(nameof(GetImagesByIrnAsyncInvalidArguments))]
        [Trait("Category", "Unit")]
        public async Task GetRemittanceByIrnAsync_InvalidArguments_ThrowsException(IrnId irn, UserId userId, string tenantId, DateTime startInsertTime, DateTime endInsertTime)
        {
            // Arrange
            var seqNum = 1;

            // Act, Assert
            await Assert.ThrowsAnyAsync<Exception>(() => _service.GetRemittanceByIrnAsync(irn, seqNum, userId, tenantId, startInsertTime, endInsertTime));
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async void GetGeneralDocumentByIrnAsync_MultiPageDocumentWithFrontAndBackImagesFound_ReturnsExpectedDocument()
        {
            // Arrange
            var tenantId = "Default";
            var irn = new IrnId("testirn");
            var seqNum = 1;
            var userId = new UserId(12345);
            var startInsertTime = DateTime.Now.AddMonths(-26);
            var endInsertTime = DateTime.Now;
            var pages = new List<GeneralDocumentPage>
            {
                new GeneralDocumentPage(0, FrontPngImage, BackPngImage),
                new GeneralDocumentPage(1, FrontPngImage, BackPngImage),
                new GeneralDocumentPage(2, FrontPngImage, BackPngImage)
            };

            var itemGeneral = new GeneralDocumentItemDetails(1, "testDoc", 2, "Owner");

            var expectedGeneralDocument = new GeneralDocument(itemGeneral.DocumentName, pages);

            SetUp_ItemRepository_ImageElementDetailsAsync(_multiPageTwoSidedItemDetails);
            SetUp_ItemRepository_DocumentNameAsync(itemGeneral);
            SetUp_ImageVault_FrontImageAsync(FrontPngImage);
            SetUp_ImageVault_BackImageAsync(BackPngImage);
            SetUp_ImageConverter_ConvertToPng(FrontPngImage, FrontPngImage);
            SetUp_ImageConverter_ConvertToPng(BackPngImage, BackPngImage);

            // Act
            var result = await _service.GetGeneralDocumentByIrnAsync(irn, seqNum, userId, tenantId, startInsertTime, endInsertTime);

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
            var seqNum = 1;
            var userId = new UserId(12345);
            var startInsertTime = DateTime.Now.AddMonths(-26);
            var endInsertTime = DateTime.Now;
            var pages = new List<GeneralDocumentPage>
            {
                new GeneralDocumentPage(0, FrontPngImage, Maybe<Image>.Empty()),
                new GeneralDocumentPage(1, FrontPngImage, Maybe<Image>.Empty()),
                new GeneralDocumentPage(2, FrontPngImage, Maybe<Image>.Empty())
            };

            var itemGeneral = new GeneralDocumentItemDetails(1, "testDoc", 2, "Owner");

            var expectedGeneralDocument = new GeneralDocument(itemGeneral.DocumentName, pages);

            SetUp_ItemRepository_ImageElementDetailsAsync(_multiPageOneSidedItemDetails);
            SetUp_ItemRepository_DocumentNameAsync(itemGeneral);
            SetUp_ImageVault_FrontImageAsync(FrontPngImage);
            SetUp_ImageConverter_ConvertToPng(FrontPngImage, FrontPngImage);

            // Act
            var result = await _service.GetGeneralDocumentByIrnAsync(irn, seqNum, userId, tenantId, startInsertTime, endInsertTime);

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
            var seqNum = 1;
            var userId = new UserId(12345);
            var startInsertTime = DateTime.Now.AddMonths(-26);
            var endInsertTime = DateTime.Now;
            var pages = new List<GeneralDocumentPage>
            {
                new GeneralDocumentPage(0, FrontPngImage, BackPngImage),
                new GeneralDocumentPage(1, FrontPngImage, Maybe<Image>.Empty()),
                new GeneralDocumentPage(2, FrontPngImage, Maybe<Image>.Empty())
            };

            var itemGeneral = new GeneralDocumentItemDetails(1, "testDoc", 2, "Owner");

            var expectedGeneralDocument = new GeneralDocument(itemGeneral.DocumentName, pages);

            SetUp_ItemRepository_ImageElementDetailsAsync(_multiPageAssortedItemDetails);
            SetUp_ItemRepository_DocumentNameAsync(itemGeneral);
            SetUp_ImageVault_FrontImageAsync(FrontPngImage);
            SetUp_ImageVault_BackImageAsync(BackPngImage);
            SetUp_ImageConverter_ConvertToPng(FrontPngImage, FrontPngImage);
            SetUp_ImageConverter_ConvertToPng(BackPngImage, BackPngImage);

            // Act
            var result = await _service.GetGeneralDocumentByIrnAsync(irn, seqNum, userId, tenantId, startInsertTime, endInsertTime);

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
            var seqNum = 1;
            var userId = new UserId(12345);
            var startInsertTime = DateTime.Now.AddMonths(-26);
            var endInsertTime = DateTime.Now;
            var pages = new List<GeneralDocumentPage>
            {
                new GeneralDocumentPage(0, FrontPngImage, BackPngImage),
                new GeneralDocumentPage(1, FrontPngImage, BackPngImage),
                new GeneralDocumentPage(2, FrontPngImage, BackPngImage)
            };

            var itemGeneral = new GeneralDocumentItemDetails(1, string.Empty, 2, "Owner");

            var expectedGeneralDocument = new GeneralDocument(itemGeneral.DocumentName, pages);

            SetUp_ItemRepository_ImageElementDetailsAsync(_multiPageTwoSidedItemDetails);
            SetUp_ItemRepository_DocumentNameAsync(itemGeneral);
            SetUp_ImageVault_FrontImageAsync(FrontPngImage);
            SetUp_ImageVault_BackImageAsync(BackPngImage);
            SetUp_ImageConverter_ConvertToPng(FrontPngImage, FrontPngImage);
            SetUp_ImageConverter_ConvertToPng(BackPngImage, BackPngImage);

            // Act
            var result = await _service.GetGeneralDocumentByIrnAsync(irn, seqNum, userId, tenantId, startInsertTime, endInsertTime);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(expectedGeneralDocument, result.Value);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async void GetGeneralDocumentByIrnAsync_PageIsMissingFrontImage_ReturnsNotFound()
        {
            // Arrange
            var tenantId = "Default";
            var irn = new IrnId("testirn");
            var seqNum = 1;
            var userId = new UserId(12345);
            var startInsertTime = DateTime.Now.AddMonths(-26);
            var endInsertTime = DateTime.Now;

            SetUp_ItemRepository_ImageElementDetailsAsync(_multiPageTwoSidedItemDetails);
            SetUp_ImageVault_FrontImageMissingAsync();

            // Act
            var result = await _service.GetGeneralDocumentByIrnAsync(irn, seqNum, userId, tenantId, startInsertTime, endInsertTime);

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
            var seqNum = 1;
            var userId = new UserId(12345);
            var startInsertTime = DateTime.Now.AddMonths(-26);
            var endInsertTime = DateTime.Now;

            SetUp_ItemRepository_ImageElementDetailsAsync(_multiPageTwoSidedItemDetails);
            SetUp_ImageVault_FrontImageAsync(FrontPngImage);
            SetUp_ImageConverter_ConvertToPng(FrontPngImage, FrontPngImage);
            SetUp_ImageVault_BackImageMissingAsync();

            // Act
            var result = await _service.GetGeneralDocumentByIrnAsync(irn, seqNum, userId, tenantId, startInsertTime, endInsertTime);

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
            var seqNum = 1;
            var userId = new UserId(12345);
            var startInsertTime = DateTime.Now.AddMonths(-26);
            var endInsertTime = DateTime.Now;

            SetUp_ItemRepository_ImageElementDetailsAsync(_emptyDetails);

            // Act
            var result = await _service.GetGeneralDocumentByIrnAsync(irn, seqNum, userId, tenantId, startInsertTime, endInsertTime);

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
            var seqNum = 1;
            var userId = new UserId(12345);
            var startInsertTime = DateTime.Now.AddMonths(-26);
            var endInsertTime = DateTime.Now;

            SetUp_ItemRepository_ImageElementDetailsAsync(_multiPageTwoSidedItemDetails);
            SetUp_ImageVault_FrontImageAsync(FrontPngImage);
            SetUp_ImageVault_BackImageAsync(BackPngImage);
            SetUp_ImageConverter_ConvertToPng(FrontPngImage, new Error("Conversion failure."));
            SetUp_ImageConverter_ConvertToPng(BackPngImage, BackPngImage);

            // Act
            var result = await _service.GetGeneralDocumentByIrnAsync(irn, seqNum, userId, tenantId, startInsertTime, endInsertTime);

            // Assert
            Assert.True(result.IsFailure);
            Assert.IsType<Error>(result.FailureValue);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async void GetGeneralDocumentByIrnAsync_ExceptionDuringExecution_ReturnsServiceFailure()
        {
            // Arrange
            var tenantId = "Default";
            var irn = new IrnId("testirn");
            var seqNum = 1;
            var userId = new UserId(12345);
            var startInsertTime = DateTime.Now.AddMonths(-26);
            var endInsertTime = DateTime.Now;

            _itemRepository.GetImageElementDetailsByIrn(Arg.Any<IrnId>(), Arg.Any<int>()).Throws(new Exception("Service failure."));

            // Act
            var result = await _service.GetGeneralDocumentByIrnAsync(irn, seqNum, userId, tenantId, startInsertTime, endInsertTime);

            // Assert
            Assert.True(result.IsFailure);
            Assert.IsType<ServiceFailure>(result.FailureValue);
        }

        [Theory]
        [MemberData(nameof(GetImagesByIrnAsyncInvalidArguments))]
        [Trait("Category", "Unit")]
        public async Task GetGeneralDocumentByIrnAsync_InvalidArguments_ThrowsException(IrnId irn, UserId userId, string tenantId, DateTime startInsertTime, DateTime endInsertTime)
        {
            // Arrange
            var seqNum = 1;

            // Act, Assert
            await Assert.ThrowsAnyAsync<Exception>(() => _service.GetGeneralDocumentByIrnAsync(irn, seqNum, userId, tenantId, startInsertTime, endInsertTime));
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async void GetImageFromVaultAsync_TargetHostSetToItms_ImageVaultCallsItms()
        {
            // Arrange
            var tenantId = "Default";
            var service = new ItmsItemService(_requestDataAccessor, _logger, _imageVaultClient, _itemRepository, _imageConverter);
            var irn = new IrnId("testirn");
            var seqNum = 1;
            var userId = new UserId(12345);

            SetUp_ImageVault_FrontImageMissingAsync();

            // Act
            var _ = await service.GetImageFromVaultAsync(irn, seqNum, ImageSurface.Front, 0, userId, tenantId);

            // Assert
            await _imageVaultClient.Received(1).GetImageForLegacy(
                Arg.Any<RequestIdentifier>(),
                LegacyTarget.Itms,
                Arg.Any<string>(),
                Arg.Any<UserId>(),
                Arg.Any<IrnId>(),
                Arg.Any<int>(),
                Arg.Any<ImageSurface>(),
                Arg.Any<int>());
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
            _itemRepository.GetImageElementDetailsByIrnAsync(Arg.Any<IrnId>(), Arg.Any<int>(), Arg.Any<DateTime>(), Arg.Any<DateTime>())
                .Returns(result);
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
