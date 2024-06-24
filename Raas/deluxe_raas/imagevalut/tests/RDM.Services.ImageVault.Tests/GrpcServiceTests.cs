using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Google.Protobuf;
using Grpc.Core;
using Grpc.Core.Testing;
using ImageVaultGrpc;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using RDM.Core;
using RDM.Data.ImageVault;
using RDM.Imaging;
using RDM.Messaging.ImageVault;
using RDM.Model.ImageVault;
using RDM.Model.Itms;
using RDM.Statistician.PerformanceLog;
using RDM.Statistician.PerformanceTimer;
using Serilog;
using Xunit;

namespace RDM.Services.ImageVault.Tests
{
    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Test files are not styled.")]
    [SuppressMessage("StyleCop.CSharp.LayoutRules", "*", Justification = "Test files are not styled.")]
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "*", Justification = "Test files are not styled.")]
    [SuppressMessage("StyleCop.CSharp.OrderingRules", "*", Justification = "Test files are not styled.")]
    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "*", Justification = "Test files are not styled.")]
    [SuppressMessage("StyleCop.CSharp.SpacingRules", "*", Justification = "Test files are not styled.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "Test files are not styled.")]

    public class GrpcServiceTests
    {
        private readonly ILogger _logger;
        private readonly IPerformanceLogger _performanceLogger;
        private readonly IRequestDataAccessor _requestDataAccessor;
        private readonly IImageRepository _imageRepository;
        private readonly ILegacyImageAccess _legacyImageAccess;
        private readonly IImageFactory _imageFactory;
        private readonly IImage _image;

        private readonly RequestIdentifier _requestId;
        private readonly GrpcService _service;

        public GrpcServiceTests()
        {
            _logger = Substitute.For<ILogger>();
            _performanceLogger = Substitute.For<IPerformanceLogger>();
            _requestDataAccessor = Substitute.For<IRequestDataAccessor>();
            _requestDataAccessor.RequestId.Returns(new RequestIdentifier("1"));
            _imageRepository = Substitute.For<IImageRepository>();
            _legacyImageAccess = Substitute.For<ILegacyImageAccess>();
            _imageFactory = Substitute.For<IImageFactory>();
            _image = Substitute.For<IImage>();
            _imageFactory.CreateImage(Arg.Any<byte[]>()).Returns(_image);

            _service = new GrpcService(
                _logger,
                _performanceLogger,
                _requestDataAccessor,
                _imageRepository,
                _legacyImageAccess,
                _imageFactory);

            _requestId = new RequestIdentifier("030833e9ffb84badab5f938b0dc9925f");
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void Ctor_ValidParameters_ObjectCreated()
        {
            // Arrange

            // Act
            var result = new GrpcService(
                _logger,
                _performanceLogger,
                _requestDataAccessor,
                _imageRepository,
                _legacyImageAccess,
                _imageFactory);

            // Assert
            Assert.NotNull(result);
        }

        public static IEnumerable<object[]> CtorInvalidArgumentsException()
        {
            var logger = Substitute.For<ILogger>();
            var performanceLogger = Substitute.For<IPerformanceLogger>();
            var requestDataAccessor = Substitute.For<IRequestDataAccessor>();
            requestDataAccessor.RequestId.Returns(new RequestIdentifier("1"));
            var legacyImageAccess = Substitute.For<ILegacyImageAccess>();
            var imageFactory = Substitute.For<IImageFactory>();

            return new List<object[]>
            {
                new object[]
                {
                    null,
                    performanceLogger,
                    requestDataAccessor,
                    legacyImageAccess,
                    imageFactory
                },
                new object[]
                {
                    logger,
                    null,
                    requestDataAccessor,
                    legacyImageAccess,
                    imageFactory
                },
                new object[]
                {
                    logger,
                    performanceLogger,
                    null,
                    legacyImageAccess,
                    imageFactory
                },
                new object[]
                {
                    logger,
                    performanceLogger,
                    requestDataAccessor,
                    null,
                    imageFactory
                },
                new object[]
                {
                    logger,
                    performanceLogger,
                    requestDataAccessor,
                    legacyImageAccess,
                    null
                }
            };
        }

        [Theory, MemberData(nameof(CtorInvalidArgumentsException))]
        [Trait("Category", "Unit")]
        public void Ctor_InvalidArguments_ThrowsException(
            ILogger logger,
            IPerformanceLogger performanceLogger,
            IRequestDataAccessor requestDataAccessor,
            ILegacyImageAccess legacyImageAccess,
            IImageFactory imageFactory)
        {
            // Arrange

            // Act
            var exception = Record.Exception(
                () => new GrpcService(
                    logger,
                    performanceLogger,
                    requestDataAccessor,
                    _imageRepository,
                    legacyImageAccess,
                    imageFactory));

            // Assert
            Assert.NotNull(exception);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async void GetImage_ValidParams_HandlesRequestContextAndPerformance()
        {
            // Arrange
            var imageId = new ImageId("374577641fdf4a2e98a4ef91ca4e9233");
            var request = new GetImageRequest
            {
                RequestId = _requestId.Value,
                ImageId = imageId.Value,
                Width = 10
            };

            // Act
            var context = Mock_ServerCallContext("GetImage");
            var _ = await Record.ExceptionAsync(async () => await _service.GetImage(request, context));

            // Assert
            _requestDataAccessor.Received(1).RequestId = new RequestIdentifier(request.RequestId);
            _requestDataAccessor.Received(1).PerformanceMonitor = Arg.Is<IPerformanceTimer>(p => p != null);
            _performanceLogger.Received(1).Log(Arg.Any<IPerformanceTimer>(), Arg.Any<string>(), Arg.Any<string>());
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async void GetImage_NoResize_RawImageReturned()
        {
            // Arrange
            var imageId = new ImageId("374577641fdf4a2e98a4ef91ca4e9233");
            var dummyBytes = new byte[] { 0x01, 0x02, 0x03, 0x04 };
            var image = new Image(dummyBytes, "image/jpeg", 10, 10);
            var request = new GetImageRequest
            {
                RequestId = _requestId.Value,
                ImageId = imageId.Value,
                Width = 10
            };

            SetUp_RepoGetImage_ReturnsImage(imageId, image);

            // Act
            var context = Mock_ServerCallContext("GetImage");
            var reply = await _service.GetImage(request, context);

            // Assert
            Assert.True(StructuralComparisons.StructuralEqualityComparer.Equals(dummyBytes, reply.Content.ToByteArray()));
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async void GetImage_Resize_ResizedImageReturned()
        {
            // Arrange
            var imageId = new ImageId("2f081e58d50b49b0aa8a884faf674af3");
            var dummyBytes = new byte[] { 0x01, 0x02, 0x03, 0x04 };
            var image = new Image(dummyBytes, "image/jpeg", 20, 10);
            var request = new GetImageRequest
            {
                RequestId = _requestId.Value,
                ImageId = imageId.Value,
                Width = 10
            };

            SetUp_RepoGetImage_ReturnsImage(imageId, image);
            SetUp_Image_Properties(10, 5);
            // Act
            // Act
            var context = Mock_ServerCallContext("GetImage");
            var reply = await _service.GetImage(request, context);

            // Assert
            Assert.Equal(10, reply.Width);
            Assert.Equal(5, reply.Height);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async void GetImageAsJpeg_ValidParams_HandlesRequestContextAndPerformance()
        {
            // Arrange
            var imageId = new ImageId("374577641fdf4a2e98a4ef91ca4e9233");
            var request = new GetImageAsJpegRequest
            {
                RequestId = _requestId.Value,
                ImageId = imageId.Value,
                Width = 10
            };

            // Act
            var context = Mock_ServerCallContext("GetImageAsJpeg");
            var _ = await Record.ExceptionAsync(async () => await _service.GetImageAsJpeg(request, context));

            // Assert
            _requestDataAccessor.Received(1).RequestId = new RequestIdentifier(request.RequestId);
            _requestDataAccessor.Received(1).PerformanceMonitor = Arg.Is<IPerformanceTimer>(p => p != null);
            _performanceLogger.Received(1).Log(Arg.Any<IPerformanceTimer>(), Arg.Any<string>(), Arg.Any<string>());
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async void GetImageAsJpeg_AlreadyJpegNoResizeNeeded_RawImageReturned()
        {
            // Arrange
            var imageId = new ImageId("374577641fdf4a2e98a4ef91ca4e9233");
            var dummyBytes = new byte[] { 0x01, 0x02, 0x03, 0x04 };
            var image = new Image(dummyBytes, "image/jpeg", 10, 10);
            var request = new GetImageAsJpegRequest
            {
                RequestId = _requestId.Value,
                ImageId = imageId.Value,
                Width = 10
            };

            SetUp_RepoGetImage_ReturnsImage(imageId, image);

            // Act
            var context = Mock_ServerCallContext("GetImageAsJpeg");
            var reply = await _service.GetImageAsJpeg(request, context);

            // Assert
            Assert.True(StructuralComparisons.StructuralEqualityComparer.Equals(dummyBytes, reply.Content.ToByteArray()));
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async void GetImageAsJpeg_Resize_ResizedImageReturned()
        {
            // Arrange
            var imageId = new ImageId("2f081e58d50b49b0aa8a884faf674af3");
            var dummyBytes = new byte[] { 0x01, 0x02, 0x03, 0x04 };
            var image = new Image(dummyBytes, "image/jpeg", 20, 10);
            var request = new GetImageAsJpegRequest
            {
                RequestId = _requestId.Value,
                ImageId = imageId.Value,
                Width = 10
            };

            SetUp_RepoGetImage_ReturnsImage(imageId, image);
            SetUp_Image_Properties(20, 10);
            SetUp_Image_Resize(10, 5);

            // Act
            var context = Mock_ServerCallContext("GetImageAsJpeg");
            var reply = await _service.GetImageAsJpeg(request, context);

            // Assert
            Assert.Equal(10, reply.Width);
            Assert.Equal(5, reply.Height);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async void GetImageAsJpeg_NonJpegImageRetrieved_ConvertedToJpeg()
        {
            // Arrange
            var imageId = new ImageId("2f081e58d50b49b0aa8a884faf674af3");
            var dummyBytes = new byte[] { 0x01, 0x02, 0x03, 0x04 };
            var image = new Image(dummyBytes, "image/tiff", 5312, 2988);
            var request = new GetImageAsJpegRequest
            {
                RequestId = _requestId.Value,
                ImageId = imageId.Value,
                Width = image.Width
            };

            SetUp_RepoGetImage_ReturnsImage(imageId, image);
            SetUp_Image_Properties(format:ImageFormat.Tiff);
            SetUp_Image_SetFormat();

            // Act
            var context = Mock_ServerCallContext("GetImageAsJpeg");
            var reply = await _service.GetImageAsJpeg(request, context);

            // Assert
            Assert.Equal("image/jpeg", reply.MimeType);
            _image.Received(1).SetFormat(ImageFormat.Jpeg);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async void GetImageAsJpeg_NonJpegImageRetrieved_ConvertedToJpeg_NoResize()
        {
            // Arrange
            var imageId = new ImageId("2f081e58d50b49b0aa8a884faf674af3");
            var dummyBytes = new byte[] { 0x01, 0x02, 0x03, 0x04 };
            var width = 5312;
            var height = 2988;
            var image = new Image(dummyBytes, "image/tiff", width, height);
            var request = new GetImageAsJpegRequest
            {
                RequestId = _requestId.Value,
                ImageId = imageId.Value,
                Width = image.Width
            };

            SetUp_RepoGetImage_ReturnsImage(imageId, image);
            SetUp_Image_Properties(format: ImageFormat.Tiff);
            SetUp_Image_SetFormat();

            // Act
            var context = Mock_ServerCallContext("GetImageAsJpeg");
            var reply = await _service.GetImageAsJpeg(request, context);

            // Assert
            Assert.Equal("image/jpeg", reply.MimeType);
            _image.Received(1).SetFormat(ImageFormat.Jpeg);

        }

        [Fact]
        [Trait("Category", "Unit")]
        public async void GetImageByIrn_LegacyImageAccess_ReturnsImageWithResize()
        {
            // Arrange
            var tenantId = "Default";
            var irn = new IrnId("EYMG6NTRKU80C04SW4CWWGKW8");
            var imageId = new ImageId("2f081e58d50b49b0aa8a884faf674af3");
            var dummyBytes = new byte[] { 0x01, 0x02, 0x03, 0x04 };
            var width = 5312;
            var height = 2988;
            var image = new Image(dummyBytes, "image/tiff", width, height);
            var request = new GetImageByIrnRequest
            {
                RequestId = _requestId.Value,
                TenantId = tenantId,
                IrnId = irn.Value,
                Surface = ImageSurface.Front.ToString(),
                Page = 0,
                Width = 800
            };

            SetUp_Image_Properties(format: ImageFormat.Tiff);
            SetUp_Image_SetFormat();
            SetUp_Image_Resize(800, 448);
            SetUp_LegacyImageAccess_GetImageFromWebClient(image);

            // Act
            var context = Mock_ServerCallContext("GetImageByIrn");
            var reply = await _service.GetImageByIrn(request, context);


            // Assert
            Assert.Equal("image/jpeg", reply.MimeType);
            Assert.Equal(800, reply.Width);
            _image.Received(1).SetFormat(ImageFormat.Jpeg);
            _image.Received(1).ResizeToWidth(800);

        }

        [Fact]
        [Trait("Category", "Unit")]
        public async void GetImageByIrn_LegacyImageAccess_ReturnsImageWithoutResize()
        {
            // Arrange
            var tenantId = "Default";
            var irn = new IrnId("EYMG6NTRKU80C04SW4CWWGKW8");
            var dummyBytes = new byte[] { 0x01, 0x02, 0x03, 0x04 };
            var width = 5312;
            var height = 2988;
            var image = new Image(dummyBytes, "image/tiff", width, height);
            var request = new GetImageByIrnRequest
            {
                RequestId = _requestId.Value,
                TenantId = tenantId,
                IrnId = irn.Value,
                Surface = ImageSurface.Front.ToString(),
                Page = 0
            };
            SetUp_Image_Properties(format: ImageFormat.Tiff);
            SetUp_Image_SetFormat();
            SetUp_LegacyImageAccess_GetImageFromWebClient(image);

            // Act
            var context = Mock_ServerCallContext("GetImageByIrn");
            var reply = await _service.GetImageByIrn(request, context);

            // Assert
            Assert.Equal("image/jpeg", reply.MimeType);
            _image.Received(1).SetFormat(ImageFormat.Jpeg);
            _image.Received(0).ResizeToWidth(Arg.Any<int>());
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async void GetImageByIrn_LegacyImageAccessReturnsNotFound_ReturnsNotFound()
        {
            // Arrange
            var tenantId = "Default";
            var irn = new IrnId("EYMG6NTRKU80C04SW4CWWGKW8");
            var request = new GetImageByIrnRequest
            {
                RequestId = _requestId.Value,
                TenantId = tenantId,
                IrnId = irn.Value,
                Surface = ImageSurface.Front.ToString(),
                Page = 0
            };

            SetUp_LegacyImageAccess_GetImageFromWebClient(new NotFound("Could not fine the image."));

            // Act
            var context = Mock_ServerCallContext("GetImageByIrn");
            var exception = await Record.ExceptionAsync(async () => await _service.GetImageByIrn(request, context));

            // Assert
            Assert.IsType<RpcException>(exception);
            var rpc = exception as RpcException;
            Assert.Equal(StatusCode.NotFound, rpc.StatusCode);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async void GetImageByIrn_LegacyImageAccessReturnsServiceFailure_ReturnsFailure()
        {
            // Arrange
            var tenantId = "Default";
            var irn = new IrnId("EYMG6NTRKU80C04SW4CWWGKW8");
            var request = new GetImageByIrnRequest
            {
                RequestId = _requestId.Value,
                TenantId = tenantId,
                IrnId = irn.Value,
                Surface = ImageSurface.Front.ToString(),
                Page = 0
            };

            SetUp_LegacyImageAccess_GetImageFromWebClient(new ServiceFailure("Something broke."));

            // Act
            var context = Mock_ServerCallContext("GetImageByIrn");
            var exception = await Record.ExceptionAsync(async () => await _service.GetImageByIrn(request, context));

            // Assert
            Assert.IsType<RpcException>(exception);
            var rpc = exception as RpcException;
            Assert.Equal(StatusCode.Aborted, rpc.StatusCode);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async void GetImageByIrn_LegacyImageAccessThrowsException_ReturnsFailure()
        {
            // Arrange
            var tenantId = "Default";
            var irn = new IrnId("EYMG6NTRKU80C04SW4CWWGKW8");
            var request = new GetImageByIrnRequest
            {
                RequestId = _requestId.Value,
                TenantId = tenantId,
                IrnId = irn.Value,
                Surface = ImageSurface.Front.ToString(),
                Page = 0
            };

            _legacyImageAccess.GetImageFromWebClient(
                                  Arg.Any<string>(),
                                  Arg.Any<UserId>(),
                                  Arg.Any<IrnId>(),
                                  Arg.Any<int>(),
                                  Arg.Any<ImageSurface>(),
                                  Arg.Any<int>())
                              .Throws(new Exception(""));

            // Act
            var context = Mock_ServerCallContext("GetImageByIrn");
            var exception = await Record.ExceptionAsync(async () => await _service.GetImageByIrn(request, context));

            // Assert
            Assert.IsType<RpcException>(exception);
            var rpc = exception as RpcException;
            Assert.Equal(StatusCode.Aborted, rpc.StatusCode);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async void GetImageByIrn_LegacyTargetNotSupported_ReturnsFailure()
        {
            // Arrange
            var tenantId = "Default";
            var irn = new IrnId("EYMG6NTRKU80C04SW4CWWGKW8");
            var request = new GetImageByIrnRequest
            {
                RequestId = _requestId.Value,
                TenantId = tenantId,
                IrnId = irn.Value,
                Surface = ImageSurface.Front.ToString(),
                Page = 0
            };

            // Act
            var context = Mock_ServerCallContext("GetImageByIrn");
            var exception = await Record.ExceptionAsync(async () => await _service.GetImageByIrn(request, context));

            // Assert
            Assert.IsType<RpcException>(exception);
            var rpc = exception as RpcException;
            Assert.Equal(StatusCode.Aborted, rpc.StatusCode);
        }

        [Trait("Category", "Unit")]
        [Fact]
        public async void GetImageForLegacy_Itms_ReturnsCorrectImage()
        {
            // Arrange
            var tenantId = "Default";
            var userId = new UserId(1234);
            var irn = new IrnId("EYMG6NTRKU80C04SW4CWWGKW8");
            var seqNum = 1;
            var request = new GetImageForLegacyRequest
            {
                RequestId = _requestId.Value,
                IrnId = irn.Value,
                LegacyTarget = LegacyTarget.Itms.ToString(),
                Page = 0,
                SeqNum = seqNum,
                Surface = ImageSurface.Front.ToString(),
                TenantId = tenantId,
                UserId = userId.Value
            };
            var image = new Image(new byte[] { 1, 2, 3, 4 }, "image/tiff", 10, 20);
            SetUp_LegacyImageAccess_GetImageFromItms(image);


            // Act
            var context = Mock_ServerCallContext("GetImageForLegacy");
            var reply = await _service.GetImageForLegacy(request, context);

            // Assert
            Assert.NotNull(reply);
            Assert.Equal(image.Width, reply.Width);
            Assert.Equal(image.Height, reply.Height);
            Assert.Equal(image.MimeType, reply.MimeType);
            _legacyImageAccess.Received(0)
                              .GetImageFromWebClient(
                                  Arg.Any<string>(),
                                  Arg.Any<UserId>(),
                                  Arg.Any<IrnId>(),
                                  Arg.Any<int>(),
                                  Arg.Any<ImageSurface>(),
                                  Arg.Any<int>());
            _legacyImageAccess.Received(1)
                              .GetImageFromItms(
                                  Arg.Any<UserId>(),
                                  Arg.Any<IrnId>(),
                                  Arg.Any<int>(),
                                  Arg.Any<ImageSurface>(),
                                  Arg.Any<int>());
        }

        [Trait("Category", "Unit")]
        [Fact]
        public async void GetImageForLegacy_WebClient_ReturnsCorrectImage()
        {
            // Arrange
            var tenantId = "Default";
            var userId = new UserId(1234);
            var irn = new IrnId("EYMG6NTRKU80C04SW4CWWGKW8");
            var seqNum = 1;
            var request = new GetImageForLegacyRequest
            {
                RequestId = _requestId.Value,
                IrnId = irn.Value,
                LegacyTarget = LegacyTarget.WebClient.ToString(),
                Page = 0,
                SeqNum = seqNum,
                Surface = ImageSurface.Front.ToString(),
                TenantId = tenantId,
                UserId = userId.Value
            };
            var image = new Image(new byte[] { 1, 2, 3, 4 }, "image/tiff", 10, 20);
            SetUp_LegacyImageAccess_GetImageFromWebClient(image);

            // Act
            var context = Mock_ServerCallContext("GetImageForLegacy");
            var reply = await _service.GetImageForLegacy(request, context);

            // Assert
            Assert.NotNull(reply);
            Assert.Equal(image.Width, reply.Width);
            Assert.Equal(image.Height, reply.Height);
            Assert.Equal(image.MimeType, reply.MimeType);
            _legacyImageAccess.Received(1)
                              .GetImageFromWebClient(
                                  Arg.Any<string>(),
                                  Arg.Any<UserId>(),
                                  Arg.Any<IrnId>(),
                                  Arg.Any<int>(),
                                  Arg.Any<ImageSurface>(),
                                  Arg.Any<int>());
            _legacyImageAccess.Received(0)
                              .GetImageFromItms(
                                  Arg.Any<UserId>(),
                                  Arg.Any<IrnId>(),
                                  Arg.Any<int>(),
                                  Arg.Any<ImageSurface>(),
                                  Arg.Any<int>());
        }

        [Trait("Category", "Unit")]
        [Fact]
        public async void GetImageForLegacy_Itms_ReturnsNotFound()
        {
            // Arrange
            var tenantId = "Default";
            var userId = new UserId(1234);
            var irn = new IrnId("EYMG6NTRKU80C04SW4CWWGKW8");
            var seqNum = 1;
            var request = new GetImageForLegacyRequest
            {
                RequestId = _requestId.Value,
                IrnId = irn.Value,
                LegacyTarget = LegacyTarget.Itms.ToString(),
                Page = 0,
                SeqNum = seqNum,
                Surface = ImageSurface.Front.ToString(),
                TenantId = tenantId,
                UserId = userId.Value
            };

            SetUp_LegacyImageAccess_GetImageFromItms(new NotFound("Could not fine the image."));

            // Act
            var context = Mock_ServerCallContext("GetImageForLegacy");
            var exception = await Record.ExceptionAsync(async () => await _service.GetImageForLegacy(request, context));

            // Assert
            _legacyImageAccess.Received(0)
                              .GetImageFromWebClient(
                                  Arg.Any<string>(),
                                  Arg.Any<UserId>(),
                                  Arg.Any<IrnId>(),
                                  Arg.Any<int>(),
                                  Arg.Any<ImageSurface>(),
                                  Arg.Any<int>());
            _legacyImageAccess.Received(1)
                              .GetImageFromItms(
                                  Arg.Any<UserId>(),
                                  Arg.Any<IrnId>(),
                                  Arg.Any<int>(),
                                  Arg.Any<ImageSurface>(),
                                  Arg.Any<int>());

            Assert.IsType<RpcException>(exception);
            var rpc = exception as RpcException;
            Assert.Equal(StatusCode.NotFound, rpc.StatusCode);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async void AddImage_ValidParams_HandlesRequestContextAndPerformance()
        {
            // Arrange
            var imageId = new ImageId("2f081e58d50b49b0aa8a884faf674af3");
            var dummyBytes = new byte[] { 0x01, 0x02, 0x03, 0x04 };
            var mimeType = "image/jpeg";
            var request = new AddImageRequest
            {
                RequestId = _requestId.Value,
                Content = ByteString.CopyFrom(dummyBytes),
                MimeType = mimeType
            };

            SetUp_RepoAddImage_ReturnsImageId(mimeType, imageId);

            // Act
            var context = Mock_ServerCallContext("AddImage");
            var _ = await Record.ExceptionAsync(async () => await _service.AddImage(request, context));

            // Assert
            _requestDataAccessor.Received(1).RequestId = new RequestIdentifier(request.RequestId);
            _requestDataAccessor.Received(1).PerformanceMonitor = Arg.Is<IPerformanceTimer>(p => p != null);
            _performanceLogger.Received(1).Log(Arg.Any<IPerformanceTimer>(), Arg.Any<string>(), Arg.Any<string>());
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async void AddImage_Resize_ResizedImageAdded()
        {
            // Arrange
            var imageId = new ImageId("2f081e58d50b49b0aa8a884faf674af3");
            var dummyBytes = new byte[] { 0x01, 0x02, 0x03, 0x04 };
            var mimeType = "image/jpeg";
            var request = new AddImageRequest
            {
                RequestId = _requestId.Value,
                Content = ByteString.CopyFrom(dummyBytes),
                MimeType = mimeType
            };

            SetUp_RepoAddImage_ReturnsImageId(mimeType, imageId);

            // Act
            var context = Mock_ServerCallContext("AddImage");
            var reply = await _service.AddImage(request, context);

            // Assert
            AssertImageAddedWithCorrectFormat(mimeType);
            Assert.Equal(imageId.Value, reply.ImageId);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async void AddImage_ConvertAndResizeGif_ConvertedAndResizedImageAdded()
        {
            // Arrange
            var imageId = new ImageId("2f081e58d50b49b0aa8a884faf674af3");
            var dummyBytes = new byte[] { 0x1, 0x2, 0x3, 0x4 };
            var originalMimeType = "image/gif";
            var convertedMimeType = "image/jpeg";
            var request = new AddImageRequest
            {
                RequestId = _requestId.Value,
                Content = ByteString.CopyFrom(dummyBytes),
                MimeType = originalMimeType
            };

            SetUp_RepoAddImage_ReturnsImageId(convertedMimeType, imageId);

            // Act
            var context = Mock_ServerCallContext("AddImage");
            var reply = await _service.AddImage(request, context);

            // Assert
            AssertImageAddedWithCorrectFormat(convertedMimeType);
            Assert.Equal(imageId.Value, reply.ImageId);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async void AddImage_ConvertAndResizeBmp_ConvertedAndResizedImageAdded()
        {
            // Arrange
            var imageId = new ImageId("2f081e58d50b49b0aa8a884faf674af3");
            var dummyBytes = new byte[] { 0x1, 0x2, 0x3, 0x4 };
            var originalMimeType = "image/bmp";
            var convertedMimeType = "image/jpeg";
            var request = new AddImageRequest
            {
                RequestId = _requestId.Value,
                Content = ByteString.CopyFrom(dummyBytes),
                MimeType = originalMimeType
            };

            SetUp_RepoAddImage_ReturnsImageId(convertedMimeType, imageId);
           

            // Act
            var context = Mock_ServerCallContext("AddImage");
            var reply = await _service.AddImage(request, context);

            // Assert
            AssertImageAddedWithCorrectFormat(convertedMimeType);
            Assert.Equal(imageId.Value, reply.ImageId);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async void AddImage_ConvertAndResizePng_ConvertedAndResizedImageAdded()
        {
            // Arrange
            var imageId = new ImageId("2f081e58d50b49b0aa8a884faf674af3");
            var dummyBytes = new byte[] { 0x01, 0x02, 0x03, 0x04 };
            var originalMimeType = "image/png";
            var convertedMimeType = "image/jpeg";
            var request = new AddImageRequest
            {
                RequestId = _requestId.Value,
                Content = ByteString.CopyFrom(dummyBytes),
                MimeType = originalMimeType
            };

            SetUp_RepoAddImage_ReturnsImageId(convertedMimeType, imageId);

            // Act
            var context = Mock_ServerCallContext("AddImage");
            var reply = await _service.AddImage(request, context);

            // Assert
            AssertImageAddedWithCorrectFormat(convertedMimeType);
            Assert.Equal(imageId.Value, reply.ImageId);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async void AddImage_ConvertAndResizeTiff_ConvertedAndResizedImageAdded()
        {
            // Arrange
            var imageId = new ImageId("2f081e58d50b49b0aa8a884faf674af3");
            var dummyBytes = new byte[] { 0x01, 0x02, 0x03, 0x04 };
            var originalMimeType = "image/tiff";
            var convertedMimeType = "image/jpeg";
            var request = new AddImageRequest
            {
                RequestId = _requestId.Value,
                Content = ByteString.CopyFrom(dummyBytes),
                MimeType = originalMimeType
            };

            SetUp_RepoAddImage_ReturnsImageId(convertedMimeType, imageId);

            // Act
            var context = Mock_ServerCallContext("AddImage");
            var reply = await _service.AddImage(request, context);

            // Assert
            AssertImageAddedWithCorrectFormat(convertedMimeType);
            Assert.Equal(imageId.Value, reply.ImageId);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async void AddImageHandler_Resize_ResizedImageAdded()
        {
            // Arrange
            var imageId = new ImageId("2f081e58d50b49b0aa8a884faf674af3");
            var dummyBytes = new byte[] { 0x01, 0x02, 0x03, 0x04 };
            var mimeType = "image/jpeg";
            var request = new AddImageRequest
            {
                RequestId = _requestId.Value,
                Content = ByteString.CopyFrom(dummyBytes),
                MimeType = mimeType
            };
            SetUp_RepoAddImage_ReturnsImageId(mimeType, imageId);

            // Act
            var context = Mock_ServerCallContext("AddImage");
            var reply = await _service.AddImage(request, context);

            // Assert
            AssertImageAddedWithCorrectFormat(mimeType);
            Assert.Equal(imageId.Value, reply.ImageId);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async void AddImage_NullRequest_ThrowsException()
        {
            // Arrange

            // Act
            var context = Mock_ServerCallContext("AddImage");
            var exception = await Record.ExceptionAsync(() => _service.AddImage(null, context));

            // Assert
            Assert.NotNull(exception);
            Assert.IsType<ArgumentNullException>(exception);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async void AddImage_ExceptionDuringExecution_ReturnsFailure()
        {
            // Arrange
            var request = new AddImageRequest
            {
                RequestId = _requestId.Value,
                Content = ByteString.CopyFrom(3, 7, 11),
                MimeType = "image/jpeg",
            };

            _imageRepository.When(
                     x => x.AddImage(
                         Arg.Any<byte[]>(),
                         Arg.Any<string>(),
                         Arg.Any<int>(),
                         Arg.Any<int>())
                         )
                 .Do(x => throw new Exception());

            // Act
            var context = Mock_ServerCallContext("AddImage");
            var exception = await Record.ExceptionAsync(async () => await _service.AddImage(request, context));

            // Assert
            Assert.IsType<RpcException>(exception);
            var rpc = exception as RpcException;
            Assert.Equal(StatusCode.Aborted, rpc.StatusCode);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async void AddTiff_TiffImage_ReturnsSuccess()
        {
            // Arrange
            var imageId = ImageId.Create();
            var dummyBytes = new byte[] { 0x01, 0x02, 0x03, 0x04 };
            var request = new AddTiffRequest
            {
                RequestId = _requestId.Value,
                Content = ByteString.CopyFrom(dummyBytes)
            };

            SetUp_RepoAddImage_ReturnsImageId(imageId);
            SetUp_Image_Properties(format: ImageFormat.Tiff, mimeType: "image/tiff");

            // Act
            var context = Mock_ServerCallContext("AddTiff");
            var reply = await _service.AddTiff(request, context);

            // Assert
            Assert.Equal(imageId.Value, reply.ImageId);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async void AddTiff_NonTiffImage_ReturnsFailure()
        {
            // Arrange
            var dummyBytes = new byte[] { 0x01, 0x02, 0x03, 0x04 };
            var request = new AddTiffRequest
            {
                RequestId = _requestId.Value,
                Content = ByteString.CopyFrom(dummyBytes)
            };

            SetUp_RepoAddImage_ReturnsImageId(ImageId.Create());
            SetUp_Image_Properties();

            // Act
            var context = Mock_ServerCallContext("AddTiff");
            var exception = await Record.ExceptionAsync(async () => await _service.AddTiff(request, context));

            // Assert
            Assert.IsType<RpcException>(exception);
            var rpc = exception as RpcException;
            Assert.Equal(StatusCode.Aborted, rpc.StatusCode);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async void WriteImageToWebClient_ImageNotFound_ReturnsFailure()
        {
            // Arrange
            var tenantId = "Default";
            var filepath = "C:\\Temp\\";
            var filename = "IRN_SeqNum_SheetNumber_Surface.tif";
            var imageId = new ImageId("2f081e58d50b49b0aa8a884faf674af3");
            var request = new WriteImageToWebClientRequest
            {
                RequestId = _requestId.Value,
                TenantId = tenantId,
                ImageId = imageId.Value,
                Filepath = filepath,
                Filename = filename
            };

            SetUp_RepoGetImage_ReturnsNotFound();

            // Act
            var context = Mock_ServerCallContext("WriteImageToWebClient");
            var exception = await Record.ExceptionAsync(async () => await _service.WriteImageToWebClient(request, context));

            // Assert
            Assert.IsType<RpcException>(exception);
            var rpc = exception as RpcException;
            Assert.Equal(StatusCode.Aborted, rpc.StatusCode);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async void WriteImageToWebClient_FailureWhenWritingImage_ReturnsFailure()
        {
            // Arrange
            var tenantId = "Default";
            var filepath = "C:\\Temp\\";
            var filename = "IRN_SeqNum_SheetNumber_Surface.tif";
            var imageId = new ImageId("2f081e58d50b49b0aa8a884faf674af3");
            var bytes = new byte[] { 1, 2, 3, 4 };
            var image = new Image(bytes, "image/jpeg", 5312, 2988);

            var request = new WriteImageToWebClientRequest
            {
                RequestId = _requestId.Value,
                TenantId = tenantId,
                ImageId = imageId.Value,
                Filepath = filepath,
                Filename = filename
            };

            SetUp_RepoGetImage_ReturnsImage(imageId, image);
            SetUp_LegacyImageAccess_WriteImageToWebClient(new Error("Couldn't write image."));

            // Act
            var context = Mock_ServerCallContext("WriteImageToWebClient");
            var exception = await Record.ExceptionAsync(async () => await _service.WriteImageToWebClient(request, context));

            // Assert
            Assert.IsType<RpcException>(exception);
            var rpc = exception as RpcException;
            Assert.Equal(StatusCode.Aborted, rpc.StatusCode);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async void VerifyImageSizeGrpc_ValidParams_HandlesRequestContextAndPerformance()
        {
            // Arrange
            var imageId = new ImageId("2f081e58d50b49b0aa8a884faf674af3");
            var request = new VerifyImageSizeRequest { ImageId = imageId.Value, RequestId = _requestId.Value };

            // Act
            var context = Mock_ServerCallContext("VerifyImageSizeGrpc");
            var exception = await Record.ExceptionAsync(() => _service.VerifyImageSizeGrpc(request, context));

            // Assert
            Assert.IsType<RpcException>(exception);
            var rpc = exception as RpcException;
            Assert.Equal(StatusCode.Aborted, rpc.StatusCode);
            _requestDataAccessor.Received(1).RequestId = _requestId;
            _requestDataAccessor.Received(1).PerformanceMonitor = Arg.Is<IPerformanceTimer>(p => p != null);
            _performanceLogger.Received(1).Log(Arg.Any<IPerformanceTimer>(), Arg.Any<string>(), Arg.Any<string>());
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async void VerifyImageSizeGrpc_NullRequest_ThrowsException()
        {
            // Arrange

            // Act
            var context = Mock_ServerCallContext("VerifyImageSizeGrpc");
            var exception = await Record.ExceptionAsync(() => _service.VerifyImageSizeGrpc(null, context));

            // Assert
            Assert.NotNull(exception);
            Assert.IsType<ArgumentNullException>(exception);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async void VerifyImageSizeGrpc_FlipAndResize_ResizedImageUpdated()
        {
            // Arrange
            var imageId = new ImageId("2f081e58d50b49b0aa8a884faf674af3");
         
            var bytes = new byte[(int)ImageVaultService.MaxAllowedFileSizeInByte + 1];
            var image = new Image(bytes, "image/jpeg", 2620, 4656);
            var expectedWidth = ImageVaultService.MaxStorageWidth;
            var expectedHeight = ImageVaultService.MaxStorageWidth / 4656 * 2620;

            var request = new VerifyImageSizeRequest { ImageId = imageId.Value, RequestId = _requestId.Value };

            SetUp_RepoGetImage_ReturnsImage(imageId, image);
            SetUp_Image_Properties(image.Width, image.Height);
            SetUp_Image_Resize(expectedWidth, expectedHeight);

            // Act
            var context = Mock_ServerCallContext("VerifyImageSizeGrpc");
            await _service.VerifyImageSizeGrpc(request, context);

            // Assert
            _requestDataAccessor.Received(1).RequestId = _requestId;
            _imageRepository.Received(1)
                 .UpdateImage(imageId, Arg.Any<byte[]>(), image.MimeType, ImageVaultService.MaxStorageWidth, Arg.Any<int>());
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async void VerifyImageSizeGrpc_Scale_ResizedImageUpdated()
        {
            // Arrange
            var imageId = new ImageId("2f081e58d50b49b0aa8a884faf674af3");
            var startingWidth = 5312;
            var startingHeight = 2988;
            var bytes = new byte[(int)ImageVaultService.MaxAllowedFileSizeInByte + 1];
            var image = new Image(bytes, "image/jpeg", startingWidth, startingHeight);
            var expectedWidth = ImageVaultService.MaxStorageWidth;
            var expectedHeight = ImageVaultService.MaxStorageWidth / startingWidth * startingHeight;

            var request = new VerifyImageSizeRequest { ImageId = imageId.Value, RequestId = _requestId.Value };

            SetUp_RepoGetImage_ReturnsImage(imageId, image);
            SetUp_Image_Properties(image.Width, image.Height);
            SetUp_Image_Resize(expectedWidth, expectedHeight);

            // Act
            var context = Mock_ServerCallContext("VerifyImageSizeGrpc");
            await _service.VerifyImageSizeGrpc(request, context);

            // Assert
            _requestDataAccessor.Received(1).RequestId = _requestId;
            _imageRepository.Received(1).UpdateImage(imageId, Arg.Any<byte[]>(), image.MimeType, Arg.Any<int>(), Arg.Any<int>());
        }

        private ServerCallContext Mock_ServerCallContext(string method)
        {
            return TestServerCallContext.Create(
                method,
                "localhost",
                DateTime.Now.AddMinutes(5),
                Metadata.Empty,
                new CancellationToken(),
                null,
                null,
                null,
                null,
                null,
                null);
        }

        private void SetUp_RepoGetImage_ReturnsImage(ImageId imageId, Image imageToReturn)
        {
            _imageRepository.GetImage(imageId).Returns(imageToReturn);
        }

        private void SetUp_RepoGetImage_ReturnsNotFound()
        {
            _imageRepository.GetImage(Arg.Any<ImageId>()).Returns(new NotFound());
        }

        private void SetUp_RepoAddImage_ReturnsImageId(ImageId imageIdToReturn)
        {
            _imageRepository.AddImage(Arg.Any<byte[]>(), Arg.Any<string>(), Arg.Any<int>(), Arg.Any<int>())
                 .Returns(imageIdToReturn);
        }

        private void SetUp_RepoAddImage_ReturnsImageId(string mimeType, ImageId imageIdToReturn)
        {
            _imageRepository.AddImage(Arg.Any<byte[]>(), mimeType, Arg.Any<int>(), Arg.Any<int>()).Returns(imageIdToReturn);
        }

        private void SetUp_LegacyImageAccess_WriteImageToWebClient(Result<Error, ImageTiffInfo> result)
        {
            _legacyImageAccess.WriteImageToWebClient(
                                  Arg.Any<Image>(),
                                  Arg.Any<ImageId>(),
                                  Arg.Any<string>(),
                                  Arg.Any<string>(),
                                  Arg.Any<string>())
                              .Returns(result);
        }

        private void SetUp_LegacyImageAccess_GetImageFromWebClient(Result<Error, Image> result)
        {
            _legacyImageAccess.GetImageFromWebClient(Arg.Any<string>(), Arg.Any<IrnId>(), Arg.Any<ImageSurface>(), Arg.Any<int>())
                .Returns(result);

            _legacyImageAccess.GetImageFromWebClient(
                    Arg.Any<string>(),
                    Arg.Any<UserId>(),
                    Arg.Any<IrnId>(),
                    Arg.Any<int>(),
                    Arg.Any<ImageSurface>(),
                    Arg.Any<int>())
                .Returns(result);
        }

        private void SetUp_LegacyImageAccess_GetImageFromItms(Result<Error, Image> result)
        {
            _legacyImageAccess.GetImageFromItms(
                    Arg.Any<UserId>(),
                    Arg.Any<IrnId>(),
                    Arg.Any<int>(),
                    Arg.Any<ImageSurface>(),
                    Arg.Any<int>())
                .Returns(result);
        }

        private void AssertImageAddedWithCorrectFormat(string expectedMimeType)
        {
            _imageRepository.Received(1)
                 .AddImage(
                     Arg.Any<byte[]>(),
                     Arg.Is<string>(format => string.CompareOrdinal(expectedMimeType, format) == 0),
                     Arg.Is<int>(width => width <= ImageVaultService.MaxStorageWidth),
                     Arg.Any<int>()
                     );

            _imageRepository.Received(0)
                 .AddImage(
                     Arg.Any<byte[]>(),
                     Arg.Is<string>(format => format != expectedMimeType),
                     Arg.Any<int>(),
                     Arg.Any<int>()
                     );
        }

        private void SetUp_Image_Properties(
            int width = 96,
            int height = 96,
            ImageFormat format = ImageFormat.Jpeg,
            string mimeType = "image/jpeg")
        {
            _image.Height.Returns(height);
            _image.Width.Returns(width);
            _image.Format.Returns(format);
            _image.MimeType.Returns(mimeType);
        }

        private void SetUp_Image_SetFormat()
        {
            _image.SetFormat(Arg.Do<ImageFormat>(f => _image.Format.Returns(f)));
        }

        private void SetUp_Image_Resize(int newWidth, int newHeight)
        {
            _image.ResizeToWidth(Arg.Do<int>(i =>
            {
                _image.Width.Returns(newWidth);
                _image.Height.Returns(newHeight);
            }));
        }
    }
}
