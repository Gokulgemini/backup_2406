using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Google.Protobuf;
using Grpc.Core;
using Grpc.Core.Interceptors;
using ImageVaultGrpc;
using OpenTracing.Contrib.Grpc.Interceptors;
using RDM.Core;
using RDM.Messaging;
using RDM.Messaging.ImageVault;
using RDM.Messaging.RabbitMQ;
using RDM.Model.ImageVault;
using RDM.Model.Itms;

namespace RDM.Client.ImageVault
{
    public class ImageVaultClient : IImageVaultClient, IDisposable
    {
        private const int Port = 50055;

        private const int BufferSize = 1000000; // 1MB

        private readonly IMessageQueue _queue;
        private readonly Channel _channel;
        private readonly ImageVaultGrpcService.ImageVaultGrpcServiceClient _grpcClient;
        private readonly int _grpcTimeout = 30; // Default 30

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageVaultClient"/> class
        /// with the supplied communication options.
        /// </summary>
        /// <param name="options">The options to use when communicating with service.</param>
        public ImageVaultClient(IMessageOptions options)
            : this(new RabbitQueue(options.Options))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageVaultClient"/> class
        /// with the supplied communication queue.
        /// </summary>
        /// <param name="queue">The queue options to use when communicating with service.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="queue"/> is <c>null</c>.
        /// </exception>
        public ImageVaultClient(IMessageQueue queue)
            : this(queue, "0.0.0.0")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageVaultClient"/> class
        /// with the supplied communication queue.
        /// </summary>
        /// <param name="queue">The queue options to use when communicating with service.</param>
        /// <param name="grpcServerHost">The host address for grpc.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="queue"/> is <c>null</c>.
        /// </exception>
        public ImageVaultClient(IMessageQueue queue, string grpcServerHost, int? grpcTimeout = null)
        {
            Contract.Requires<ArgumentNullException>(queue != null, nameof(queue));

            _queue = queue;

            if (grpcTimeout != null)
            {
                _grpcTimeout = (int)grpcTimeout;
            }

            var channelOptions = new List<ChannelOption>
            {
                new ChannelOption(ChannelOptions.MaxReceiveMessageLength, 30 * 1024 * 1024), // 30MB
            };
            _channel = new Channel(grpcServerHost, Port, ChannelCredentials.Insecure, channelOptions);


            // If we have a valid trace logger, make sure we don't break the chain
            if (Statistician.Factory.TraceLogger != null)
            {
                // Setup the jaeger trace interceptor
                var tracingInterceptor = new ClientTracingInterceptor(Statistician.Factory.TraceLogger.Tracer);

                _grpcClient = new ImageVaultGrpcService.ImageVaultGrpcServiceClient(_channel.Intercept(tracingInterceptor));
            }
            else
            {
                _grpcClient = new ImageVaultGrpcService.ImageVaultGrpcServiceClient(_channel);
            }
        }

        public void Dispose()
        {
            _channel.ShutdownAsync().Wait();
        }

        /// <inheritdoc/>
        public Result<Error, ImageId> AddImage(RequestIdentifier requestId, Stream content, string mimeType)
        {
            Contract.Requires<ArgumentNullException>(content != null, nameof(content));
            Contract.Requires<ArgumentException>(content.Length > 0, nameof(content));

            return AddImageImplementation(requestId, StreamToBytes(content), mimeType);
        }

 
        /// <inheritdoc/>
        public Result<Error, ImageId> AddImage(RequestIdentifier requestId, byte[] content, string mimeType)
        {
            return AddImageImplementation(requestId, content, mimeType);
        }

        /// <inheritdoc/>
        public Result<Error, ImageId> AddTiff(RequestIdentifier requestId, byte[] content)
        {
            return AddTiffImplementation(requestId, content);
        }

        /// <inheritdoc/>
        public Result<Error, Image> GetImage(RequestIdentifier requestId, ImageId imageId)
        {
            return GetImageImplementation(requestId, imageId, null);
        }

