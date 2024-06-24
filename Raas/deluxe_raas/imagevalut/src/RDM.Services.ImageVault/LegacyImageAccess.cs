using System;
using System.Collections.Generic;
using System.IO;
using RDM.Core;
using RDM.Data.ImageVault.Legacy;
using RDM.Imaging;
using RDM.Model.ImageVault;
using RDM.Model.Itms;
using RDM.Statistician;

namespace RDM.Services.ImageVault
{
    /// <summary>
    /// A class which attempts to hide all of the nasty legacy-logic from the main service object.
    /// Read and write images from the different areas in our legacy system.
    /// Because it is indeed that complicated.
    /// </summary>
    /// <seealso cref="ILegacyImageAccess" />
    public class LegacyImageAccess : ILegacyImageAccess
    {
        private readonly string _linuxMountPrefix = "/mnt/";

        private readonly IRequestDataAccessor _requestDataAccessor;
        private readonly IItmsImageRepository _itmsImageRepository;
        private readonly IDictionary<string, IWebClientImageRepository> _webClientImageRepositories;
        private readonly IImageFactory _imageFactory;
        private readonly IBinaryFileReaderWriter _fileReaderWriter;

        public LegacyImageAccess(
            IRequestDataAccessor requestDataAccessor,
            IItmsImageRepository itmsImageRepository,
            IDictionary<string, IWebClientImageRepository> webClientImageRepositories,
            IImageFactory imageFactory,
            IBinaryFileReaderWriter fileReaderWriter)
        {
            Contract.Requires<ArgumentNullException>(requestDataAccessor != null, nameof(requestDataAccessor));
            Contract.Requires<ArgumentNullException>(itmsImageRepository != null, nameof(itmsImageRepository));
            Contract.Requires<ArgumentNullException>(imageFactory != null, nameof(imageFactory));
            Contract.Requires<ArgumentNullException>(fileReaderWriter != null, nameof(fileReaderWriter));

            _requestDataAccessor = requestDataAccessor;
            _itmsImageRepository = itmsImageRepository;
            _webClientImageRepositories = webClientImageRepositories;
            _imageFactory = imageFactory;
            _fileReaderWriter = fileReaderWriter;
        }

        public Result<Error, Image> GetImageFromItms(UserId userId, IrnId irn, int seqNum, ImageSurface surface, int page)
        {
            Contract.Requires<ArgumentNullException>(irn != null, nameof(irn));
            using (Factory.TraceLogger?.Tracer.BuildSpan("GetImageFromItms")
                          .WithTag(OpenTracing.Tag.Tags.Component, "RDM.Services.ImageVault.LegacyImageAccess")
                          .StartActive())
            {
                // Reading images out of ITMS:
                // 1) Retrieve the file information using the IRN, surface, and page number.
                //      - File information may or may not contain archive file information.
                //      - If archive file information is contained then we must extract the image from the archive instead.
                // 2) File information retrieved from the database will be stored as Window URLs so we must update them to Linux URLs if needed.
                //      - If we convert the URLs to a Linux format then we also want to check to make sure that the mount is reachable.
                // 3) Read the bytes with the provided file/archive information.
                // 4) ITMS does not store the width or the height which we need so we must pack the bytes into an imaging library to retrieve this.

                return _itmsImageRepository.GetImageFileInfo(userId, irn.ToString(), seqNum, surface.ToString(), page)
                                           .OnSuccess(EnsureFileInfoMatchesEnvironment)
                                           .OnSuccess(ReadBytesFromItms)
                                           .OnSuccess(ConvertBytesToImage);
            }
        }

        public Result<Error, Image> GetImageFromWebClient(string tenantId, IrnId irn, ImageSurface surface, int page)
        {
            Contract.Requires<ArgumentNullException>(irn != null, nameof(irn));

            // Reading images out of WebClient:
            // 1) Retrieve the file information using the IRN, surface, and page number.
            // 2) File information retrieved from the database will be stored as Window URLs so we must update them to Linux URLs if needed.
            //      - If we convert the URLs to a Linux format then we also want to check to make sure that the mount is reachable.
            // 3) Read the bytes with the provided file information.
            // 4) Create an image object to return back.

            return _webClientImageRepositories[tenantId]
                   .GetImageFileInfo(irn.ToString(), surface.ToString(), page)
                   .OnSuccess(EnsureFileInfoMatchesEnvironment)
                   .OnSuccess(ReadBytesFromWebClient)
                   .OnSuccess(ConvertBytesToImage);
        }

