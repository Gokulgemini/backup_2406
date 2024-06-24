using System;
using System.Threading.Tasks;
using Google.Protobuf;
using Grpc.Core;
using ImageVaultGrpc;
using RDM.Core;
using RDM.Data.ImageVault;
using RDM.Imaging;
using RDM.Messaging.ImageVault;
using RDM.Model.Itms;
using RDM.Statistician.PerformanceLog;
using RDM.Statistician.PerformanceTimer;
using Serilog;
using Void = ImageVaultGrpc.Void;

namespace RDM.Services.ImageVault
{
    public class GrpcService : ImageVaultGrpcService.ImageVaultGrpcServiceBase
    {
        internal const string JpegMimeType = "image/jpeg";
        internal const string TiffMimeType = "image/tiff";
        internal const int MaxAllowedFileSizeInByte = 1300000; // 1.3MB

        internal const int MaxStorageWidth = 1600;
        internal const int MaxResizes = 4;

        private readonly ILogger _logger;
        private readonly IPerformanceLogger _performanceLogger;
        private readonly IRequestDataAccessor _requestDataAccessor;
        private readonly IImageRepository _imageRepository;
        private readonly ILegacyImageAccess _legacyImageAccess;
        private readonly IImageFactory _imageFactory;

        public GrpcService(
            ILogger logger,
            IPerformanceLogger performanceLogger,
            IRequestDataAccessor requestDataAccessor,
            IImageRepository imageRepository,
            ILegacyImageAccess legacyImageAccess,
            IImageFactory imageFactory)
        {
            Contract.Requires<ArgumentNullException>(logger != null, nameof(logger));
            Contract.Requires<ArgumentNullException>(performanceLogger != null, nameof(performanceLogger));
            Contract.Requires<ArgumentNullException>(requestDataAccessor != null, nameof(requestDataAccessor));
            Contract.Requires<ArgumentNullException>(imageRepository != null, nameof(imageRepository));
            Contract.Requires<ArgumentNullException>(legacyImageAccess != null, nameof(legacyImageAccess));
            Contract.Requires<ArgumentNullException>(imageFactory != null, nameof(imageFactory));

            _logger = logger;
            _performanceLogger = performanceLogger;
            _requestDataAccessor = requestDataAccessor;
            _imageRepository = imageRepository;
            _legacyImageAccess = legacyImageAccess;
            _imageFactory = imageFactory;
        }

        public override async Task<AddImageReply> AddImage(AddImageRequest message, ServerCallContext context)
        {
            Contract.Requires<ArgumentNullException>(message != null, nameof(message));

            BeginRequestContext(message.RequestId);

            try
            {
                _logger.Debug(
                   "AddImage gRPC called with MimeType '{MimeType}', RequestId '{RequestId}'.",
                   message.MimeType,
                   message.RequestId);

                var imageData = ResizeImageIfNecessary(message.Content.ToByteArray(), ImageFormat.Jpeg);

                // Add the image to the repository
                var imageId = _imageRepository.AddImage(
                    imageData.ImageBytes,
                    JpegMimeType,
                    imageData.Width,
                    imageData.Height);

                _logger.Debug(
                    "AddImage gRPC returning success with ImageId '{ImageId}', RequestId '{RequestId}'.",
                    imageId.Value,
                    message.RequestId);

                return await Task.FromResult(
                    new AddImageReply
                    {
                        ImageId = imageId.Value
                    });

            }
            catch (RpcException)
            {
                // already logged and I don't think we want this to be fatal
                throw;
            }
            catch (Exception e)
            {
                _requestDataAccessor.PerformanceMonitor.StopAll();
                _logger.Fatal(
                    e,
                    "AddImage gRPC failed with message " + e.Message + ". RequestId '{RequestId}'. Please contact a developer.",
                    message.RequestId);

                throw new RpcException(new Status(StatusCode.Aborted, e.Message));
            }
            finally
            {
                EndRequestContext("GrpcService.AddImage");
            }
        }

