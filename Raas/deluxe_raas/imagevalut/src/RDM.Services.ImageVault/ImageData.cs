using System;
using RDM.Imaging;

namespace RDM.Services.ImageVault
{
    public class ImageData
    {
        public ImageData(byte[] imageBytes, int width, int height)
        {
            ImageBytes = imageBytes;
            Width = width;
            Height = height;
        }

        public byte[] ImageBytes { get; }
        public int Width { get; }
        public int Height { get; }

        public int FileSize { get { return ImageBytes?.Length ?? 0; } }

        public static ImageData FromImage(IImage image, ImageFormat format)
        {
            image.SetFormat(format);
            return new ImageData(image.ToByteArray(), image.Width, image.Height);
        }
    }
}
