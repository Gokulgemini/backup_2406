using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using NSubstitute;
using RDM.Messaging;
using RDM.Messaging.ImageVault;
using RDM.Model.ImageVault;
using RDM.Model.Itms;
using Xunit;

namespace RDM.Client.ImageVault.Tests
{
    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Test files are not styled.")]
    [SuppressMessage("StyleCop.CSharp.LayoutRules", "*", Justification = "Test files are not styled.")]
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "*", Justification = "Test files are not styled.")]
    [SuppressMessage("StyleCop.CSharp.OrderingRules", "*", Justification = "Test files are not styled.")]
    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "*", Justification = "Test files are not styled.")]
    [SuppressMessage("StyleCop.CSharp.SpacingRules", "*", Justification = "Test files are not styled.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "Test files are not styled.")]
    public class ImageVaultClientUnitTests
    {
        private readonly ImageVaultClient _client;
        private readonly IMessageQueue _queue;

        private readonly byte[] _content;
        private readonly string _mimeType;
        private readonly RequestIdentifier _requestId;
        private readonly ImageId _imageId;

        public ImageVaultClientUnitTests()
        {
            _queue = Substitute.For<IMessageQueue>();
            _client = new ImageVaultClient(_queue);

            _content = new byte[] { 1, 3, 5 };
            _mimeType = "image/jpeg";
            _requestId = new RequestIdentifier("030833e9ffb84badab5f938b0dc9925f");
            _imageId = new ImageId("08a5971d949a4204be8b0f0e2eabc3ba");
        }

        // Tech debt made to get GRPC unit tests working ITMSAPI-7321
        //public static IEnumerable<object[]> AddImageInvalidArguments()
        //{
        //    var requestId = new RequestIdentifier("030833e9ffb84badab5f938b0dc9925f");
        //    var content = new byte[] { 1, 2, 3, 4 };
        //    var mimeType = "image/jpeg";
        //    var expiresOn = new DateTimeOffset(2017, 3, 23, 14, 08, 33, TimeSpan.FromHours(-5));

        //    return new List<object[]>
        //    {
        //        new object[] { null, content, mimeType, expiresOn },
        //        new object[] { RequestIdentifier.Empty, content, mimeType, expiresOn },
        //        new object[] { requestId, null, mimeType, expiresOn },
        //        new object[] { requestId, new byte[] { }, mimeType, expiresOn },
        //        new object[] { requestId, content, string.Empty, expiresOn }
        //    };
        //}

        //[Theory, MemberData(nameof(AddImageInvalidArguments))]
        //[Trait("Category", "Unit")]
        //public void AddImage_InvalidArguments_ThrowsException(
        //    RequestIdentifier requestId,
        //    byte[] content,
        //    string mimeType,
        //    DateTimeOffset expiresOn)
        //{
        //    // Arrange

        //    // Act
        //    var exception = Record.Exception(() => _client.AddImage(requestId, content, mimeType, expiresOn));

        //    // Assert
        //    Assert.NotNull(exception);
        //    _queue.Received(0).RequestReply<RequestAddImageMessage, ReplyAddImageMessage>(Arg.Any<RequestAddImageMessage>());
        //}

        //public static IEnumerable<object[]> AddImageInvalidStreamArguments()
        //{
        //    var requestId = new RequestIdentifier("030833e9ffb84badab5f938b0dc9925f");
        //    var bytes = new byte[] { 1, 2, 3, 4 };
        //    var content = new MemoryStream(bytes);
        //    var mimeType = "image/jpeg";
        //    var expiresOn = new DateTimeOffset(2017, 3, 23, 14, 08, 33, TimeSpan.FromHours(-5));

        //    return new List<object[]>
        //    {
        //        new object[] { null, content, mimeType, expiresOn },
        //        new object[] { RequestIdentifier.Empty, content, mimeType, expiresOn },
        //        new object[] { requestId, null, mimeType, expiresOn },
        //        new object[] { requestId, new MemoryStream(new byte[] { }), mimeType, expiresOn },
        //        new object[] { requestId, content, string.Empty, expiresOn }
        //    };
        //}

        //[Theory, MemberData(nameof(AddImageInvalidStreamArguments))]
        //[Trait("Category", "Unit")]
        //public void AddImage_InvalidStreamArguments_ThrowsException(
        //    RequestIdentifier requestId,
        //    Stream content,
        //    string mimeType,
        //    DateTimeOffset expiresOn)
        //{
        //    // Arrange

        //    // Act
        //    var exception = Record.Exception(() => _client.AddImage(requestId, content, mimeType, expiresOn));

        //    // Assert
        //    Assert.NotNull(exception);
        //    _queue.Received(0).RequestReply<RequestAddImageMessage, ReplyAddImageMessage>(Arg.Any<RequestAddImageMessage>());
        //}

        //[Fact]
        //[Trait("Category", "Unit")]
        //public void AddImage_ReplyStatusSuccess_ReturnsExpectedImageId()
        //{
        //    // Arrange
        //    var imageId = new ImageId("589ecab9fef749dc9095074a836674d9");
        //    SetupAddImageReturn(AddImageStatus.Success, imageId);

        //    // Act
        //    var result = _client.AddImage(_requestId, _content, _mimeType);

        //    // Assert
        //    Assert.Equal(imageId, result);
        //    _queue.Received(1).RequestReply<RequestAddImageMessage, ReplyAddImageMessage>(Arg.Any<RequestAddImageMessage>());
        //}

        //[Fact]
        //[Trait("Category", "Unit")]
        //public void AddImage_ReplyStatusFailure_ReturnsNull()
        //{
        //    // Arrange
        //    SetupAddImageReturn(AddImageStatus.Failure);

        //    // Act
        //    var result = _client.AddImage(_requestId, _content, _mimeType);

        //    // Assert
        //    Assert.Null(result);
        //    _queue.Received(1).RequestReply<RequestAddImageMessage, ReplyAddImageMessage>(Arg.Any<RequestAddImageMessage>());
        //}

        //public static IEnumerable<object[]> AddTiffInvalidArguments()
        //{
        //    var requestId = new RequestIdentifier("030833e9ffb84badab5f938b0dc9925f");
        //    var content = new byte[] { 1, 2, 3, 4 };
        //    var expiresOn = new DateTimeOffset(2017, 3, 23, 14, 08, 33, TimeSpan.FromHours(-5));

        //    return new List<object[]>
        //    {
        //        new object[] { null, content, expiresOn },
        //        new object[] { RequestIdentifier.Empty, content, expiresOn },
        //        new object[] { requestId, null, expiresOn },
        //        new object[] { requestId, new byte[] { }, expiresOn }
        //    };
        //}

        //[Theory, MemberData(nameof(AddTiffInvalidArguments))]
        //[Trait("Category", "Unit")]
        //public void AddTiff_InvalidArguments_ThrowsException(
        //    RequestIdentifier requestId,
        //    byte[] content,
        //    DateTimeOffset expiresOn)
        //{
        //    // Arrange

        //    // Act
        //    var exception = Record.Exception(() => _client.AddTiff(requestId, content, expiresOn));

        //    // Assert
        //    Assert.NotNull(exception);
        //    _queue.Received(0).RequestReply<RequestAddTiffMessage, ReplyAddTiffMessage>(Arg.Any<RequestAddTiffMessage>());
        //}

        //[Fact]
        //[Trait("Category", "Unit")]
        //public void AddTiff_ReplyStatusSuccess_ReturnsExpectedImageId()
        //{
        //    // Arrange
        //    var imageId = new ImageId("589ecab9fef749dc9095074a836674d9");
        //    SetupAddTiffReturn(AddImageStatus.Success, imageId);

        //    // Act
        //    var result = _client.AddTiff(_requestId, _content);

        //    // Assert
        //    Assert.Equal(imageId, result);
        //    _queue.Received(1).RequestReply<RequestAddTiffMessage, ReplyAddTiffMessage>(Arg.Any<RequestAddTiffMessage>());
        //}

        //[Fact]
        //[Trait("Category", "Unit")]
        //public void AddTiff_ReplyStatusFailure_ReturnsNull()
        //{
        //    // Arrange
        //    SetupAddTiffReturn(AddImageStatus.Failure, null);

        //    // Act
        //    var result = _client.AddTiff(_requestId, _content);

        //    // Assert
        //    Assert.Null(result);
        //    _queue.Received(1).RequestReply<RequestAddTiffMessage, ReplyAddTiffMessage>(Arg.Any<RequestAddTiffMessage>());
        //}

        //public static IEnumerable<object[]> GetImageInvalidArguments()
        //{
        //    var requestId = new RequestIdentifier("030833e9ffb84badab5f938b0dc9925f");
        //    var imageId = new ImageId("589ecab9fef749dc9095074a836674d9");

        //    return new List<object[]>
        //    {
        //        new object[] { null, imageId },
        //        new object[] { RequestIdentifier.Empty, imageId },
        //        new object[] { requestId, null },
        //        new object[] { requestId, ImageId.Empty }
        //    };
        //}

        //[Theory, MemberData(nameof(GetImageInvalidArguments))]
        //[Trait("Category", "Unit")]
        //public void GetImage_InvalidArguments_ThrowsException(RequestIdentifier requestId, ImageId imageId)
        //{
        //    // Arrange

        //    // Act
        //    var exception = Record.Exception(() => _client.GetImage(requestId, imageId));

        //    // Assert
        //    Assert.NotNull(exception);
        //    _queue.Received(0).RequestReply<RequestGetImageMessage, ReplyGetImageMessage>(Arg.Any<RequestGetImageMessage>());
        //}

        //[Fact]
        //[Trait("Category", "Unit")]
        //public void GetImage_ReplyStatusSuccess_ReturnsExpectedImage()
        //{
        //    // Arrange
        //    var image = new Image(_content, _mimeType, 10, 10);
        //    SetupGetImageReturn(GetImageStatus.Success, image);

        //    // Act
        //    var result = _client.GetImage(_requestId, _imageId);

        //    // Assert
        //    Assert.Equal(image, result);
        //    _queue.Received(1).RequestReply<RequestGetImageMessage, ReplyGetImageMessage>(Arg.Any<RequestGetImageMessage>());
        //}

        //[Fact]
        //[Trait("Category", "Unit")]
        //public void GetImage_StatusFailure_ReturnsNull()
        //{
        //    // Arrange
        //    SetupGetImageReturn(GetImageStatus.Failure, null);

        //    // Act
        //    var result = _client.GetImage(_requestId, _imageId);

        //    // Assert
        //    _queue.Received(1).RequestReply<RequestGetImageMessage, ReplyGetImageMessage>(Arg.Any<RequestGetImageMessage>());
        //    Assert.Equal(default, result);
        //}

        //public static IEnumerable<object[]> GetImageByIrnInvalidArguments()
        //{
        //    var requestId = new RequestIdentifier("030833e9ffb84badab5f938b0dc9925f");
        //    var irn = new IrnId("EYMG6NTRKU80C04SW4CWWGKW8");
        //    var tenantId = "Default";
        //    var surface = ImageSurface.Front;
        //    var page = 0;

        //    return new List<object[]>
        //    {
        //        new object[] { null, irn, surface, page, tenantId },
        //        new object[] { RequestIdentifier.Empty, irn, surface, page, tenantId },
        //        new object[] { requestId, null, surface, page, tenantId },
        //        new object[] { requestId, IrnId.Empty, surface, page, tenantId },
        //        new object[] { requestId, irn, surface, page, null },
        //        new object[] { requestId, irn, surface, page, string.Empty }
        //    };
        //}

        //[Theory]
        //[MemberData(nameof(GetImageByIrnInvalidArguments))]
        //[Trait("Category", "Unit")]
        //public void GetImageByIrn_InvalidArguments_ThrowsException(
        //    RequestIdentifier requestId,
        //    IrnId irn,
        //    ImageSurface surface,
        //    int page,
        //    string tenantId)
        //{
        //    // Arrange
        //    // Act, Assert
        //    Assert.ThrowsAny<Exception>(() => _client.GetImageByIrn(requestId, irn, surface, page, tenantId));
        //}

        //[Fact]
        //[Trait("Category", "Unit")]
        //public void GetImageByIrn_ReplyStatusSuccess_ReturnsExpectedImage()
        //{
        //    // Arrange
        //    var irn = new IrnId("EYMG6NTRKU80C04SW4CWWGKW8");
        //    var surface = ImageSurface.Front;
        //    var tenantId = "Default";
        //    var page = 0;

        //    var image = new Image(_content, _mimeType, 10, 10);
        //    SetupGetImageByIrnReturn(GetImageStatus.Success, image);

        //    // Act
        //    var result = _client.GetImageByIrn(_requestId, irn, surface, page, tenantId);

        //    // Assert
        //    Assert.Equal(image, result);
        //}

        //[Fact]
        //[Trait("Category", "Unit")]
        //public void GetImageByIrn_StatusFailure_ReturnsNull()
        //{
        //    // Arrange
        //    var irn = new IrnId("EYMG6NTRKU80C04SW4CWWGKW8");
        //    var surface = ImageSurface.Front;
        //    var tenantId = "Default";
        //    var page = 0;

        //    SetupGetImageByIrnReturn(GetImageStatus.Failure, null);

        //    // Act
        //    var result = _client.GetImageByIrn(_requestId, irn, surface, page, tenantId);

        //    // Assert
        //    Assert.Equal(default, result);
        //}

        //public static IEnumerable<object[]> GetImageForLegacyInvalidArguments()
        //{
        //    var requestId = new RequestIdentifier("030833e9ffb84badab5f938b0dc9925f");
        //    var tenantId = "Default";
        //    var legacyTarget = LegacyTarget.Itms;
        //    var userId = new UserId(1);
        //    var irn = new IrnId("EYMG6NTRKU80C04SW4CWWGKW8");
        //    var seqNum = 1;
        //    var surface = ImageSurface.Front;
        //    var page = 0;

        //    return new List<object[]>
        //    {
        //        new object[] { null, legacyTarget, tenantId, userId, irn, seqNum, surface, page },
        //        new object[] { RequestIdentifier.Empty, legacyTarget, tenantId, userId, irn, seqNum, surface, page },
        //        new object[] { requestId, legacyTarget, tenantId, userId, null, seqNum, surface, page },
        //        new object[] { requestId, legacyTarget, tenantId, userId, IrnId.Empty, seqNum, surface, page },
        //        new object[] { requestId, legacyTarget, tenantId, null, irn, seqNum, surface, page },
        //        new object[] { requestId, legacyTarget, tenantId, UserId.InvalidUser, irn, seqNum, surface, page }
        //    };
        //}

        //[Theory]
        //[MemberData(nameof(GetImageForLegacyInvalidArguments))]
        //[Trait("Category", "Unit")]
        //public async Task GetImageForLegacy_InvalidArguments_ThrowsException(
        //    RequestIdentifier requestId,
        //    LegacyTarget legacyTarget,
        //    string tenantId,
        //    UserId userId,
        //    IrnId irn,
        //    int seqNum,
        //    ImageSurface surface,
        //    int page)
        //{
        //    // Arrange
        //    // Act, Assert
        //    await Assert.ThrowsAnyAsync<Exception>(
        //        () => _client.GetImageForLegacy(requestId, legacyTarget, tenantId, userId, irn, seqNum, surface, page));
        //}

        //[Fact]
        //[Trait("Category", "Unit")]
        //public void GetImageForLegacy_ReplyStatusSuccess_ReturnsExpectedImage()
        //{
        //    // Arrange
        //    var legacyTarget = LegacyTarget.Itms;
        //    var tenantId = "Default";
        //    var userId = new UserId(1);
        //    var irn = new IrnId("EYMG6NTRKU80C04SW4CWWGKW8");
        //    var seqNum = 1;
        //    var surface = ImageSurface.Front;
        //    var page = 0;

        //    var image = new Image(_content, _mimeType, 10, 10);
        //    SetupGetImageForLegacyReturn(GetImageStatus.Success, image);

        //    // Act
        //    var result = _client.GetImageForLegacy(_requestId, legacyTarget, tenantId, userId, irn, seqNum, surface, page).Result;

        //    // Assert
        //    Assert.Equal(image, result);
        //}

        //[Fact]
        //[Trait("Category", "Unit")]
        //public void GetImageForLegacy_StatusFailure_ReturnsNull()
        //{
        //    // Arrange
        //    var legacyTarget = LegacyTarget.Itms;
        //    var tenantId = "Default";
        //    var userId = new UserId(1);
        //    var irn = new IrnId("EYMG6NTRKU80C04SW4CWWGKW8");
        //    var seqNum = 1;
        //    var surface = ImageSurface.Front;
        //    var page = 0;

        //    SetupGetImageForLegacyReturn(GetImageStatus.Failure, null);

        //    // Act
        //    var result = _client.GetImageForLegacy(_requestId, legacyTarget, tenantId, userId, irn, seqNum, surface, page).Result;

        //    // Assert
        //    Assert.Equal(default, result);
        //}

        //public static IEnumerable<object[]> GetImageAsJpegInvalidArguments()
        //{
        //    var requestId = new RequestIdentifier("030833e9ffb84badab5f938b0dc9925f");
        //    var imageId = new ImageId("589ecab9fef749dc9095074a836674d9");

        //    return new List<object[]>
        //    {
        //        new object[] { null, imageId },
        //        new object[] { RequestIdentifier.Empty, imageId },
        //        new object[] { requestId, null },
        //        new object[] { requestId, ImageId.Empty }
        //    };
        //}

        //[Theory, MemberData(nameof(GetImageAsJpegInvalidArguments))]
        //[Trait("Category", "Unit")]
        //public void GetImageAsJpeg_InvalidArguments_ThrowsException(RequestIdentifier requestId, ImageId imageId)
        //{
        //    // Arrange

        //    // Act
        //    var exception = Record.Exception(() => _client.GetImageAsJpeg(requestId, imageId));

        //    // Assert
        //    Assert.NotNull(exception);
        //    _queue.Received(0)
        //          .RequestReply<RequestGetImageAsJpegMessage, ReplyGetImageAsJpegMessage>(
        //              Arg.Any<RequestGetImageAsJpegMessage>());
        //}

        //[Fact]
        //[Trait("Category", "Unit")]
        //public void GetImageAsJpeg_ReplyStatusSuccess_ReturnsExpectedImage()
        //{
        //    // Arrange
        //    var image = new Image(_content, "image/jpeg", 10, 10);
        //    SetupGetImageAsJpegReturn(GetImageStatus.Success, image);

        //    // Act
        //    var result = _client.GetImageAsJpeg(_requestId, _imageId);

        //    // Assert
        //    Assert.Equal(image, result);
        //    _queue.Received(1)
        //          .RequestReply<RequestGetImageAsJpegMessage, ReplyGetImageAsJpegMessage>(
        //              Arg.Any<RequestGetImageAsJpegMessage>());
        //}

        //[Fact]
        //[Trait("Category", "Unit")]
        //public void GetImageAsJpeg_StatusFailure_ReturnsNull()
        //{
        //    // Arrange
        //    SetupGetImageAsJpegReturn(GetImageStatus.Failure, null);

        //    // Act
        //    var result = _client.GetImageAsJpeg(_requestId, _imageId);

        //    // Assert
        //    _queue.Received(1)
        //          .RequestReply<RequestGetImageAsJpegMessage, ReplyGetImageAsJpegMessage>(
        //              Arg.Any<RequestGetImageAsJpegMessage>());
        //    Assert.Equal(default, result);
        //}

        //public static IEnumerable<object[]> WriteImageToWebClientInvalidArguments()
        //{
        //    var requestId = new RequestIdentifier("030833e9ffb84badab5f938b0dc9925f");
        //    var imageId = new ImageId("589ecab9fef749dc9095074a836674d9");
        //    var filepath = "C:\\Temp\\";
        //    var filename = "IRN_SeqNum_SheetNumber_Surface.tif";
        //    var tenantId = "Default";

        //    return new List<object[]>
        //    {
        //        new object[] { null, tenantId, imageId, filepath, filename },
        //        new object[] { RequestIdentifier.Empty, tenantId, imageId, filepath, filename },
        //        new object[] { requestId, tenantId, null, filepath, filename },
        //        new object[] { requestId, tenantId, ImageId.Empty, filepath, filename },
        //        new object[] { requestId, tenantId, imageId, null, filename },
        //        new object[] { requestId, tenantId, imageId, string.Empty, filename },
        //        new object[] { requestId, tenantId, imageId, "           ", filename },
        //        new object[] { requestId, tenantId, imageId, filepath, null },
        //        new object[] { requestId, tenantId, imageId, filepath, string.Empty },
        //        new object[] { requestId, tenantId, imageId, filepath, "           " }
        //    };
        //}

        //[Theory, MemberData(nameof(WriteImageToWebClientInvalidArguments))]
        //[Trait("Category", "Unit")]
        //public void WriteImageToWebClient_InvalidArguments_ThrowsException(
        //    RequestIdentifier requestId,
        //    string tenantId,
        //    ImageId imageId,
        //    string filepath,
        //    string filename)
        //{
        //    // Arrange

        //    // Act
        //    var exception = Record.Exception(
        //        () => _client.WriteImageToWebClient(requestId, tenantId, imageId, filepath, filename));

        //    // Assert
        //    Assert.NotNull(exception);
        //    _queue.Received(0)
        //          .RequestReply<RequestWriteImageToWebClientMessage, ReplyWriteImageToWebClientMessage>(
        //              Arg.Any<RequestWriteImageToWebClientMessage>());
        //}

        //[Fact]
        //[Trait("Category", "Unit")]
        //public void WriteImageToWebClient_ReplyStatusSuccess_ReturnsExpectedImageInfo()
        //{
        //    // Arrange
        //    var imageId = new ImageId("97c921f0ecfc411b9997bd6720ceb26b");
        //    var filepath = "C:\\Temp\\";
        //    var fileName = "testFileName";
        //    var tenantId = "Default";
        //    var imageUrl = "testImageUrl";
        //    var tiffSize = 168961;
        //    var tiffWidth = 1178;
        //    var tiffHeight = 544;
        //    var imageInfo = new ImageTiffInfo(imageId, fileName, imageUrl, tiffSize, tiffWidth, tiffHeight);

        //    SetupWriteImageToWebClientImageInfoReturn(WriteImageToWebClientStatus.Success, imageInfo);

        //    // Act
        //    var result = _client.WriteImageToWebClient(_requestId, tenantId, _imageId, filepath, fileName);

        //    // Assert
        //    Assert.Equal(imageInfo, result);
        //    _queue.Received(1)
        //          .RequestReply<RequestWriteImageToWebClientMessage, ReplyWriteImageToWebClientMessage>(
        //              Arg.Any<RequestWriteImageToWebClientMessage>());
        //}

        //[Fact]
        //[Trait("Category", "Unit")]
        //public void WriteImageToWebClient_ReplyStatusFailure_ReturnsNull()
        //{
        //    // Arrange
        //    var filepath = "C:\\Temp\\";
        //    var filename = "IRN_SeqNum_SheetNumber_Surface.tif";
        //    var tenantId = "Default";

        //    SetupWriteImageToWebClientImageInfoReturn(WriteImageToWebClientStatus.Failure, null);

        //    // Act
        //    var result = _client.WriteImageToWebClient(_requestId, tenantId, _imageId, filepath, filename);

        //    // Assert
        //    Assert.Null(result);
        //    _queue.Received(1)
        //          .RequestReply<RequestWriteImageToWebClientMessage, ReplyWriteImageToWebClientMessage>(
        //              Arg.Any<RequestWriteImageToWebClientMessage>());
        //}

        [Fact]
        [Trait("Category", "Unit")]
        public void RemoveImage_NullImageId_ThrowsException()
        {
            // Arrange

            // Act
            var exception = Record.Exception(() => _client.RemoveImage(_requestId, null));

            // Assert
            Assert.NotNull(exception);
            Assert.IsType<ArgumentNullException>(exception);
            _queue.Received(0).Publish(Arg.Any<RemoveImageMessage>());
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void RemoveImage_NullRequestId_ThrowsException()
        {
            // Arrange

            // Act
            var exception = Record.Exception(() => _client.RemoveImage(null, _imageId));

            // Assert
            Assert.NotNull(exception);
            Assert.IsType<ArgumentNullException>(exception);
            _queue.Received(0).Publish(Arg.Any<RemoveImageMessage>());
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void RemoveImage_EmptyRequestId_ThrowsException()
        {
            // Arrange

            // Act
            var exception = Record.Exception(() => _client.RemoveImage(RequestIdentifier.Empty, _imageId));

            // Assert
            Assert.NotNull(exception);
            Assert.IsType<ArgumentException>(exception);
            _queue.Received(0).Publish(Arg.Any<RemoveImageMessage>());
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void RemoveImage_ValidParameters_GeneratesMessage()
        {
            // Arrange
            SetupRemoveImageReturn();

            // Act
            _client.RemoveImage(_requestId, _imageId);

            // Assert
            _queue.Received(1).Publish(Arg.Any<RemoveImageMessage>());
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void VerifyImageSize_NullImageId_ThrowsException()
        {
            // Arrange

            // Act
            var exception = Record.Exception(() => _client.VerifyImageSize(_requestId, null));

            // Assert
            Assert.NotNull(exception);
            Assert.IsType<ArgumentNullException>(exception);
            _queue.Received(0).Publish(Arg.Any<VerifyImageSizeMessage>());
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void VerifyImageSize_NullRequestId_ThrowsException()
        {
            // Arrange

            // Act
            var exception = Record.Exception(() => _client.VerifyImageSize(null, _imageId));

            // Assert
            Assert.NotNull(exception);
            Assert.IsType<ArgumentNullException>(exception);
            _queue.Received(0).Publish(Arg.Any<VerifyImageSizeMessage>());
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void VerifyImageSize_EmptyRequestId_ThrowsException()
        {
            // Arrange

            // Act
            var exception = Record.Exception(() => _client.VerifyImageSize(RequestIdentifier.Empty, _imageId));

            // Assert
            Assert.NotNull(exception);
            Assert.IsType<ArgumentException>(exception);
            _queue.Received(0).Publish(Arg.Any<VerifyImageSizeMessage>());
        }

        private void SetupAddImageReturn(AddImageStatus status = AddImageStatus.Success, ImageId imageId = null)
        {
            if (imageId == null)
            {
                imageId = new ImageId("08a5971d949a4204be8b0f0e2eabc3ba");
            }

            var reply = new ReplyAddImageMessage(status, imageId);
            _queue.RequestReply<RequestAddImageMessage, ReplyAddImageMessage>(Arg.Any<RequestAddImageMessage>()).Returns(reply);
        }

        private void SetupAddTiffReturn(AddImageStatus status, ImageId imageId)
        {
            var reply = new ReplyAddTiffMessage(status, imageId);
            _queue.RequestReply<RequestAddTiffMessage, ReplyAddTiffMessage>(Arg.Any<RequestAddTiffMessage>()).Returns(reply);
        }

        private void SetupGetImageReturn(GetImageStatus status, Image image)
        {
            var reply = new ReplyGetImageMessage(status, image);
            _queue.RequestReply<RequestGetImageMessage, ReplyGetImageMessage>(Arg.Any<RequestGetImageMessage>()).Returns(reply);
        }

        private void SetupGetImageByIrnReturn(GetImageStatus status, Image image)
        {
            var reply = new ReplyGetImageByIrnMessage(status, image);
            _queue.RequestReply<RequestGetImageByIrnMessage, ReplyGetImageByIrnMessage>(Arg.Any<RequestGetImageByIrnMessage>())
                  .Returns(reply);
        }

        private void SetupGetImageForLegacyReturn(GetImageStatus status, Image image)
        {
            var reply = new ReplyGetImageForLegacyMessage(status, image);
            _queue.RequestReply<RequestGetImageForLegacyMessage, ReplyGetImageForLegacyMessage>(
                    Arg.Any<RequestGetImageForLegacyMessage>())
                .Returns(reply);
        }

        private void SetupGetImageAsJpegReturn(GetImageStatus status, Image image)
        {
            var reply = new ReplyGetImageAsJpegMessage(status, image);
            _queue.RequestReply<RequestGetImageAsJpegMessage, ReplyGetImageAsJpegMessage>(Arg.Any<RequestGetImageAsJpegMessage>())
                  .Returns(reply);
        }

        private void SetupWriteImageToWebClientImageInfoReturn(WriteImageToWebClientStatus status, ImageTiffInfo imageInfo)
        {
            var reply = new ReplyWriteImageToWebClientMessage(status, imageInfo);

            _queue.RequestReply<RequestWriteImageToWebClientMessage, ReplyWriteImageToWebClientMessage>(
                      Arg.Any<RequestWriteImageToWebClientMessage>())
                  .Returns(reply);
        }

        private void SetupRemoveImageReturn()
        {
            _queue.Publish(Arg.Any<RemoveImageMessage>());
        }
    }
}
