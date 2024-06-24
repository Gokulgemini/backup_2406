using System;
using System.IO;
using System.Threading.Tasks;
using RDM.Core;
using RDM.Messaging.ImageVault;
using RDM.Model.ImageVault;
using RDM.Model.Itms;

namespace RDM.Client.ImageVault
{
    public interface IImageVaultClient
    {
        /// <summary>
        /// Stores the supplied image in the vault.
        /// </summary>
        /// <param name="requestId">The request Id to associate with the log.</param>
        /// <param name="content">The byte content of the image to store.</param>
        /// <param name="mimeType">The mimetype identifier of the image.</param>
        /// <returns>
        /// Returns the identifier assigned to the image in the vault.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="requestId"/> or <paramref name="content"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="requestId"/> is <c>RequestIdentifier.Empty</c>
        /// or when <paramref name="mimeType"/> is <c>null</c> or whitespace.
        /// </exception>
        Result<Error, ImageId> AddImage(RequestIdentifier requestId, Stream content, string mimeType);

        /// <summary>
        /// Stores the supplied image in the vault.
        /// </summary>
        /// <param name="requestId">The request Id to associate with the log.</param>
        /// <param name="content">The byte content of the image to store.</param>
        /// <param name="mimeType">The mimetype identifier of the image.</param>
        /// <returns>
        /// Returns the identifier assigned to the image in the vault.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="requestId"/> or <paramref name="content"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="requestId"/> is <c>RequestIdentifier.Empty</c>,
        /// or when <paramref name="mimeType"/> is <c>null</c> or whitespace.
        /// </exception>
        Result<Error, ImageId> AddImage(RequestIdentifier requestId, byte[] content, string mimeType);

        /// <summary>
        /// Stores the supplied tiff in the vault.
        /// </summary>
        /// <param name="requestId">The request Id to associate with the log.</param>
        /// <param name="content">The byte content of the image to store.</param>
        /// <returns>
        /// Returns the identifier assigned to the image in the vault.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="requestId"/> or <paramref name="content"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="requestId"/> is <c>RequestIdentifier.Empty</c>
        /// or when <paramref name="content"/> has a length of 0.
        /// </exception>
        Result<Error, ImageId> AddTiff(RequestIdentifier requestId, byte[] content);

        /// <summary>
        /// Retrieves the specified <see cref="ImageId"/> from the vault.
        /// </summary>
        /// <param name="requestId">The request Id to associate with the log.</param>
        /// <param name="imageId">The identifier of the image to retrieve.</param>
        /// <returns>
        /// Returns the image from the vault if found, otherwise returns <c>null</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="requestId"/> or <paramref name="imageId"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="requestId"/> is <c>RequestIdentifier.Empty</c>
        /// or when <paramref name="imageId"/> is <c>ImageId.Empty</c>.
        /// </exception>
        Result<Error, Image> GetImage(RequestIdentifier requestId, ImageId imageId);

        /// <summary>
        /// Retrieves the specified <see cref="ImageId"/> from the vault.
        /// </summary>
        /// <param name="requestId">The request Id to associate with the log.</param>
        /// <param name="imageId">The identifier of the image to retrieve.</param>
        /// <param name="width">The desired width of the returned image, <c>null</c> to perform no resize.</param>
        /// <returns>
        /// Returns the image from the vault if found, otherwise returns <c>null</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="requestId"/> or <paramref name="imageId"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="requestId"/> is <c>RequestIdentifier.Empty</c>
        /// or when <paramref name="imageId"/> is <c>ImageId.Empty</c>.
        /// </exception>
        Result<Error, Image> GetImage(RequestIdentifier requestId, ImageId imageId, int width);

        /// <summary>
        /// Retrieves the specified <see cref="ImageId"/> from the vault and converts it to jpeg if it is not already one.
        /// </summary>
        /// <param name="requestId">The request Id to associate with the log.</param>
        /// <param name="imageId">The identifier of the image to retrieve.</param>
        /// <returns>
        /// Returns the image from the vault if found, otherwise returns <c>null</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="requestId"/> or <paramref name="imageId"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="requestId"/> is <c>RequestIdentifier.Empty</c>
        /// or when <paramref name="imageId"/> is <c>ImageId.Empty</c>.
        /// </exception>
        Result<Error, Image> GetImageAsJpeg(RequestIdentifier requestId, ImageId imageId);

