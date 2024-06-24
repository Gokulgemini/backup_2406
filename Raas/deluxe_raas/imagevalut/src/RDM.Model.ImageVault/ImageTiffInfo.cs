using System;
using Newtonsoft.Json;
using RDM.Core;
using RDM.Model.Itms;

namespace RDM.Model.ImageVault
{
    /// <summary>
    /// Provides a container for all information about an image stored in the vault.
    /// </summary>
    public class ImageTiffInfo : IEquatable<ImageTiffInfo>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImageTiffInfo"/> class
        /// with the supplied information.
        /// </summary>
        /// <param name="imageId">The unique identifier for the image.</param>
        /// <param name="imageFilename">The filename of the image stored.</param>
        /// <param name="imageUrl">The filepath of the image stored.</param>
        /// <param name="tiffSize">The filesize of the TIFF image.</param>
        /// <param name="tiffWidth">The width of the TIFF image.</param>
        /// <param name="tiffHeight">The height of the TIFF image.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="imageId"/> is <c>null</c>.
        /// </exception>
        [JsonConstructor]
        public ImageTiffInfo(ImageId imageId,  string imageFilename, string imageUrl, int tiffSize, int tiffWidth, int tiffHeight)
        {
            Contract.Requires<ArgumentNullException>(imageId != null, nameof(imageId));
            Contract.Requires<ArgumentException>(!string.IsNullOrWhiteSpace(imageFilename), nameof(imageFilename));
            Contract.Requires<ArgumentException>(!string.IsNullOrWhiteSpace(imageUrl), nameof(imageUrl));
            Contract.Requires<ArgumentException>(tiffSize > 0, nameof(tiffSize));
            Contract.Requires<ArgumentException>(tiffWidth > 0, nameof(tiffWidth));
            Contract.Requires<ArgumentException>(tiffHeight > 0, nameof(tiffHeight));

            ImageId = imageId;
            ImageFilename = imageFilename;
            ImageUrl = imageUrl;
            TiffSize = tiffSize;
            TiffWidth = tiffWidth;
            TiffHeight = tiffHeight;
        }

        /// <summary>
        /// The unique identifier for the image.
        /// </summary>
        public ImageId ImageId { get; }

        /// <summary>
        /// The filename of the image stored.
        /// </summary>
        public string ImageFilename { get; }

        /// <summary>
        /// The filepath of the image stored.
        /// </summary>
        public string ImageUrl { get; }

        /// <summary>
        /// The filesize of the TIFF image.
        /// </summary>
        public int TiffSize { get; }

        /// <summary>
        /// The Width of the TIFF image.
        /// </summary>
        public int TiffWidth { get; }

        /// <summary>
        /// The Height of the TIFF image.
        /// </summary>
        public int TiffHeight { get; }

        /// <inheritdoc/>
        public bool Equals(ImageTiffInfo other)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }

            if (ReferenceEquals(other, this))
            {
                return true;
            }

            return
                ImageId.Equals(other.ImageId) &&
                string.CompareOrdinal(ImageFilename, other.ImageFilename) == 0 &&
                string.CompareOrdinal(ImageUrl, other.ImageUrl) == 0 &&
                TiffSize == other.TiffSize &&
                TiffWidth == other.TiffWidth &&
                TiffHeight == other.TiffHeight;
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return Equals(obj as ImageTiffInfo);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                int result = 13;

                result = (result * 31) + ImageId.GetHashCode();
                result = (result * 31) + ImageFilename.GetHashCode();
                result = (result * 31) + ImageUrl.GetHashCode();
                result = (result * 31) + TiffSize.GetHashCode();
                result = (result * 31) + TiffWidth.GetHashCode();
                result = (result * 31) + TiffHeight.GetHashCode();

                return result;
            }
        }
    }
}