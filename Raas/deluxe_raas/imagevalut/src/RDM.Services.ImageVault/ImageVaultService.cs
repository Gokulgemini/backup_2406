using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using RDM.Core;
using RDM.Data.ImageVault;
using RDM.Messaging;
using RDM.Messaging.ImageVault;
using RDM.Model.Itms;
using RDM.Statistician.PerformanceLog;
using RDM.Statistician.PerformanceTimer;
using Serilog;
using Serilog.Core;

namespace RDM.Services.ImageVault
{
    public class ImageVaultService : IHostedService
    {
        public static readonly string ServiceName = "ImageVault";

        internal const string JpegMimeType = "image/jpeg";
        internal const string TiffMimeType = "image/tiff";
        internal const int MaxStorageWidth = 1600;
        internal const double MaxAllowedFileSizeInByte = 1300000; // 1.3MB
        internal const int MaxResizes = 4;

        private readonly ILogger _logger;
        private readonly LoggingLevelSwitch _loggingLevelSwitch;
        private readonly IPerformanceLogger _performanceLogger;
        private readonly IRequestDataAccessor _requestDataAccessor;
        private readonly IMessageQueue _queue;
        private readonly IImageRepository _imageRepository;
        private readonly IFailureMode _failureMode;
        private readonly IHostApplicationLifetime _appLifetime;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageVaultService" /> class
        /// with the supplied objects.
        /// </summary>
        /// <param name="logger">An instance of the logging service.</param>
        /// <param name="loggingLevelSwitch">The switch that controls realtime log level changes.</param>
        /// <param name="performanceLogger">The performance logger</param>
        /// <param name="requestDataAccessor">The request data accessor.</param>
        /// <param name="messageQueue">The message queue for communication.</param>
        /// <param name="imageRepository">The Original Vault repository.</param>
        /// <param name="legacyImageAccess">The legacy image access.</param>
        /// <param name="failureMode">The failure mode./param>
        /// <param name="appLifetime">The app lifetime.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="logger" />, <paramref name="loggingLevelSwitch" />, <paramref name="performanceLogger" />,
        /// <paramref name="requestDataAccessor" />, <paramref name="messageQueue" />, <paramref name="imageRepository" />, <paramref name="failureMode" />, or <paramref name="appLifetime" />
        /// is <c>null</c>.
        public ImageVaultService(
            ILogger logger,
            LoggingLevelSwitch loggingLevelSwitch,
            IPerformanceLogger performanceLogger,
            IRequestDataAccessor requestDataAccessor,
            IMessageQueue messageQueue,
            IImageRepository imageRepository,
            IFailureMode failureMode,
            IHostApplicationLifetime appLifetime)
        {
            Contract.Requires<ArgumentNullException>(logger != null, nameof(logger));
            Contract.Requires<ArgumentNullException>(
                loggingLevelSwitch != default(LoggingLevelSwitch),
                nameof(loggingLevelSwitch));
            Contract.Requires<ArgumentNullException>(performanceLogger != null, nameof(performanceLogger));
            Contract.Requires<ArgumentNullException>(requestDataAccessor != null, nameof(requestDataAccessor));
            Contract.Requires<ArgumentNullException>(messageQueue != null, nameof(messageQueue));
            Contract.Requires<ArgumentNullException>(imageRepository != null, nameof(imageRepository));
            Contract.Requires<ArgumentNullException>(failureMode != null, nameof(failureMode));
            Contract.Requires<ArgumentNullException>(appLifetime != null, nameof(appLifetime));

            _logger = logger;
            _loggingLevelSwitch = loggingLevelSwitch;
            _performanceLogger = performanceLogger;
            _requestDataAccessor = requestDataAccessor;
            _queue = messageQueue;
            _imageRepository = imageRepository;
            _failureMode = failureMode;
            _appLifetime = appLifetime;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                _logger.Information($"{ServiceName} worker starting");
                Start();
                _logger.Information($"{ServiceName} worker started");
                return Task.CompletedTask;
            }
            catch (Exception e)
            {
                _logger.Error($"{ServiceName} failed to start:" + e);
                throw;
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            try
            {
                _logger.Information($"{ServiceName} worker stopping.");
                Stop();
                _logger.Information($"{ServiceName} worker stopped.");
            }
            catch (Exception e)
            {
                _logger.Error($"{ServiceName} failed to stop: " + e.ToString());
            }
            return Task.CompletedTask;
        }

        public void Start()
        {
            _queue.Subscribe<RemoveImageMessage>(RemoveImageHandler);
            _queue.SubscribeOperations<SetApplicationLogLevelMessage>(LogLevelChangeHandler);
        }

        public void Stop()
        {
            _queue.Unsubscribe<RemoveImageMessage>(RemoveImageHandler);
            _queue.Unsubscribe<SetApplicationLogLevelMessage>(LogLevelChangeHandler);
        }

        internal void LogLevelChangeHandler(SetApplicationLogLevelMessage message)
        {
            Contract.Requires<ArgumentNullException>(message != null, nameof(message));

            _loggingLevelSwitch.MinimumLevel = message.NewLevel;

            _logger.Information("Loglevel change to '{NewLevel}'", message.NewLevel);
        }

        internal void RemoveImageHandler(RemoveImageMessage message)
        {
            Contract.Requires<ArgumentNullException>(message != null, nameof(message));

            BeginRequestContext(message.RequestId);

            try
            {
                _logger.Debug(
                    "RemoveImageHandler called with ImageId '{ImageId}, RequestId '{RequestId}'.",
                    message.ImageId.Value,
                    message.RequestId.Value);

                _imageRepository.RemoveImage(message.ImageId);
            }
            catch (Exception e)
            {
                _requestDataAccessor.PerformanceMonitor.StopAll();
                _logger.Fatal(
                    e,
                    "RemoveImageHandler failed with message "
                    + e.Message
                    + ". RequestId '{RequestId}'. Please contact a developer.",
                    message.RequestId.Value);
                ErrorHandler(message);
            }
            finally
            {
                EndRequestContext("ImageVaultService.RemoveImageHandler");
            }
        }

        private void BeginRequestContext(RequestIdentifier requestId)
        {
            var monitor = new PerformanceTimer();

            _requestDataAccessor.RequestId = requestId;
            _requestDataAccessor.PerformanceMonitor = monitor;

            monitor.Start("TOTAL_TIME");
        }

        private void EndRequestContext(string handlerName)
        {
            var monitor = _requestDataAccessor.PerformanceMonitor;

            monitor.Stop("TOTAL_TIME");

            _performanceLogger.Log(monitor, handlerName, _requestDataAccessor.RequestId.Value);

            _requestDataAccessor.RequestId = null;
            _requestDataAccessor.PerformanceMonitor = null;
        }

        private void ErrorHandler(IMessage message)
        {
            var killer = new DeathRattleMessage(message, DateTime.UtcNow, ServiceName);
            _queue.Publish(killer);

            if (_failureMode.Mode == Mode.Stop)
                _appLifetime.StopApplication();
        }
    }
}