        /// <inheritdoc/>
        public Result<Error, Image> GetImage(RequestIdentifier requestId, ImageId imageId, int width)
        {
            return GetImageImplementation(requestId, imageId, width);
        }

        /// <inheritdoc/>
        public Result<Error, Image> GetImageAsJpeg(RequestIdentifier requestId, ImageId imageId)
        {
            return GetImageAsJpegImplementation(requestId, imageId, null);
        }

        /// <inheritdoc/>
        public Result<Error, Image> GetImageAsJpeg(RequestIdentifier requestId, ImageId imageId, int width)
        {
            return GetImageAsJpegImplementation(requestId, imageId, width);
        }

        /// <inheritdoc/>
        public Result<Error, Image> GetImageByIrn(RequestIdentifier requestId, IrnId irnId, ImageSurface surface, int page, string tenantId, int width)
        {
            Contract.Requires<ArgumentNullException>(requestId != null, nameof(requestId));
            Contract.Requires<ArgumentException>(requestId != RequestIdentifier.Empty, nameof(requestId));
            Contract.Requires<ArgumentNullException>(irnId != null, nameof(irnId));
            Contract.Requires<ArgumentException>(irnId != IrnId.Empty, nameof(irnId));
            Contract.Requires<ArgumentException>(!string.IsNullOrWhiteSpace(tenantId), nameof(tenantId));

            try
            {
                var reply = _grpcClient.GetImageByIrn(
                    new GetImageByIrnRequest
                    {
                        RequestId = requestId.Value,
                        IrnId = irnId.Value,
                        Page = page,
                        Surface = surface.ToString(),
                        TenantId = tenantId,
                        Width = width
                    },
                    deadline: DateTime.UtcNow.AddSeconds(_grpcTimeout));

                return new Image(reply.Content.ToByteArray(), reply.MimeType, reply.Width, reply.Height);
            }
            catch (RpcException e)
            {
                if (e.StatusCode == StatusCode.NotFound)
                {
                    return new NotFound("Item was not found");
                }

                if (e.StatusCode == StatusCode.DeadlineExceeded)
                {
                    return Timeout();
                }

                return new ServiceFailure($"RPC communication Failed, Status:'{e.Status.StatusCode}'. Details: '{e.Status.Detail}'.");
            }
        }

        public async Task<Result<Error, Image>> GetImageForLegacy(
            RequestIdentifier requestId,
            LegacyTarget legacyTarget,
            string tenantId,
            UserId userId,
            IrnId irnId,
            int seqNum,
            ImageSurface surface,
            int page)
        {
            try
            {
                var reply = await _grpcClient.GetImageForLegacyAsync(
                        new GetImageForLegacyRequest
                        {
                            IrnId = irnId.Value,
                            LegacyTarget = legacyTarget.ToString(),
                            Page = page,
                            SeqNum = seqNum,
                            UserId = userId.Value,
                            RequestId = requestId.Value,
                            Surface = surface.ToString(),
                            TenantId = tenantId
                        },
                        deadline: DateTime.UtcNow.AddSeconds(_grpcTimeout));

                return new Image(reply.Content.ToByteArray(), reply.MimeType, reply.Width, reply.Height);
            }
            catch (RpcException e)
            {
                if (e.StatusCode == StatusCode.NotFound)
                {
                    return new NotFound("Item was not found");
                }

                if (e.StatusCode == StatusCode.DeadlineExceeded)
                {
                    return Timeout();
                }

                return new ServiceFailure($"RPC communication Failed, Status:'{e.Status.StatusCode}'. Details: '{e.Status.Detail}'.");
            }
        }

