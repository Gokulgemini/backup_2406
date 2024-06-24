using System;
using System.IO;

namespace RDM.Imaging
{
    public interface IImage : IDisposable
    {
        /// <summary>
        /// The width of the image in pixels
        /// </summary>
        int Width { get; }

        /// <summary>
        /// The height of the image in pixels
        /// </summary>
        int Height { get; }

        /// <summary>
        /// Indicates whether the image has a width that's larger than
        /// it's height.
        /// </summary>
        bool IsLandscape { get; }

        /// <summary>
        /// Rotates the image to landscape
        /// </summary>
        /// <returns><see langword="true"/> if the image is rotated,
        /// <see langword="false"/> if the image was already in landscape.</returns>
        bool ToLandscape();

        /// <summary>
        /// The current file format of the image.
        /// </summary>
        ImageFormat Format { get; }

        /// <summary>
        /// Mime type for the current image file format
        /// </summary>
        string MimeType { get; }

        /// <summary>
        /// Changes the image format to that specified by <paramref name="format"/>.
        /// </summary>
        /// <param name="format">The target format.</param>
        void SetFormat(ImageFormat format);

        /// <summary>
        /// Resizes the image to match the specified width.
        /// Height will be calculated to maintain aspect ratio.
        /// </summary>
        /// <param name="targetWidth">Width to which the image should be resized</param>
        /// <returns>Returns true if the image was changed, false if no
        /// change was necessary</returns>
        bool ResizeToWidth(int targetWidth);

        /// <summary>
        /// Resizes the image by scaling down 25% until the file size is below
        /// <paramref name="maximumFileSize"/>, or <paramref name="maxResizes"/> iterations has been
        /// reached.
        /// </summary>
        /// <param name="maximumFileSize">The maximum file size in bytes.</param>
        /// <param name="maxResizes">The maximum file size in bytes.</param>
        /// <param name="format">Checking the file size requires saving the image to a format.
        /// The format supplied here will be used to check the file size.</param>
        void ResizeToMaximumFileSize(int maximumFileSize, int maxResizes, ImageFormat format);

        /// <summary>
        /// Saves the image in the specified format (or default of Jpeg) and writes
        /// to a stream. 
        /// </summary>
        /// <param name="format">The format of the image file</param>
        /// <returns>An array of <c>byte</c> containing the image file.</returns>
        byte[] ToByteArray();

        /// <summary>
        /// Saves the image in the specified format (or default of Jpeg) to
        /// a file at <paramref name="filePath"/>.
        /// </summary>
        /// <param name="filePath">The path to which to save the file.</param>
        void Save(string filePath);

        /// <summary>
        /// Saves the image in the specified format (or default of Jpeg) to
        /// <paramref name="stream"/>.
        /// </summary>
        /// <param name="stream">The stream to which to save the file.</param>
        void Save(Stream stream);
    }
}
