using System;
using RDM.Core;
using RDM.Messaging;
using RDM.Messaging.ImageViewerAPI;
using Serilog;
using Serilog.Core;

namespace RDM.Services.ImageViewerAPI
{
    public class OperationsService
    {
        public static readonly string ServiceName = "OperationsService";

        private readonly ILogger _logger;
        private readonly LoggingLevelSwitch _loggingLevelSwitch;
        private readonly IMessageQueue _queue;

        private bool _subscribed;

        public OperationsService(ILogger logger, LoggingLevelSwitch loggingLevelSwitch, IMessageQueue queue)
        {
            Contract.Requires<ArgumentNullException>(logger != default(ILogger), nameof(logger));
            Contract.Requires<ArgumentNullException>(loggingLevelSwitch != default(LoggingLevelSwitch), nameof(loggingLevelSwitch));
            Contract.Requires<ArgumentNullException>(queue != default(IMessageQueue), nameof(queue));

            _logger = logger;
            _loggingLevelSwitch = loggingLevelSwitch;
            _queue = queue;
        }

        public void Start()
        {
            if (!_subscribed)
            {
                _subscribed = true;

                // Subscribe to Messages
                _queue.SubscribeOperations<SetApplicationLogLevelMessage>(LogLevelChangeHandler);

                _logger.Information($"{ServiceName} Worker Started");
            }
        }

        public void Stop()
        {
            if (_subscribed)
            {
                // Unsubscribe to Messages
                _queue.Unsubscribe<SetApplicationLogLevelMessage>(LogLevelChangeHandler);

                _subscribed = false;
                _logger.Information($"{ServiceName} Worker Stopped");
            }
        }

        internal void LogLevelChangeHandler(SetApplicationLogLevelMessage message)
        {
            Contract.Requires<ArgumentNullException>(message != null, nameof(message));

            _loggingLevelSwitch.MinimumLevel = message.NewLevel;

            _logger.Information("Loglevel change to '{NewLevel}'", message.NewLevel);
        }
    }
}