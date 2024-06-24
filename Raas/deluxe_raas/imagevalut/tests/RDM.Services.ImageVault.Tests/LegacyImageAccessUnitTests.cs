using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using NSubstitute;
using RDM.Core;
using RDM.Data.ImageVault.Legacy;
using RDM.Imaging;
using RDM.Model.Itms;
using Xunit;

namespace RDM.Services.ImageVault.Tests
{
    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Test files are not styled.")]
    [SuppressMessage("StyleCop.CSharp.LayoutRules", "*", Justification = "Test files are not styled.")]
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "*", Justification = "Test files are not styled.")]
    [SuppressMessage("StyleCop.CSharp.OrderingRules", "*", Justification = "Test files are not styled.")]
    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "*", Justification = "Test files are not styled.")]
    [SuppressMessage("StyleCop.CSharp.SpacingRules", "*", Justification = "Test files are not styled.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "Test files are not styled.")]
    public class LegacyImageAccessUnitTests
    {
        private readonly IRequestDataAccessor _requestDataAccessor;
        private readonly IItmsImageRepository _itmsImageRepository;
        private readonly IDictionary<string, IWebClientImageRepository> _webClientImageRepositories;
        private readonly IWebClientImageRepository _webClientImageRepository;
        private readonly IImageFactory _imageFactory;
        private readonly IBinaryFileReaderWriter _fileReaderWriter;
        private readonly IImage _image;

        private readonly LegacyImageAccess _legacyImageAccess;

        public LegacyImageAccessUnitTests()
        {
            _requestDataAccessor = Substitute.For<IRequestDataAccessor>();
            _itmsImageRepository = Substitute.For<IItmsImageRepository>();

            _webClientImageRepository = Substitute.For<IWebClientImageRepository>();
            _webClientImageRepositories = Substitute.For<Dictionary<string, IWebClientImageRepository>>();
            _webClientImageRepositories.Add("Default", _webClientImageRepository);

            _imageFactory = Substitute.For<IImageFactory>();
            _image = Substitute.For<IImage>();
            _imageFactory.CreateImage(Arg.Any<byte[]>()).Returns(_image);

            _fileReaderWriter = Substitute.For<IBinaryFileReaderWriter>();

            _legacyImageAccess = new LegacyImageAccess(
                _requestDataAccessor,
                _itmsImageRepository,
                _webClientImageRepositories,
                _imageFactory,
                _fileReaderWriter);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void GetImageFromItms_ImageStoredInArchive_RetrievesImageFromArchive()
        {
            // Arrange
            var userId = new UserId(1234);
            var irn = new IrnId("EYMG6NTRKU80C04SW4CWWGKW8");
            var seqNum = 1;
            var imageFileInfo = new ItmsImageFileInfo("fileUrl", "filename", 123, "archiveUrl", "archiveFilename");
            var imageBytes = new byte[4];// ImageUtilities.GetBytesFromResourceName("RDM.Services.ImageVault.Tests.large_image.tiff");

            SetUp_ItmsImageRepository_ImageFileInfo(imageFileInfo);
            SetUp_FileReaderWriter_ReadBytesFromArchive(imageBytes);
            SetUp_Image_Properties(format: ImageFormat.Tiff, mimeType: "image/tiff");

            // Act
            var getImageResult = _legacyImageAccess.GetImageFromItms(userId, irn, seqNum, ImageSurface.Front, 0);

            // Assert
            Assert.True(getImageResult.IsSuccess);
            _fileReaderWriter.Received(1).ReadBytesFromArchive(Arg.Any<string>(), Arg.Any<string>());
            _fileReaderWriter.Received(0).ReadBytesFromFile(Arg.Any<string>());
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void GetImageFromItms_ImageNotFoundInArchive_ReturnsFailure()
        {
            // Arrange
            var userId = new UserId(1234);
            var irn = new IrnId("EYMG6NTRKU80C04SW4CWWGKW8");
            var seqNum = 1;
            var imageFileInfo = new ItmsImageFileInfo("fileUrl", "filename", 123, "archiveUrl", "archiveFilename");

            SetUp_ItmsImageRepository_ImageFileInfo(imageFileInfo);
            SetUp_FileReaderWriter_ReadBytesFromArchive(new NotFound());

            // Act
            var getImageResult = _legacyImageAccess.GetImageFromItms(userId, irn, seqNum, ImageSurface.Front, 0);

            // Assert
            Assert.True(getImageResult.IsFailure);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void GetImageFromItms_ImageStoredInShare_RetrievesImageFromFile()
        {
            // Arrange
            var userId = new UserId(1234);
            var irn = new IrnId("EYMG6NTRKU80C04SW4CWWGKW8");
            var seqNum = 1;
            var imageFileInfo = new ItmsImageFileInfo("fileUrl", "filename", 123, Maybe<string>.Empty(), Maybe<string>.Empty());
            var imageBytes = new byte[4];//ImageUtilities.GetBytesFromResourceName("RDM.Services.ImageVault.Tests.large_image.tiff");

            SetUp_ItmsImageRepository_ImageFileInfo(imageFileInfo);
            SetUp_FileReaderWriter_ReadBytesFromFile(imageBytes);
            SetUp_Image_Properties();
            // Act
            var getImageResult = _legacyImageAccess.GetImageFromItms(userId, irn, seqNum, ImageSurface.Front, 0);

            // Assert
            Assert.True(getImageResult.IsSuccess);
            _fileReaderWriter.Received(1).ReadBytesFromFile(Arg.Any<string>());
            _fileReaderWriter.Received(0).ReadBytesFromArchive(Arg.Any<string>(), Arg.Any<string>());
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void GetImageFromItms_ImageNotFoundInShare_ReturnsFailure()
        {
            // Arrange
            var userId = new UserId(1234);
            var irn = new IrnId("EYMG6NTRKU80C04SW4CWWGKW8");
            var seqNum = 1;
            var imageFileInfo = new ItmsImageFileInfo("fileUrl", "filename", 123, Maybe<string>.Empty(), Maybe<string>.Empty());

            SetUp_ItmsImageRepository_ImageFileInfo(imageFileInfo);
            SetUp_FileReaderWriter_ReadBytesFromFile(new NotFound());

            // Act
            var getImageResult = _legacyImageAccess.GetImageFromItms(userId, irn, seqNum, ImageSurface.Front, 0);

            // Assert
            Assert.True(getImageResult.IsFailure);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void GetImageFromWebClient_ImageStoredInShare_RetrievesImageFromFile()
        {
            // Arrange
            var tenantId = "Default";
            var irn = new IrnId("EYMG6NTRKU80C04SW4CWWGKW8");
            var imageFileInfo = new WebClientImageFileInfo("fileUrl", "filename");
            var imageBytes = new byte[4];

            SetUp_WebClientImageRepository_ImageFileInfo(imageFileInfo);
            SetUp_FileReaderWriter_ReadBytesFromFile(imageBytes);
            SetUp_Image_Properties();

            // Act
            var getImageResult = _legacyImageAccess.GetImageFromWebClient(tenantId, irn, ImageSurface.Front, 0);

            // Assert
            Assert.True(getImageResult.IsSuccess);
            _fileReaderWriter.Received(1).ReadBytesFromFile(Arg.Any<string>());
            _fileReaderWriter.Received(0).ReadBytesFromArchive(Arg.Any<string>(), Arg.Any<string>());
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void GetImageFromWebClient_ImageNotFoundInShare_ReturnsFailure()
        {
            // Arrange
            var tenantId = "Default";
            var irn = new IrnId("EYMG6NTRKU80C04SW4CWWGKW8");
            var imageFileInfo = new WebClientImageFileInfo("fileUrl", "filename");

            SetUp_WebClientImageRepository_ImageFileInfo(imageFileInfo);
            SetUp_FileReaderWriter_ReadBytesFromFile(new NotFound());

            // Act
            var getImageResult = _legacyImageAccess.GetImageFromWebClient(tenantId, irn, ImageSurface.Front, 0);

            // Assert
            Assert.True(getImageResult.IsFailure);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void GetImageFromWebClient_ForLegacy_ImageStoredInShare_RetrievesImageFromFile()
        {
            // Arrange
            var tenantId = "Default";
            var userId = new UserId(1234);
            var irn = new IrnId("EYMG6NTRKU80C04SW4CWWGKW8");
            var seqNum = 1;
            var imageFileInfo = new WebClientImageFileInfo("fileUrl", "filename");
            var imageBytes = new byte[4];

            SetUp_WebClientImageRepository_ImageFileInfo(imageFileInfo);
            SetUp_FileReaderWriter_ReadBytesFromFile(imageBytes);
            SetUp_Image_Properties();

            // Act
            var getImageResult = _legacyImageAccess.GetImageFromWebClient(tenantId, userId, irn, seqNum, ImageSurface.Front, 0);

            // Assert
            Assert.True(getImageResult.IsSuccess);
            _fileReaderWriter.Received(1).ReadBytesFromFile(Arg.Any<string>());
            _fileReaderWriter.Received(0).ReadBytesFromArchive(Arg.Any<string>(), Arg.Any<string>());
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void GetImageFromWebClient_ForLegacy_ImageNotFoundInShare_ReturnsFailure()
        {
            // Arrange
            var tenantId = "Default";
            var userId = new UserId(1234);
            var irn = new IrnId("EYMG6NTRKU80C04SW4CWWGKW8");
            var seqNum = 1;
            var imageFileInfo = new WebClientImageFileInfo("fileUrl", "filename");

            SetUp_WebClientImageRepository_ImageFileInfo(imageFileInfo);
            SetUp_FileReaderWriter_ReadBytesFromFile(new NotFound());

            // Act
            var getImageResult = _legacyImageAccess.GetImageFromWebClient(tenantId, userId, irn, seqNum, ImageSurface.Front, 0);

            // Assert
            Assert.True(getImageResult.IsFailure);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void WriteImageToWebClient_TiffImage_ReturnsSuccess()
        {
            // Arrange
            var tenantId = "Default";
            var imageId = new ImageId("2f081e58d50b49b0aa8a884faf674af3");
            var imageBytes = new byte[4];
            var image = new Image(imageBytes, "image/tiff", 5312, 2988);
            var filepath = "C:\\Temp\\";
            var filename = "IRN_SeqNum_SheetNumber_Surface.tif";

            SetUp_FileReaderWriter_WriteFile(None.Value);

            // Act
            var reply = _legacyImageAccess.WriteImageToWebClient(image, imageId, tenantId, filepath, filename);

            // Assert
            Assert.True(reply.IsSuccess);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void WriteImageToWebClient_FileAlreadyExists_ThrowsException()
        {
            // Arrange
            var tenantId = "Default";
            var imageId = new ImageId("2f081e58d50b49b0aa8a884faf674af3");
            var imageBytes = new byte[4];
            var image = new Image(imageBytes, "image/tiff", 5312, 2988);
            var filepath = "C:\\Temp\\";
            var filename = "IRN_SeqNum_SheetNumber_Surface.tif";

            SetUp_FileReaderWriter_WriteFile(new ServiceFailure("A file at this path already exists."));

            // Act, Assert
            Assert.ThrowsAny<Exception>(
                () => _legacyImageAccess.WriteImageToWebClient(image, imageId, tenantId, filepath, filename));
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void WriteImageToWebClient_NonTiffImage_ConvertsToTiff()
        {
            // Arrange
            var tenantId = "Default";
            var imageId = new ImageId("2f081e58d50b49b0aa8a884faf674af3");
            var imageBytes = new byte[4];
            var image = new Image(imageBytes, "image/jpeg", 5312, 2988);
            var tiffBytes = new byte[4];
            var filepath = "C:\\Temp\\";
            var filename = "IRN_SeqNum_SheetNumber_Surface.tif";

            SetUp_ImageFactory_SetFormatToTiff(tiffBytes);
            SetUp_FileReaderWriter_WriteFile(None.Value);
            SetUp_Image_Properties();

            // Act
            var reply = _legacyImageAccess.WriteImageToWebClient(image, imageId, tenantId, filepath, filename);

            // Assert
            _image.Received(1).SetFormat(ImageFormat.Tiff);
            Assert.True(reply.IsSuccess);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void ConvertWindowsSharePathToLinuxMountPath_UncPath_ReturnsLinuxMountPath()
        {
            // Arrange
            var uncPath = @"\\host-name\share-name\file-path";
            var expectedPath = "/mnt/host-name/share-name/file-path";

            // Act
            var linuxPath = _legacyImageAccess.ConvertWindowsSharePathToLinuxMountPath(uncPath);

            // Assert
            Assert.Equal(expectedPath, linuxPath);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void ExtractLinuxFileSharePath_FullLinuxPath_ReturnsPathToShareDirectory()
        {
            // Arrange
            var fullPath = "/mnt/host-name/share-name/some/random/directory";
            var expectedPath = "/mnt/host-name/share-name";

            // Act
            var sharePath = _legacyImageAccess.ExtractLinuxFileSharePath(fullPath);

            // Assert
            Assert.True(sharePath.HasValue);
            Assert.Equal(expectedPath, sharePath.Value);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void ExtractLinuxFileSharePath_LinuxPathNotLongEnough_ReturnsNull()
        {
            // Arrange
            var shortPath = "/mnt/host-name/";

            // Act
            var sharePath = _legacyImageAccess.ExtractLinuxFileSharePath(shortPath);

            // Assert
            Assert.False(sharePath.HasValue);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void ExtractLinuxFileSharePath_UncPath_ReturnsNull()
        {
            // Arrange
            var shortPath = @"\\host-name\share-name\some\random\directory";

            // Act
            var sharePath = _legacyImageAccess.ExtractLinuxFileSharePath(shortPath);

            // Assert
            Assert.False(sharePath.HasValue);
        }

        private void SetUp_ItmsImageRepository_ImageFileInfo(Result<Error, ItmsImageFileInfo> result)
        {
            _itmsImageRepository.GetImageFileInfo(
                                    Arg.Any<UserId>(),
                                    Arg.Any<string>(),
                                    Arg.Any<int>(),
                                    Arg.Any<string>(),
                                    Arg.Any<int>())
                                .Returns(result);
        }

        private void SetUp_WebClientImageRepository_ImageFileInfo(Result<Error, WebClientImageFileInfo> result)
        {
            _webClientImageRepository.GetImageFileInfo(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<int>()).Returns(result);

            _webClientImageRepository.GetImageFileInfo(
                                         Arg.Any<UserId>(),
                                         Arg.Any<string>(),
                                         Arg.Any<int>(),
                                         Arg.Any<string>(),
                                         Arg.Any<int>())
                                     .Returns(result);
        }

        private void SetUp_FileReaderWriter_ReadBytesFromArchive(Result<Error, byte[]> result)
        {
            _fileReaderWriter.ReadBytesFromArchive(Arg.Any<string>(), Arg.Any<string>()).Returns(result);
        }

        private void SetUp_FileReaderWriter_ReadBytesFromFile(Result<Error, byte[]> result)
        {
            _fileReaderWriter.ReadBytesFromFile(Arg.Any<string>()).Returns(result);
        }

        private void SetUp_FileReaderWriter_WriteFile(Result<Error, None> result)
        {
            _fileReaderWriter.WriteFile(Arg.Any<string>(), Arg.Any<byte[]>()).Returns(result);
        }

        private void SetUp_ImageFactory_SetFormatToTiff(byte[] tiffBytes)
        {
            _image.ToByteArray().Returns(tiffBytes);
        }

        private void SetUp_Image_Properties(
            int width = 96,
            int height = 96,
            ImageFormat format = ImageFormat.Jpeg,
            string mimeType = "image/jpeg")
        {
            _image.Height.Returns(height);
            _image.Width.Returns(width);
            _image.Format.Returns(format);
            _image.MimeType.Returns(mimeType);
        }

    }
}