        public Result<Error, Image> GetImageFromWebClient(
            string tenantId,
            UserId userId,
            IrnId irn,
            int seqNum,
            ImageSurface surface,
            int page)
        {
            Contract.Requires<ArgumentNullException>(irn != null, nameof(irn));

            // Reading images out of WebClient:
            // 1) Retrieve the file information using the IRN, surface, and page number.
            // 2) File information retrieved from the database will be stored as Window URLs so we must update them to Linux URLs if needed.
            //      - If we convert the URLs to a Linux format then we also want to check to make sure that the mount is reachable.
            // 3) Read the bytes with the provided file information.
            // 4) Create an image object to return back.

            return _webClientImageRepositories[tenantId]
                   .GetImageFileInfo(userId, irn.ToString(), seqNum, surface.ToString(), page)
                   .OnSuccess(EnsureFileInfoMatchesEnvironment)
                   .OnSuccess(ReadBytesFromWebClient)
                   .OnSuccess(ConvertBytesToImage);
        }

        public Result<Error, ImageTiffInfo> WriteImageToWebClient(
            Image image,
            ImageId imageId,
            string tenantId,
            string filepath,
            string filename)
        {
            Contract.Requires<ArgumentNullException>(image != null, nameof(image));
            Contract.Requires<ArgumentNullException>(imageId != null, nameof(imageId));
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(tenantId), nameof(tenantId));
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(filepath), nameof(filepath));
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(filename), nameof(filename));

            // Writing images into WebClient:
            // 1) Images must be stored in tiff format. Convert the image to tiff if needed.
            // 2) Get the storage path for the image from the database.
            // 3) Determine the name of the file using the image ID.
            // 4) Ensure the path matches the current environment.
            // 5) Write the image to the determined path.
            //      - If we fail when trying to write the image we will throw an exception to elevate this error.
            //      - Normally the only way writing can fail at this point is when a file with the same name already exists.
            // 6) Return information on the tiff image that was written.
            //      - ***NOTE*** The URL stored in the tiff information must be the original Windows URL from step 2.

            var tiffImage = ConvertToTiffImage(image);

            var fileInfo = new WebClientImageFileInfo(filepath, filename);

            return EnsureFileInfoMatchesEnvironment(fileInfo)
                   .OnSuccess(info => WriteBytesToWebClient(info, tiffImage.Content).OnFailure(ThrowWriteImageException))
                   .OnSuccess(
                       () => new ImageTiffInfo(
                           imageId,
                           filename,
                           filepath,
                           tiffImage.Content.Length,
                           tiffImage.Width,
                           tiffImage.Height));
        }

        internal string ConvertWindowsSharePathToLinuxMountPath(string path)
        {
            // UNC is the naming system used for accessing Windows shared networks. It has the following syntax:
            // \\host-name\share-name\file-path
            // It is a Linux convention to store mounts within the /mnt/ directory so we must change it like so:
            // /mnt/host-name/share-name/file-path
            return path.Replace(@"\\", _linuxMountPrefix).Replace(@"\", "/");
        }

        internal Maybe<string> ExtractLinuxFileSharePath(string path)
        {
            // The path to the file share mounted on linux will have the following format:
            // /mnt/host-name/share-name/*
            // Attempt to derive the path to the share directory. If that path can not be derived then return null

            var slashes = 0;
            for (var i = 0; i < path.Length; i++)
            {
                if (path[i] == '/' && ++slashes == 4)
                {
                    return path.Substring(0, i);
                }
            }

            return Maybe<string>.Empty();
        }

        // Ensures the path matches the environment. All file information is stored in the DB as Windows paths.
        // If our services are running on Linux we need to adjust them accordingly.       
        private Result<Error, ItmsImageFileInfo> EnsureFileInfoMatchesEnvironment(ItmsImageFileInfo fileInfo)
        {
            try
            {
                _requestDataAccessor.PerformanceMonitor?.Start("LegacyImageAccess.EnsureFileInfoMatchesEnvironment");
                using (Factory.TraceLogger?.Tracer.BuildSpan("EnsureFileInfoMatchesEnvironment")
                              .WithTag(OpenTracing.Tag.Tags.Component, "RDM.Services.ImageVault.LegacyImageAccess")
                              .StartActive())
                {
                    // If we're not running under Linux then don't bother
                    if (Environment.OSVersion.Platform != PlatformID.Unix)
                    {
                        return fileInfo;
                    }

                    var adjustedArchiveUrl = fileInfo.IsFileStoredInArchive
                        ? ConvertWindowsSharePathToLinuxMountPath(fileInfo.ArchiveUrl.Value)
                        : Maybe<string>.Empty();

                    return new ItmsImageFileInfo(
                        ConvertWindowsSharePathToLinuxMountPath(fileInfo.FileUrl),
                        fileInfo.Filename,
                        fileInfo.FileId,
                        adjustedArchiveUrl,
                        fileInfo.ArchiveFilename);
                }
            }
            finally
            {
                _requestDataAccessor.PerformanceMonitor?.Stop("LegacyImageAccess.EnsureFileInfoMatchesEnvironment");
            }
        }

        // Ensures the path matches the environment. All file information is stored in the DB as Windows-ready paths.
        // If our services are running on Linux we need to adjust them accordingly.
        private Result<Error, WebClientImageFileInfo> EnsureFileInfoMatchesEnvironment(WebClientImageFileInfo fileInfo)
        {
            _requestDataAccessor.PerformanceMonitor?.Start("LegacyImageAccess.EnsureFileInfoMatchesEnvironment");

            // If we're not running under Linux then don't bother
            if (Environment.OSVersion.Platform != PlatformID.Unix)
            {
                return fileInfo;
            }

            // Adjust both file and archive URLs
            var adjustedFileUrl = ConvertWindowsSharePathToLinuxMountPath(fileInfo.FileUrl);

            _requestDataAccessor.PerformanceMonitor?.Stop("LegacyImageAccess.EnsureFileInfoMatchesEnvironment");

            return new WebClientImageFileInfo(adjustedFileUrl, fileInfo.Filename);
        }

        private Result<Error, byte[]> ReadBytesFromItms(ItmsImageFileInfo fileInfo)
        {
            _requestDataAccessor.PerformanceMonitor?.Start("LegacyImageAccess.ReadBytesFromItms");
            using (Factory.TraceLogger?.Tracer.BuildSpan("ReadBytesFromItms")
                          .WithTag(OpenTracing.Tag.Tags.Component, "RDM.Services.ImageVault.LegacyImageAccess")
                          .StartActive())
            {
                if (fileInfo.IsFileStoredInArchive)
                {
                    var archivePath = Path.Combine(fileInfo.ArchiveUrl.Value, fileInfo.ArchiveFilename.Value);
                    var filePath = $"{fileInfo.FileId.ToString()}/{fileInfo.Filename}";

                    return _fileReaderWriter.ReadBytesFromArchive(archivePath, filePath);
                }

                var path = Path.Combine(fileInfo.FileUrl, fileInfo.Filename);

                _requestDataAccessor.PerformanceMonitor?.Stop("LegacyImageAccess.ReadBytesFromItms");

                return _fileReaderWriter.ReadBytesFromFile(path);
            }
        }

        private Result<Error, byte[]> ReadBytesFromWebClient(WebClientImageFileInfo fileInfo)
        {
            _requestDataAccessor.PerformanceMonitor?.Start("LegacyImageAccess.ReadBytesFromWebClient");

            var path = Path.Combine(fileInfo.FileUrl, fileInfo.Filename);

            _requestDataAccessor.PerformanceMonitor?.Stop("LegacyImageAccess.ReadBytesFromWebClient");

            return _fileReaderWriter.ReadBytesFromFile(path);
        }

        private Result<Error, Image> ConvertBytesToImage(byte[] imageContent)
        {
            _requestDataAccessor.PerformanceMonitor?.Start("LegacyImageAccess.ConvertBytesToImage");
            using (Factory.TraceLogger?.Tracer.BuildSpan("ConvertBytesToImage")
                          .WithTag(OpenTracing.Tag.Tags.Component, "RDM.Services.ImageVault.LegacyImageAccess")
                          .StartActive())
            {
                // ITMS doesn't store the image's width and height in its version of the tblImage table.
                // In order to get these values we need to put the bytes[] into an image library.
                try
                {
                    using(var image = _imageFactory.CreateImage(imageContent))
                    {
                        return new Image(imageContent, image.MimeType, image.Width, image.Height);
                    }
                }
                catch (ImageException)
                {
                    return new ServiceFailure(
                        "LegacyImageAccess was unable to turn the byte array into an image. Please contact a developer about this issue.");
                }
                finally
                {
                    _requestDataAccessor.PerformanceMonitor?.Stop("LegacyImageAccess.ConvertBytesToImage");
                }
            }
        }

        private Result<Error, None> WriteBytesToWebClient(WebClientImageFileInfo fileInfo, byte[] bytes)
        {
            var path = Path.Combine(fileInfo.FileUrl, fileInfo.Filename);

            return _fileReaderWriter.WriteFile(path, bytes);
        }

        private Image ConvertToTiffImage(Image imageInfo)
        {
            _requestDataAccessor.PerformanceMonitor?.Start("LegacyImageAccess.ConvertToTiffImage");
            try
            {
                if (imageInfo.MimeType == ImageVaultService.TiffMimeType)
                {
                    return imageInfo;
                }

                using (var image = _imageFactory.CreateImage(imageInfo.Content))
                {
                    image.SetFormat(ImageFormat.Tiff);
                    var tiffBytes = image.ToByteArray();

                    return new Image(tiffBytes, ImageVaultService.TiffMimeType, image.Width, image.Height);
                }
            }
            finally
            {
                _requestDataAccessor.PerformanceMonitor?.Stop("LegacyImageAccess.ConvertToTiffImage");
            }
        }

        private void ThrowWriteImageException(Error err)
        {
            throw new InvalidOperationException(
                $"LegacyImageAccess failed to write the image to disk with error '{err.Message}'. Please contact IT about this issue.");
        }
    }
}