        /// <inheritdoc/>
        public Result<Error, ImageTiffInfo> WriteImageToWebClient(
            RequestIdentifier requestId,
            string tenantId,
            ImageId imageId,
            string filepath,
            string filename)
        {
            Contract.Requires<ArgumentNullException>(requestId != null, nameof(requestId));
            Contract.Requires<ArgumentException>(requestId != RequestIdentifier.Empty, nameof(requestId));
            Contract.Requires<ArgumentNullException>(imageId != null, nameof(imageId));
            Contract.Requires<ArgumentException>(imageId != ImageId.Empty, nameof(imageId));
            Contract.Requires<ArgumentException>(!string.IsNullOrWhiteSpace(filepath), nameof(filepath));
            Contract.Requires<ArgumentException>(!string.IsNullOrWhiteSpace(filename), nameof(filename));
            Contract.Requires<ArgumentException>(!string.IsNullOrWhiteSpace(tenantId), nameof(tenantId));

            try
            {
                var reply = _grpcClient.WriteImageToWebClient(
                    new WriteImageToWebClientRequest
                    {
                        RequestId = requestId.Value,
                        TenantId = tenantId,
                        ImageId = imageId.Value,
                        Filename = filename,
                        Filepath = filepath
                    },
                    deadline: DateTime.UtcNow.AddSeconds(_grpcTimeout));

                return new ImageTiffInfo(new ImageId(reply.ImageId), reply.ImageFilename, reply.ImageUrl, reply.TiffSize, reply.TiffWidth, reply.TiffHeight);
            }
            catch (RpcException e)
            {
                if (e.StatusCode == StatusCode.DeadlineExceeded)
                {
                    return Timeout();
                }

                return new ServiceFailure($"RPC communication Failed, Status:'{e.Status.StatusCode}'. Details: '{e.Status.Detail}'.");
            }
        }

        /// <inheritdoc/>
        public void RemoveImage(RequestIdentifier requestId, ImageId imageId)
        {
            Contract.Requires<ArgumentNullException>(imageId != null, nameof(imageId));
            Contract.Requires<ArgumentException>(imageId != ImageId.Empty, nameof(imageId));
            Contract.Requires<ArgumentNullException>(requestId != null, nameof(requestId));
            Contract.Requires<ArgumentException>(requestId != RequestIdentifier.Empty, nameof(requestId));

            var request = new RemoveImageMessage(requestId, imageId);
            _queue.Publish(request);
        }

        /// <summary>
        /// This method is used by test cases which check for an exception being thrown.
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="imageId"></param>
        public void VerifyImageSize(RequestIdentifier requestId, ImageId imageId)
        {
            Contract.Requires<ArgumentNullException>(requestId != null, nameof(requestId));
            Contract.Requires<ArgumentException>(requestId != RequestIdentifier.Empty, nameof(requestId));
            Contract.Requires<ArgumentNullException>(imageId != null, nameof(imageId));
            Contract.Requires<ArgumentException>(imageId != ImageId.Empty, nameof(imageId));

            _grpcClient.VerifyImageSizeGrpc(
                new VerifyImageSizeRequest
                {
                    RequestId = requestId?.Value,
                    ImageId = imageId?.Value
                },
                deadline: DateTime.UtcNow.AddSeconds(_grpcTimeout));
        }

        private Result<Error, ImageId> AddImageImplementation(
            RequestIdentifier requestId,
            byte[] content,
            string mimeType)
        {
            Contract.Requires<ArgumentNullException>(requestId != null, nameof(requestId));
            Contract.Requires<ArgumentException>(requestId != RequestIdentifier.Empty, nameof(requestId));
            Contract.Requires<ArgumentNullException>(content != null, nameof(content));
            Contract.Requires<ArgumentException>(content.Length > 0, nameof(content));
            Contract.Requires<ArgumentException>(!string.IsNullOrWhiteSpace(mimeType), nameof(mimeType));

            try
            {
                var reply = _grpcClient.AddImage(
                    new AddImageRequest
                    {
                        RequestId = requestId.Value,
                        Content = ByteString.CopyFrom(content),
                        MimeType = mimeType,
                    },
                    deadline: DateTime.UtcNow.AddSeconds(_grpcTimeout));

                return new ImageId(reply.ImageId);
            }
            catch (RpcException e)
            {
                if (e.StatusCode == StatusCode.DeadlineExceeded)
                {
                    return Timeout();
                }

                return new ServiceFailure($"RPC communication Failed, Status:'{e.Status.StatusCode}'. Details: '{e.Status.Detail}'.");
            }
        }

