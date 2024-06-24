using System;
using ImageMagick;

namespace RDM.Imaging
{
    internal static class Extensions
    {
        public static MagickFormat ToMagickFormat(this ImageFormat imageFormat)
        {
            switch (imageFormat)
            {
                case ImageFormat.Jpeg:
                {
                    return MagickFormat.Jpeg;
                }
                case ImageFormat.Bmp:
                {
                    return MagickFormat.Bmp;
                }
                case ImageFormat.Png:
                {
                    return MagickFormat.Png;
                }
                case ImageFormat.Gif:
                {
                    return MagickFormat.Gif;
                }
                case ImageFormat.Tiff:
                {
                    // NOTE! Compression needs to be set as well as format
                    // to correctly save a tiff. 
                    return MagickFormat.Tiff;
                }
                default:
                {
                    throw new ArgumentException(
                        $"Image format {imageFormat} cannot be converted to a valid ImageMagick format.");
                }
            }
        }

        public static ImageFormat ToImageFormat(this MagickFormat magickFormat)
        {
            switch(magickFormat)
            {
                case MagickFormat.Bmp:
                case MagickFormat.Bmp2:
                case MagickFormat.Bmp3:
                {
                    return ImageFormat.Bmp;
                }
                case MagickFormat.Gif:
                case MagickFormat.Gif87:
                {
                    return ImageFormat.Gif;
                }
                case MagickFormat.Jpg:
                case MagickFormat.Jpeg:
                {
                    return ImageFormat.Jpeg;
                }
                case MagickFormat.Tiff:
                case MagickFormat.Tif:
                case MagickFormat.Tiff64:
                {
                    return ImageFormat.Tiff;
                }
                case MagickFormat.Png:
                case MagickFormat.Png00:
                case MagickFormat.Png8:
                case MagickFormat.Png24:
                case MagickFormat.Png32:
                case MagickFormat.Png48:
                case MagickFormat.APng:
                case MagickFormat.Png64:
                {
                    return ImageFormat.Png;
                }
                default:
                {
                    throw new ArgumentException(
                        $"Unable to convert format type {magickFormat} to a supported format type.");
                }
            }
        }

        public static string ToMimeType(this ImageFormat format)
        {
            switch (format)
            {
                case ImageFormat.Jpeg:
                {
                    return "image/jpeg";
                }
                case ImageFormat.Bmp:
                {
                    return "image/bmp";
                }
                case ImageFormat.Gif:
                {
                    return "image/gif";
                }
                case ImageFormat.Png:
                {
                    return "image/png";
                }
                case ImageFormat.Tiff:
                {
                    return "image/tiff";
                }
            }

            throw new ArgumentException($"Unable to determine the correct mime type for format {format}");
        }

    }
}