        public override async Task<AddImageReply> AddTiff(AddTiffRequest message, ServerCallContext context)
        {
            Contract.Requires<ArgumentNullException>(message != null, nameof(message));

            BeginRequestContext(message.RequestId);

            try
            {
                _logger.Debug(
                    "AddTiff gRPC called with image of size '{FileSize}'. RequestId '{RequestId}'.",
                    message.Content.Length,
                    message.RequestId);

                // Convert back to ByteArray
                var imageData = message.Content.ToByteArray();
                using (var image = _imageFactory.CreateImage(imageData))
                {
                    if (image.Format != ImageFormat.Tiff)
                    {
                        _logger.Debug(
                            "AddTiff gRPC returning failure due to the supplied image not being in tiff format. Unexpected image format '{ImageFormat}'. RequestId '{RequestId}'.",
                            image.Format.ToString(),
                            message.RequestId);

                        throw new RpcException(new Status(StatusCode.Aborted, "Wrong Format"));
                    }

                    // Add the image to the repository
                    var imageId = _imageRepository.AddImage(
                        imageData,
                        TiffMimeType,
                        image.Width,
                        image.Height);

                    _logger.Debug(
                        "AddTiff gRPC returning success with ImageId '{ImageId}', RequestId '{RequestId}'.",
                        imageId.Value,
                        message.RequestId);

                    return await Task.FromResult(
                        new AddImageReply
                        {
                            ImageId = imageId.Value
                        });
                }
            }
            catch (RpcException)
            {
                // already logged and I don't think we want this to be fatal
                throw;
            }
            catch (Exception e)
            {
                _requestDataAccessor.PerformanceMonitor.StopAll();
                _logger.Fatal(
                    e,
                    "AddTiff gRPC failed with message " + e.Message + ". RequestId '{RequestId}'. Please contact a developer.",
                    message.RequestId);

                throw new RpcException(new Status(StatusCode.Aborted, e.Message));
            }
            finally
            {
                EndRequestContext("GrpcService.AddTiff");
            }
        }

        public override async Task<GetImageReply> GetImage(GetImageRequest message, ServerCallContext context)
        {
            Contract.Requires<ArgumentNullException>(message != null, nameof(message));

            BeginRequestContext(message.RequestId);

            try
            {
                _logger.Debug(
                    "GetImage gRPC called with ImageId '{ImageId}', Width '{Width}', RequestId '{RequestId}'.",
                    message.ImageId,
                    message.Width,
                    message.RequestId);


                var getImageResult = _imageRepository.GetImage(new ImageId(message.ImageId));
                if (getImageResult.IsFailure)
                {
                    _logger.Debug("GetImage gRPC could not find the image. RequestId '{RequestId}'.", message.RequestId);

                    throw new RpcException(new Status(StatusCode.NotFound, "Item Not Found"));
                }

                var desiredWidth = message.Width;
                var image = getImageResult.Value;
                var isResizeNeeded = desiredWidth != 0 && desiredWidth != image.Width; // gRPC does not support null so use 0 as null

                if (!isResizeNeeded)
                {
                    _logger.Debug(
                        "GetImage gRPC returning with MimeType '{MimeType}', Width '{Width}', Height '{Height}', Content Size '{ContentSize}', RequestId '{RequestId}'.",
                        image.MimeType,
                        image.Width,
                        image.Height,
                        image.Content.Length,
                        message.RequestId);

                    return await Task.FromResult(
                        new GetImageReply
                        {
                            Content = ByteString.CopyFrom(image.Content),
                            MimeType = image.MimeType,
                            Width = image.Width,
                            Height = image.Height
                        });
                }

                var updatedImage = ReSizeAndConvertImage(image.Content, desiredWidth);

                _logger.Debug(
                    "GetImage gRPC returning with Width '{Width}', Height '{Height}', Content Size '{ContentSize}', RequestId '{RequestId}'.",
                    updatedImage.Width,
                    updatedImage.Height,
                    updatedImage.ImageBytes.Length,
                    message.RequestId);

                return await Task.FromResult(
                    new GetImageReply
                    {
                        Content = ByteString.CopyFrom(updatedImage.ImageBytes),
                        MimeType = JpegMimeType,
                        Width = updatedImage.Width,
                        Height = updatedImage.Height
                    });
            }
            catch (RpcException)
            {
                // already logged and I don't think we want this to be fatal
                throw;
            }
            catch (Exception e)
            {
                _requestDataAccessor.PerformanceMonitor.StopAll();
                _logger.Fatal(
                    e,
                    "GetImage gRPC failed with message " + e.Message + ". RequestId '{RequestId}'. Please contact a developer.",
                    message.RequestId);

                throw new RpcException(new Status(StatusCode.Aborted, e.Message));
            }
            finally
            {
                EndRequestContext("GrpcService.GetImage");
            }
        }