        /// <summary>
        /// Retrieves the specified <see cref="ImageId"/> from the vault and converts it to jpeg if it is not already one.
        /// </summary>
        /// <param name="requestId">The request Id to associate with the log.</param>
        /// <param name="imageId">The identifier of the image to retrieve.</param>
        /// <param name="width">The desired width of the returned image, <c>null</c> to perform no resize.</param>
        /// <returns>
        /// Returns the image from the vault if found, otherwise returns <c>null</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="requestId"/> or <paramref name="imageId"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="requestId"/> is <c>RequestIdentifier.Empty</c>
        /// or when <paramref name="imageId"/> is <c>ImageId.Empty</c>.
        /// </exception>
        Result<Error, Image> GetImageAsJpeg(RequestIdentifier requestId, ImageId imageId, int width);

        /// <summary>
        /// Retrieves the requested image from WebClientDB
        /// </summary>
        /// <param name="requestId">The request Id to associate with the log.</param>
        /// <param name="irnId">The identifier for the item.</param>
        /// <param name="surface">The surface of the image to be retrieved.</param>
        /// <param name="page">The page of the image to be retrieved.</param>
        /// <param name="tenantId">The identifier of the tenant to access data from.</param>
        /// <param name="width">Specifies the width of the returned image. A zero value will return the image as stored.</param>
        /// <returns>
        /// Returns the content of the image if the item exists
        /// </returns>
        Result<Error, Image> GetImageByIrn(RequestIdentifier requestId, IrnId irnId, ImageSurface surface, int page, string tenantId, int width = 0);

        /// <summary>
        /// An asynchronous gRPC version of GetImageForLegacy, which retrieves the requested image from the specified target legacy environment
        /// </summary>
        /// <param name="requestId">The request Id to associate with the log.</param>
        /// <param name="legacyTarget">The legacy target to pull the image from.</param>
        /// <param name="tenantId">The identifier of the tenant to access data from.</param>
        /// <param name="userId">The identifier of the user trying to access image info.</param>
        /// <param name="irnId">The identifier for the item.</param>
        /// <param name="seqNum">The item's sequential number within a transaction.</param>
        /// <param name="surface">The surface of the image to be retrieved.</param>
        /// <param name="page">The page of the image to be retrieved.</param>
        /// <returns>
        /// Returns the content of the image if the item exists
        /// </returns>
        Task<Result<Error, Image>> GetImageForLegacy(
            RequestIdentifier requestId,
            LegacyTarget legacyTarget,
            string tenantId,
            UserId userId,
            IrnId irnId,
            int seqNum,
            ImageSurface surface,
            int page);

        /// <summary>
        /// Finds the requested image and writes it to WebClient.
        /// </summary>
        /// <param name="requestId">The request Id to associate with the log.</param>
        /// <param name="tenantId">The identifier of the tenant to access data from.</param>
        /// <param name="imageId">The identifier of the image to retrieve.</param>
        /// <param name="filepath">The path to where the file should be stored when saving.</param>
        /// <param name="filename">The name the image file to use when saving.</param>
        /// <returns>
        /// Returns info on the tiff if found, otherwise returns <c>null</c>.
        /// </returns>
        Result<Error, ImageTiffInfo> WriteImageToWebClient(
            RequestIdentifier requestId,
            string tenantId,
            ImageId imageId,
            string filepath,
            string filename);

        /// <summary>
        /// Ensures image size is checked and adjusted if needed.
        /// </summary>
        /// <param name="requestId">The request Id to associate with the log.</param>
        /// <param name="imageId">The identifier of the image to check.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="requestId"/> or <paramref name="imageId"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="requestId"/> is <c>RequestIdentifier.Empty</c>
        /// or when <paramref name="imageId"/> is <c>ImageId.Empty</c>.
        /// </exception>
        void VerifyImageSize(RequestIdentifier requestId, ImageId imageId);

        /// <summary>
        /// Ensures the specified <see cref="ImageId"/> is not present in the vault.
        /// </summary>
        /// <param name="requestId">The request Id to associate with the log.</param>
        /// <param name="imageId">The identifier of the image to remove.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="requestId"/> or <paramref name="imageId"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="requestId"/> is <c>RequestIdentifier.Empty</c>
        /// or when <paramref name="imageId"/> is <c>ImageId.Empty</c>.
        /// </exception>
        void RemoveImage(RequestIdentifier requestId, ImageId imageId);
    }
}
