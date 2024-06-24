using System.Threading.Tasks;
using ImageMagick;
using RDM.Core;
using RDM.Model.Itms;
using Serilog;

namespace RDM.Services.ImageViewerAPI
{
    public class ImageConverter : IImageConverter
    {
        public const string PngMimeType = "image/png";

        private readonly IRequestDataAccessor _requestDataAccessor;
        private readonly ILogger _logger;

        public ImageConverter(IRequestDataAccessor requestDataAccessor, ILogger logger)
        {
            _requestDataAccessor = requestDataAccessor;
            _logger = logger;
        }

        public async Task<Result<Error, Image>> ConvertImageToPng(Image originalImage)
        {
            var monitor = _requestDataAccessor.PerformanceMonitor;
            monitor?.Start("ImageConverter.ConvertImageToPng");

            if (originalImage.MimeType == PngMimeType)
            {
                return originalImage;
            }

            try
            {
                using (var original = new MagickImage(originalImage.Content))
                {
                    var convertedBytes = await Task.Run(() => original.ToByteArray(MagickFormat.Png));

                    return new Image(convertedBytes, PngMimeType, originalImage.Width, originalImage.Height);
                }
            }
            catch (MagickException me)
            {
                _logger.Error(
                    me,
                    "An exception was thrown when attempting to convert an image of mimetype {OriginalMimeType} to {PngMimeType}. Please contact a developer about this issue. RequestId '{RequestId}'.",
                    originalImage.MimeType,
                    PngMimeType,
                    _requestDataAccessor.RequestId);

                return new Error("Could not convert the provided image to png format.");
            }
            finally
            {
                monitor?.Stop("ImageConverter.ConvertImageToPng");
            }
        }
    }
}