        public override async Task<GetImageReply> GetImageAsJpeg(GetImageAsJpegRequest message, ServerCallContext context)
        {
            Contract.Requires<ArgumentNullException>(message != null, nameof(message));

            BeginRequestContext(message.RequestId);

            try
            {
                _logger.Debug(
                    "GetImageAsJpeg gRPC called with ImageId '{ImageId}', Width '{Width}', RequestId '{RequestId}'.",
                    message.ImageId,
                    message.Width,
                    message.RequestId);

                var getImageResult = _imageRepository.GetImage(new ImageId(message.ImageId));
                if (getImageResult.IsFailure)
                {
                    _logger.Debug(
                        "GetImageAsJpeg gRPC could not find the image. RequestId '{RequestId}'.",
                        message.RequestId);

                    throw new RpcException(new Status(StatusCode.NotFound, "Item Not Found"));
                }

                var desiredWidth = message.Width;
                var image = getImageResult.Value;
                var isImageJpeg = image.MimeType == JpegMimeType;
                var isResizeNeeded = desiredWidth != 0 && desiredWidth != image.Width; // gRPC does not support null so use 0 as null

                // If the retrieved image is already a jpeg and no resize is needed go ahead and return it as is.
                if (isImageJpeg && !isResizeNeeded)
                {
                    _logger.Debug(
                        "GetImageAsJpeg gRPC returning with MimeType '{MimeType}', Width '{Width}', Height '{Height}', Content Size '{ContentSize}', RequestId '{RequestId}'.",
                        image.MimeType,
                        image.Width,
                        image.Height,
                        image.Content.Length,
                        message.RequestId);

                    return await Task.FromResult(
                        new GetImageReply
                        {
                            Content = ByteString.CopyFrom(image.Content),
                            MimeType = image.MimeType,
                            Width = image.Width,
                            Height = image.Height
                        });
                }

                var updatedImage = ReSizeAndConvertImage(image.Content, desiredWidth);

                return await Task.FromResult(
                    new GetImageReply
                    {
                        Content = ByteString.CopyFrom(updatedImage.ImageBytes),
                        MimeType = JpegMimeType,
                        Width = updatedImage.Width,
                        Height = updatedImage.Height
                    });
            }
            catch (RpcException)
            {
                // already logged and I don't think we want this to be fatal
                throw;
            }
            catch (Exception e)
            {
                _requestDataAccessor.PerformanceMonitor.StopAll();
                _logger.Fatal(
                    e,
                    "GetImageAsJpeg gRPC failed with message "
                    + e.Message
                    + ". RequestId '{RequestId}'. Please contact a developer.",
                    message.RequestId);

                throw new RpcException(new Status(StatusCode.Aborted, e.Message));
            }
            finally
            {
                EndRequestContext("GrpcService.GetImageAsJpeg");
            }
        }

