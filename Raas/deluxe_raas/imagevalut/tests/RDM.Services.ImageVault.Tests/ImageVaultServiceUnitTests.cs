using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using RDM.Core;
using RDM.Data.ImageVault;
using RDM.Messaging;
using RDM.Messaging.ImageVault;
using RDM.Model.ImageVault;
using RDM.Model.Itms;
using RDM.Statistician.PerformanceLog;
using RDM.Statistician.PerformanceTimer;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Xunit;
using Microsoft.Extensions.Hosting;

namespace RDM.Services.ImageVault.Tests
{
    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Test files are not styled.")]
    [SuppressMessage("StyleCop.CSharp.LayoutRules", "*", Justification = "Test files are not styled.")]
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "*", Justification = "Test files are not styled.")]
    [SuppressMessage("StyleCop.CSharp.OrderingRules", "*", Justification = "Test files are not styled.")]
    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "*", Justification = "Test files are not styled.")]
    [SuppressMessage("StyleCop.CSharp.SpacingRules", "*", Justification = "Test files are not styled.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "Test files are not styled.")]
    public class ImageVaultServiceUnitTests
    {
        private readonly ILogger _logger;
        private readonly LoggingLevelSwitch _loggingLevelSwitch;
        private readonly IPerformanceLogger _performanceLogger;
        private readonly IRequestDataAccessor _requestDataAccessor;
        private readonly IMessageQueue _queue;
        private readonly IImageRepository _repo;
        private readonly IFailureMode _failureMode;
        private readonly IHostApplicationLifetime _appLifetime;
        private readonly RequestIdentifier _requestId;
        private readonly ImageVaultService _service;

        public ImageVaultServiceUnitTests()
        {
            _logger = Substitute.For<ILogger>();
            _loggingLevelSwitch = new LoggingLevelSwitch();
            _performanceLogger = Substitute.For<IPerformanceLogger>();
            _requestDataAccessor = Substitute.For<IRequestDataAccessor>();
            _requestDataAccessor.RequestId.Returns(new RequestIdentifier("1"));
            _queue = Substitute.For<IMessageQueue>();
            _repo = Substitute.For<IImageRepository>();
            _failureMode = Substitute.For<IFailureMode>();
            _appLifetime = Substitute.For<IHostApplicationLifetime>();

            _service = new ImageVaultService(
                _logger,
                _loggingLevelSwitch,
                _performanceLogger,
                _requestDataAccessor,
                _queue,
                _repo,
                _failureMode,
                _appLifetime);

            _requestId = new RequestIdentifier("030833e9ffb84badab5f938b0dc9925f");
        }

        public static IEnumerable<object[]> CtorInvalidArgumentsException()
        {
            var logger = Substitute.For<ILogger>();
            var loggingLevelSwitch = new LoggingLevelSwitch();
            var performanceLogger = Substitute.For<IPerformanceLogger>();
            var requestDataAccessor = Substitute.For<IRequestDataAccessor>();
            requestDataAccessor.RequestId.Returns(new RequestIdentifier("1"));
            var messageQueue = Substitute.For<IMessageQueue>();
            var imageRepository = Substitute.For<IImageRepository>();
            var legacyImageAccess = Substitute.For<ILegacyImageAccess>();
            var failureMode = Substitute.For<IFailureMode>();
            var appLifetime = Substitute.For<IHostApplicationLifetime>();

            return new List<object[]>
            {
                new object[]
                {
                    null,
                    loggingLevelSwitch,
                    performanceLogger,
                    requestDataAccessor,
                    messageQueue,
                    imageRepository,
                    failureMode,
                    appLifetime
                },
                new object[]
                {
                    logger,
                    null,
                    performanceLogger,
                    requestDataAccessor,
                    messageQueue,
                    imageRepository,
                    failureMode,
                    appLifetime
                },
                new object[]
                {
                    logger,
                    loggingLevelSwitch,
                    null,
                    requestDataAccessor,
                    messageQueue,
                    imageRepository,
                    failureMode,
                    appLifetime
                },
                new object[]
                {
                    logger,
                    loggingLevelSwitch,
                    performanceLogger,
                    null,
                    messageQueue,
                    imageRepository,
                    failureMode,
                    appLifetime
                },
                new object[]
                {
                    logger,
                    loggingLevelSwitch,
                    performanceLogger,
                    requestDataAccessor,
                    null,
                    imageRepository,
                    failureMode,
                    appLifetime
                },
                new object[]
                {
                    logger,
                    loggingLevelSwitch,
                    performanceLogger,
                    requestDataAccessor,
                    messageQueue,
                    null,
                    failureMode,
                    appLifetime
                },
                 new object[]
                {
                    logger,
                    loggingLevelSwitch,
                    performanceLogger,
                    requestDataAccessor,
                    messageQueue,
                    imageRepository,
                    null,
                    appLifetime
                },
                 new object[]
                {
                    logger,
                    loggingLevelSwitch,
                    performanceLogger,
                    requestDataAccessor,
                    messageQueue,
                    imageRepository,
                    failureMode,
                    null
                }
            };
        }

        [Theory, MemberData(nameof(CtorInvalidArgumentsException))]
        [Trait("Category", "Unit")]
        public void Ctor_InvalidArguments_ThrowsException(
            ILogger logger,
            LoggingLevelSwitch loggingLevelSwitch,
            IPerformanceLogger performanceLogger,
            IRequestDataAccessor requestDataAccessor,
            IMessageQueue queue,
            IImageRepository imageRepository,
            IFailureMode failureMode,
            IHostApplicationLifetime appLifetime)
        {
            // Arrange

            // Act
            var exception = Record.Exception(
                () => new ImageVaultService(
                    logger,
                    loggingLevelSwitch,
                    performanceLogger,
                    requestDataAccessor,
                    queue,
                    imageRepository,
                    failureMode,
                    appLifetime));

            // Assert
            Assert.NotNull(exception);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void Ctor_ValidParameters_ObjectCreated()
        {
            // Arrange

            // Act
            var result = new ImageVaultService(
                _logger,
                _loggingLevelSwitch,
                _performanceLogger,
                _requestDataAccessor,
                _queue,
                _repo,
                _failureMode,
                _appLifetime);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void Start_NotSubscribed_Subscribes()
        {
            // Arrange

            // Act
            _service.Start();

            // Assert
            _queue.Received(1).Subscribe(Arg.Any<Action<RemoveImageMessage>>());
            _queue.Received(1).SubscribeOperations(Arg.Any<Action<SetApplicationLogLevelMessage>>());
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void LogLevelChangeHandler_ChangeLevel_MinimumLevelChanged()
        {
            // Arrange
            _loggingLevelSwitch.MinimumLevel = LogEventLevel.Warning;

            var expectedLevel = LogEventLevel.Debug;
            var message = new SetApplicationLogLevelMessage(expectedLevel);

            // Act
            var result = Record.Exception(() => _service.LogLevelChangeHandler(message));

            // Assert
            Assert.Null(result);
            Assert.Equal(expectedLevel, _loggingLevelSwitch.MinimumLevel);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void RemoveImageHandler_ValidParams_HandlesRequestContextAndPerformance()
        {
            // Arrange
            var imageId = new ImageId("2f081e58d50b49b0aa8a884faf674af3");
            var request = new RemoveImageMessage(_requestId, imageId);

            // Act
            _service.RemoveImageHandler(request);

            // Assert
            _requestDataAccessor.Received(1).RequestId = request.RequestId;
            _requestDataAccessor.Received(1).PerformanceMonitor = Arg.Is<IPerformanceTimer>(p => p != null);
            _performanceLogger.Received(1).Log(Arg.Any<IPerformanceTimer>(), Arg.Any<string>(), Arg.Any<string>());
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void RemoveImageHandler_NullRequest_ThrowsException()
        {
            // Arrange

            // Act
            var exception = Record.Exception(() => _service.RemoveImageHandler(null));

            // Assert
            Assert.NotNull(exception);
            Assert.IsType<ArgumentNullException>(exception);
        }

        private void SetUp_RepoGetImage_ReturnsImage(ImageId imageId, Image imageToReturn)
        {
            _repo.GetImage(imageId).Returns(imageToReturn);
        }

        private void SetUp_RepoGetImage_ReturnsNotFound()
        {
            _repo.GetImage(Arg.Any<ImageId>()).Returns(new NotFound());
        }

        private void SetUp_RepoAddImage_ReturnsImageId()
        {
            _repo.AddImage(Arg.Any<byte[]>(), Arg.Any<string>(), Arg.Any<int>(), Arg.Any<int>())
                 .Returns(ImageId.Create());
        }

        private void SetUp_RepoAddImage_ReturnsImageId(string mimeType, ImageId imageIdToReturn)
        {
            _repo.AddImage(Arg.Any<byte[]>(), mimeType, Arg.Any<int>(), Arg.Any<int>()).Returns(imageIdToReturn);
        }

        private void AssertImageAddedWithCorrectFormat(string expectedMimeType)
        {
            _repo.Received(1)
                 .AddImage(
                     Arg.Any<byte[]>(),
                     Arg.Is<string>(format => string.CompareOrdinal(expectedMimeType, format) == 0),
                     Arg.Is<int>(width => width <= ImageVaultService.MaxStorageWidth),
                     Arg.Any<int>()
                     );

            _repo.Received(0)
                 .AddImage(
                     Arg.Any<byte[]>(),
                     Arg.Is<string>(format => format != expectedMimeType),
                     Arg.Any<int>(),
                     Arg.Any<int>()
                     );
        }
    }
}
