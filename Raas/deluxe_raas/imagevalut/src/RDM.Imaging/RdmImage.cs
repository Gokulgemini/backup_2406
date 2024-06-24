using System;
using System.IO;
using ImageMagick;
using RDM.Core;

namespace RDM.Imaging
{
    /// <summary>
    /// A default image class that implements <see cref="IImage"/> using
    /// ImageMagick.
    /// </summary>
    public class RdmImage : IImage
    {
        private MagickImage _image;
        private const int ThresholdNeighborhood = 19;
        private const float ThresholdBias = -2.0f;

        public RdmImage(byte[] imageData)
        {
            Contract.Requires<ArgumentNullException>(imageData != null, nameof(imageData));

            try
            {
                _image = new MagickImage(imageData);
            }
            catch(MagickException magicEx)
            {
                throw new ImageException(magicEx.Message, magicEx);
            }
        }

        public RdmImage(string filePath)
        {
            Contract.Requires<ArgumentNullException>(filePath != null, nameof(filePath));

            _image = new MagickImage(filePath);
        }

        /// <inheritdoc/>
        public int Width => _image.Width;

        /// <inheritdoc/>
        public int Height => _image.Height;

        /// <inheritdoc/>
        public ImageFormat Format => _image.Format.ToImageFormat();

        public string MimeType => Format.ToMimeType();

        /// <inheritdoc/>
        public void Dispose()
        {
            _image.Dispose();
        }

        /// <inheritdoc/>
        public bool IsLandscape
        {
            get { return _image.Height <= _image.Width; }
        }

        /// <inheritdoc/>
        public bool ToLandscape()
        {
            if (!IsLandscape)
            {
                _image.Rotate(-90);
                return true;
            }

            return false;
        }

        /// <inheritdoc />
        public bool ResizeToWidth(int targetWidth)
        {
            var imageChanged = false;

            if (_image.Width != targetWidth)
            {
                // It has been determined that the default filter type would
                // cause issues with Mitek's image quality detection and cause
                // more items to pass then expected. Cubic has been showing
                // similar results to ECAPI.
                _image.FilterType = FilterType.Cubic;

                // Height will be calculated based on width to maintain the
                // aspect ratio
                _image.Resize(targetWidth, 0);

                imageChanged = true;
            }

            return imageChanged;
        }

        /// <inheritdoc/>
        public void ResizeToMaximumFileSize(int maximumFileSize, int maxResizes, ImageFormat format)
        {
            // It's too bad we have to ToByteArray this to get the size, but there's
            // not really a good option.
            SetFormat(format);

            var scaleCount = 0;

            while (GetFileSize() > maximumFileSize)
            {
                scaleCount++;
                // Keep reducing scale until we are under our file size threshold
                _image.Resize(new Percentage(75));
                // I don't know if this makes sense... previous code would bail
                // after 4 resizes and just give up (because it reduced scale
                // 25 each time). I doubt we'll need to scale down that far anyway.
                if (scaleCount > maxResizes)
                    break;
            }

            int GetFileSize()
            {
                return ToByteArray().Length;
            }
        }

        /// <inheritdoc/>
        public void SetFormat(ImageFormat format)
        {
            if ( Format == format)
            {
                // Already in the right format;
                return;
            }

            if ( format == ImageFormat.Tiff)
            {
                // Of course these are specific options that we wouldn't want in
                // a general purpose image library, but if we are saving as tiffs
                // that means something specific to ITMS.
                if (_image.ChannelCount > 1)
                {
                    _image.Grayscale();
                }

                _image.AdaptiveThreshold(ThresholdNeighborhood, ThresholdNeighborhood, new Percentage(ThresholdBias));
                _image.Density = new Density(200);
                _image.Settings.Compression = CompressionMethod.Group4;
            }

            _image.Format = format.ToMagickFormat();
        }

        public byte[] ToByteArray()
        {
            return _image.ToByteArray();
        }

        public void Save(string filePath)
        {
            _image.Write(filePath);
        }

        public void Save(Stream stream)
        {
            _image.Write(stream);
        }
    }
}