        public override async Task<GetImageReply> GetImageByIrn(GetImageByIrnRequest message, ServerCallContext context)
        {
            Contract.Requires<ArgumentNullException>(message != null, nameof(message));

            BeginRequestContext(message.RequestId);

            try
            {
                _logger.Debug(
                    "GetImageByIrn gRPC called with IrnId '{IrnId}', Surface '{Surface}', Page '{Page}', Width '{Width}', RequestId '{RequestId}'.",
                    message.IrnId,
                    message.Surface,
                    message.Page,
                    message.Width,
                    message.RequestId);


                var desiredWidth = message.Width;
                var irnId = new IrnId(message.IrnId);
                Enum.TryParse(message.Surface, out ImageSurface surface);

                var getImageResult = _legacyImageAccess.GetImageFromWebClient(
                    message.TenantId,
                    irnId,
                    surface,
                    message.Page);

                if (getImageResult.IsFailure)
                {
                    var errorMessage = "GetImageByIrn gRPC failed with error: '"
                                       + getImageResult.FailureValue.Message
                                       + "'. RequestId '{RequestId}'.";

                    switch (getImageResult.FailureValue)
                    {
                        case NotFound _:
                        {
                            _logger.Debug(errorMessage, message.RequestId);

                            throw new RpcException(new Status(StatusCode.NotFound, "Item Not Found"));
                        }

                        default:
                        {
                            _logger.Error(errorMessage, message.RequestId);

                            throw new RpcException(new Status(StatusCode.Aborted, getImageResult.FailureValue.Message));
                        }
                    }
                }

                var image = getImageResult.Value;
                var isImageJpeg = image.MimeType == JpegMimeType;
                var isResizeNeeded = desiredWidth != 0 && desiredWidth != image.Width; // gRPC does not support null so use 0 as null

                // If the retrieved image is already a jpeg and no resize is needed go ahead and return it as is.
                if (isImageJpeg && !isResizeNeeded)
                {
                    _logger.Debug(
                        "GetImageByIrn gRPC found the requested image and is returning it. RequestId '{RequestId}'.",
                        message.RequestId);

                    return await Task.FromResult(
                        new GetImageReply
                        {
                            Content = ByteString.CopyFrom(getImageResult.Value.Content),
                            MimeType = getImageResult.Value.MimeType,
                            Width = getImageResult.Value.Width,
                            Height = getImageResult.Value.Height
                        });
                }

                var updatedImage = ReSizeAndConvertImage(image.Content, desiredWidth);

                return await Task.FromResult(
                    new GetImageReply
                    {
                        Content = ByteString.CopyFrom(updatedImage.ImageBytes),
                        MimeType = JpegMimeType,
                        Width = updatedImage.Width,
                        Height = updatedImage.Height
                    });
            }
            catch (RpcException)
            {
                // already logged and I don't think we want this to be fatal
                throw;
            }
            catch (Exception e)
            {
                _requestDataAccessor.PerformanceMonitor.StopAll();
                _logger.Fatal(
                    e,
                    "GetImageByIrn gRPC failed with message "
                    + e.Message
                    + ". RequestId '{RequestId}'. Please contact a developer.",
                    message.RequestId);

                throw new RpcException(new Status(StatusCode.Aborted, e.Message));
            }
            finally
            {
                EndRequestContext("GrpcService.GetImageByIrn");
            }
        }