        private Result<Error, ImageId> AddTiffImplementation(RequestIdentifier requestId, byte[] content)
        {
            Contract.Requires<ArgumentNullException>(requestId != null, nameof(requestId));
            Contract.Requires<ArgumentException>(requestId != RequestIdentifier.Empty, nameof(requestId));
            Contract.Requires<ArgumentNullException>(content != null, nameof(content));
            Contract.Requires<ArgumentException>(content.Length > 0, nameof(content));

            try
            {
                var reply = _grpcClient.AddTiff(
                    new AddTiffRequest
                    {
                        RequestId = requestId.Value,
                        Content = ByteString.CopyFrom(content)
                    },
                    deadline: DateTime.UtcNow.AddSeconds(_grpcTimeout));

                return new ImageId(reply.ImageId);
            }
            catch (RpcException e)
            {
                if (e.StatusCode == StatusCode.DeadlineExceeded)
                {
                    return Timeout();
                }

                return new ServiceFailure($"RPC communication Failed, Status:'{e.Status.StatusCode}'. Details: '{e.Status.Detail}'.");
            }
        }

        private Result<Error, Image> GetImageImplementation(RequestIdentifier requestId, ImageId imageId, int? width)
        {
            Contract.Requires<ArgumentNullException>(imageId != null, nameof(imageId));
            Contract.Requires<ArgumentException>(imageId != ImageId.Empty, nameof(imageId));
            Contract.Requires<ArgumentNullException>(requestId != null, nameof(requestId));
            Contract.Requires<ArgumentException>(requestId != RequestIdentifier.Empty, nameof(requestId));

            try
            {
                var reply = _grpcClient.GetImage(
                    new GetImageRequest
                    {
                        RequestId = requestId.Value,
                        ImageId = imageId.Value,
                        Width = width ?? 0
                    },
                    deadline: DateTime.UtcNow.AddSeconds(_grpcTimeout));


                return new Image(reply.Content.ToByteArray(), reply.MimeType, reply.Width, reply.Height);
            }
            catch (RpcException e)
            {
                if (e.StatusCode == StatusCode.NotFound)
                {
                    return new NotFound("Item was not found");
                }

                if (e.StatusCode == StatusCode.DeadlineExceeded)
                {
                    return Timeout();
                }

                return new ServiceFailure($"RPC communication Failed, Status:'{e.Status.StatusCode}'. Details: '{e.Status.Detail}'.");
            }
        }

        private Result<Error, Image> GetImageAsJpegImplementation(RequestIdentifier requestId, ImageId imageId, int? width)
        {
            Contract.Requires<ArgumentNullException>(imageId != null, nameof(imageId));
            Contract.Requires<ArgumentException>(imageId != ImageId.Empty, nameof(imageId));
            Contract.Requires<ArgumentNullException>(requestId != null, nameof(requestId));
            Contract.Requires<ArgumentException>(requestId != RequestIdentifier.Empty, nameof(requestId));

            try
            {
                var reply = _grpcClient.GetImageAsJpeg(
                    new GetImageAsJpegRequest
                    {
                        RequestId = requestId.Value,
                        ImageId = imageId.Value,
                        Width = width ?? 0
                    },
                    deadline: DateTime.UtcNow.AddSeconds(_grpcTimeout));

                return new Image(reply.Content.ToByteArray(), reply.MimeType, reply.Width, reply.Height);
            }
            catch (RpcException e)
            {
                if (e.StatusCode == StatusCode.NotFound)
                {
                    return new NotFound("Item was not found");
                }

                if (e.StatusCode == StatusCode.DeadlineExceeded)
                {
                    return Timeout();
                }

                return new ServiceFailure($"RPC communication Failed, Status:'{e.Status.StatusCode}'. Details: '{e.Status.Detail}'.");
            }
        }

        private byte[] StreamToBytes(Stream content)
        {
            using (var ms = new MemoryStream())
            {
                // Use builtin to convert to MemoryStream before turning into an array
                content.CopyTo(ms, BufferSize);

                return ms.ToArray();
            }
        }

        private Error Timeout() => new Unavailable("Timed out while waiting for a response.");
    }
}
