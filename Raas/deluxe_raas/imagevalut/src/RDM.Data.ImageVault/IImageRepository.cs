using System;
using RDM.Core;
using RDM.Model.Itms;

namespace RDM.Data.ImageVault
{
    public interface IImageRepository
    {
        /// <summary>
        /// Stores the image information in the datastore.
        /// </summary>
        /// <param name="content">The binary content of the image.</param>
        /// <param name="mimeType">The image encoding format.</param>
        /// <param name="width">The width of the image.</param>
        /// <param name="height">The height  of the image.</param>
        /// <returns>
        /// Returns the <see cref="Image"/> representing the image in the datastore.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="content"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="mimeType"/> is <c>null</c> or whitespace.
        /// </exception>
        ImageId AddImage(byte[] content, string mimeType, int width, int height);

        /// <summary>
        /// Retrieves the information for the specified image.
        /// </summary>
        /// <param name="imageId">The unique identifier of the image to retrieve.</param>
        /// <returns>
        /// Returns the information about the specified image if found, otherwise returns <c>null</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="imageId"/> is <c>null</c>.
        /// </exception>
        Result<Error, Image> GetImage(ImageId imageId);

        /// <summary>
        /// Updates the specified image from the datastore
        /// </summary>
        /// <param name="imageId">The unique identifier of the image to update.</param>
        /// <param name="content">The binary content of the image.</param>
        /// <param name="mimeType">The image encoding format.</param>
        /// <param name="width">The width of the image.</param>
        /// <param name="height">The height  of the image.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="imageId"/> is <c>null</c>.
        /// </exception>
        void UpdateImage(ImageId imageId, byte[] content, string mimeType, int width, int height);

        /// <summary>
        /// Deletes the specified image from the datastore, if present.
        /// </summary>
        /// <param name="imageId">The unique identifier of the image to remove.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="imageId"/> is <c>null</c>.
        /// </exception>
        void RemoveImage(ImageId imageId);
    }
}