        public override async Task<GetImageReply> GetImageForLegacy(GetImageForLegacyRequest message, ServerCallContext context)
        {
            Contract.Requires<ArgumentNullException>(message != null, nameof(message));

            BeginRequestContext(message.RequestId);

            try
            {
                _logger.Debug(
                    "GetImageForLegacy called with LegacyTarget '{LegacyTarget}', IrnId '{IrnId}', UserId '{UserId}', SeqNum '{SeqNum}' Surface '{Surface}', Page '{Page}',RequestId '{RequestId}',TenantId '{TenantId}'.",
                    message.LegacyTarget,
                    message.IrnId,
                    message.UserId,
                    message.SeqNum,
                    message.Surface,
                    message.Page,
                    message.RequestId,
                    message.TenantId);

                Enum.TryParse(message.LegacyTarget, out LegacyTarget legacyTarget);
                var tenantId = message.TenantId;
                var userId = new UserId(message.UserId);
                var irnId = new IrnId(message.IrnId);
                var seqNum = message.SeqNum;
                Enum.TryParse(message.Surface, out ImageSurface surface);
                var page = message.Page;

                Result<Error, Image> getImageResult;
                switch (legacyTarget)
                {
                    case LegacyTarget.WebClient:
                    {
                        getImageResult = _legacyImageAccess.GetImageFromWebClient(tenantId, userId, irnId, seqNum, surface, page);

                        break;
                    }

                    case LegacyTarget.Itms:
                    {
                        getImageResult = _legacyImageAccess.GetImageFromItms(userId, irnId, seqNum, surface, page);

                        break;
                    }

                    default:
                    {
                        var status = new Status(
                            StatusCode.InvalidArgument,
                            $"GetImageForLegacy has not implemented a way to retrieve images for legacy target '{legacyTarget}'.");
                        _logger.Error(status.Detail, message.RequestId);
                        throw new RpcException(status);
                    }
                }

                if (getImageResult.IsSuccess)
                {
                    _logger.Debug(
                        "GetImageForLegacy found the requested image for LegacyTarget '{LegacyTarget}', IrnId '{IrnId}', UserId '{UserId}', SeqNum '{SeqNum}' Surface '{Surface}', Page '{Page}',RequestId '{RequestId}',TenantId '{TenantId}'.",
                        message.LegacyTarget,
                        message.IrnId,
                        message.UserId,
                        message.SeqNum,
                        message.Surface,
                        message.Page,
                        message.RequestId,
                        message.TenantId);

                    return await Task.FromResult(
                        new GetImageReply
                        {
                            Content = ByteString.CopyFrom(getImageResult.Value.Content),
                            MimeType = getImageResult.Value.MimeType,
                            Width = getImageResult.Value.Width,
                            Height = getImageResult.Value.Height
                        });
                }

                var errorMessage = "GetImageForLegacy failed with error: '"
                                   + getImageResult.FailureValue.Message
                                   + " LegacyTarget '{LegacyTarget}', IrnId '{IrnId}', UserId '{UserId}', SeqNum '{SeqNum}' Surface '{Surface}', Page '{Page}',RequestId '{RequestId}',TenantId '{TenantId}'.";
                switch (getImageResult.FailureValue)
                {
                    case NotFound _:
                    {
                        _logger.Information(errorMessage, 
                            message.LegacyTarget,
                            message.IrnId,
                            message.UserId,
                            message.SeqNum,
                            message.Surface,
                            message.Page,
                            message.RequestId,
                            message.TenantId);

                        throw new RpcException(new Status(StatusCode.NotFound, "Item Not Found"));
                    }

                    default:
                    {
                        _logger.Error(errorMessage, 
                            message.LegacyTarget,
                            message.IrnId,
                            message.UserId,
                            message.SeqNum,
                            message.Surface,
                            message.Page,
                            message.RequestId,
                            message.TenantId);


                        throw new RpcException(new Status(StatusCode.Aborted, getImageResult.FailureValue.Message));
                    }
                }
            }
            catch (RpcException)
            {
                // already logged and I don't think we want this to be fatal
                throw;
            }
            catch (Exception e)
            {
                _requestDataAccessor.PerformanceMonitor.StopAll();
                _logger.Fatal(
                    e,
                    "GetImageForLegacy failed with message "
                    + e.Message
                    + ". LegacyTarget '{LegacyTarget}', IrnId '{IrnId}', UserId '{UserId}', SeqNum '{SeqNum}' Surface '{Surface}', Page '{Page}', TenantId '{TenantId}'."
                    + " RequestId '{RequestId}'. Please contact a developer.",
                    message.LegacyTarget,
                    message.IrnId,
                    message.UserId,
                    message.SeqNum,
                    message.Surface,
                    message.Page,
                    message.TenantId,
                    message.RequestId);

                throw new RpcException(new Status(StatusCode.Aborted, e.Message));
            }
            finally
            {
                EndRequestContext("GrpcService.GetImageForLegacy");
            }
        }

        public override async Task<WriteImageToWebClientReply> WriteImageToWebClient(WriteImageToWebClientRequest message, ServerCallContext context)
        {
            Contract.Requires<ArgumentNullException>(message != null, nameof(message));

            BeginRequestContext(message.RequestId);

            try
            {
                _logger.Debug(
                    "WriteImageToWebClient gRPC called with ImageId '{ImageId}', RequestId '{RequestId}'.",
                    message.ImageId,
                    message.RequestId);

                var imageId = new ImageId(message.ImageId);

                var writeToLegacyResult = _imageRepository
                    .GetImage(imageId)
                    .OnSuccess(
                        image => _legacyImageAccess.WriteImageToWebClient(
                            image,
                            imageId,
                            message.TenantId,
                            message.Filepath,
                            message.Filename));

                if (writeToLegacyResult.IsFailure)
                {
                    // Pretty much any way this function could fail would be due to an issue that requires someone from IT or deployment to look at.
                    // Normally if an image is not found that's not necessarily a bad thing but because this is part of the shunting process it is something that should be examined.
                    // All of failures should be logged as an error in this case.
                    _logger.Error(
                        "WriteImageToWebClient gRPC failed with error message '"
                        + writeToLegacyResult.FailureValue.Message
                        + "'. RequestId '{RequestId}'.",
                        message.RequestId);

                    throw new RpcException(new Status(StatusCode.Aborted, writeToLegacyResult.FailureValue.Message));
                }

                var tiffInfo = writeToLegacyResult.Value;

                _logger.Debug(
                    "WriteImageToWebClient gRPC successfully wrote the image to legacy. Returning with ImageFilename '{ImageFilename}', ImageUrl '{ImageUrl}', TiffSize '{TiffSize}', TiffWidth '{TiffWidth}', TiffHeight '{TiffHeight}', RequestId '{RequestId}'.",
                    tiffInfo.ImageFilename,
                    tiffInfo.ImageUrl,
                    tiffInfo.TiffSize,
                    tiffInfo.TiffWidth,
                    tiffInfo.TiffHeight,
                    message.RequestId);

                return await Task.FromResult(
                    new WriteImageToWebClientReply
                    {
                        ImageId = tiffInfo.ImageId.Value,
                        ImageFilename = tiffInfo.ImageFilename,
                        ImageUrl = tiffInfo.ImageUrl,
                        TiffSize = tiffInfo.TiffSize,
                        TiffWidth = tiffInfo.TiffWidth,
                        TiffHeight = tiffInfo.TiffHeight
                    });
            }
            catch (RpcException)
            {
                // already logged and I don't think we want this to be fatal
                throw;
            }
            catch (Exception e)
            {
                _requestDataAccessor.PerformanceMonitor.StopAll();
                _logger.Fatal(
                    e,
                    "WriteImageToWebClient gRPC failed with message "
                    + e.Message
                    + ". RequestId '{RequestId}'. Please contact a developer.",
                    message.RequestId);

                throw new RpcException(new Status(StatusCode.Aborted, e.Message));
            }
            finally
            {
                EndRequestContext("GrpcService.WriteImageToWebClient");
            }
        }

        public override async Task<Void> VerifyImageSizeGrpc(VerifyImageSizeRequest message, ServerCallContext context)
        {
            Contract.Requires<ArgumentNullException>(message != null, nameof(message));

            BeginRequestContext(message?.RequestId);

            try
            {
                _logger.Debug(
                    "VerifyImageSizeGrpc called with ImageId '{ImageId}, RequestId '{RequestId}'.",
                    message.ImageId,
                    message.RequestId);

                var imageId = new ImageId(message.ImageId);

                var getImageResult = _imageRepository.GetImage(imageId);
                if (getImageResult.IsFailure)
                {
                    _logger.Error(
                        "Attempting to verify the size of an image that does not exist. Please contact a developer about this issue. ImageId '{ImageId}', RequestId '{RequestId}'.",
                        message.ImageId,
                        message.RequestId);

                    return await Task.FromResult(new Void());
                }

                var image = getImageResult.Value;
                if (image.Content.Length <= MaxAllowedFileSizeInByte)
                {
                    _logger.Debug(
                        "VerifyImageSizeGrpc: Image '{ImageId}' is already at an accepted size. RequestId '{RequestId}'.",
                        message.ImageId,
                        message.RequestId);

                    return await Task.FromResult(new Void());
                }

                _logger.Debug(
                    "VerifyImageSizeGrpc: Image '{ImageId}' is too large with length {Length}. Attempting to scale it down. RequestId '{RequestId}'.",
                    message.ImageId,
                    image.Content.Length,
                    message.RequestId);

                var imageData = ResizeImageIfNecessary(image.Content);

                _imageRepository.UpdateImage(
                    imageId,
                    imageData.ImageBytes,
                    JpegMimeType,
                    imageData.Width,
                    imageData.Height);

                return await Task.FromResult(new Void());
            }
            catch (RpcException)
            {
                throw;
            }
            catch (Exception e)
            {
                _requestDataAccessor.PerformanceMonitor.StopAll();
                _logger.Fatal(
                    e,
                    "VerifyImageSizeGrpc gRPC failed with message "
                    + e.Message
                    + ". RequestId '{RequestId}'. Please contact a developer.",
                    message.RequestId);

                throw new RpcException(new Status(StatusCode.Aborted, e.Message));
            }
            finally
            {
                EndRequestContext("GrpcService.VerifyImageSizeGrpc");
            }
        }

        private void BeginRequestContext(string requestId)
        {
            var monitor = new PerformanceTimer();

            _requestDataAccessor.RequestId = new RequestIdentifier(requestId);
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

        /// <summary>
        /// This method is meant to replace/combine work done to resize the image
        /// in both AddImageHandler and VerifyImageSizeHandler, and therefore making
        /// VerifyImageSizeHandler obsolete.
        ///
        /// OpenCV was used to resize in one, and ImageMagick in the other. We are
        /// moving to just ImageMagick for now.
        /// </summary>
        /// <param name="imageBytes"></param>
        private ImageData ResizeImageIfNecessary(byte[] imageBytes, ImageFormat format = ImageFormat.Jpeg)
        {
            using (var image = _imageFactory.CreateImage(imageBytes))
            {
                // Code put in place to merge changes from VerifyImageFileSize and this function
                // included changing to landscape. I'm a little concerned of the effect of this
                // on documents, but so far no one has complained so I guess it's OK?
                image.ToLandscape();

                // Resize to the maximum width. This resize honors aspect ratio so the height
                // can just be zero.
                if (image.Width > MaxStorageWidth)
                {
                    image.ResizeToWidth(MaxStorageWidth);
                }

                // If necessary, resize to the maximum file size as well
                // Previous code forced or assumed this was a Jpeg image, so we will continue
                // that. 
                image.ResizeToMaximumFileSize(MaxAllowedFileSizeInByte, 4, format);

                return ImageData.FromImage(image, format);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private ImageData ReSizeAndConvertImage(byte[] imageBytes, int desiredWidth, ImageFormat format = ImageFormat.Jpeg)
        {
            using (var image = _imageFactory.CreateImage(imageBytes))
            {
                if ((desiredWidth > 0) && (desiredWidth != image.Width))
                {
                    image.ResizeToWidth(desiredWidth);
                }

                return ImageData.FromImage(image, format);
            }
        }
    }
}
