using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Reflection;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using RDM.Core;
using RDM.Model.Itms;
using Xunit;
using Xunit.Abstractions;
using Serilog;

namespace RDM.Services.ImageVault.Tests
{
    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Test files are not styled.")]
    [SuppressMessage("StyleCop.CSharp.LayoutRules", "*", Justification = "Test files are not styled.")]
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "*", Justification = "Test files are not styled.")]
    [SuppressMessage("StyleCop.CSharp.OrderingRules", "*", Justification = "Test files are not styled.")]
    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "*", Justification = "Test files are not styled.")]
    [SuppressMessage("StyleCop.CSharp.SpacingRules", "*", Justification = "Test files are not styled.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "Test files are not styled.")]
    public class BinaryFileReaderWriterUnitTests
    {
        private readonly IRequestDataAccessor _requestDataAccessor;
        private readonly FileBase _fileBase;
        private readonly DirectoryBase _directoryBase;
        private readonly PathBase _pathBase;
        private readonly IBinaryFileReaderWriter _readerWriter;
        private readonly IDateTime _dateTime;
        private readonly ILogger _logger;

        public BinaryFileReaderWriterUnitTests(ITestOutputHelper output)
        {
            // Pass the ITestOutputHelper object to the TestOutput sink
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.TestOutput(output)
                .CreateLogger();

            _requestDataAccessor = Substitute.For<IRequestDataAccessor>();

            _pathBase = Substitute.For<PathBase>();
            _fileBase = Substitute.For<FileBase>();
            _directoryBase = Substitute.For<DirectoryBase>();

            _dateTime = Substitute.For<IDateTime>();

            _logger = Substitute.For<ILogger>();

            var fileSystem = Substitute.For<IFileSystem>();
            fileSystem.Path.Returns(_pathBase);
            fileSystem.File.Returns(_fileBase);
            fileSystem.Directory.Returns(_directoryBase);

            _readerWriter = new BinaryFileReaderWriter(_requestDataAccessor, fileSystem, _dateTime, _logger);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void ReadBytesFromFile_FileExists_ReturnsExpectedBytes()
        {
            // Arrange
            var path = "test/path.jpg";
            var expectedBytes = new byte[] { 1, 2, 3, 4 };

            SetUp_FileSystem_FileReadAllBytes(path, expectedBytes);

            // Act
            var result = _readerWriter.ReadBytesFromFile(path);

            // Assert
            Log.Information($"Is success: {result.IsSuccess}");
            Assert.True(result.IsSuccess);
            Log.Information($"Comparing sequence");
            Assert.True(expectedBytes.SequenceEqual(result.Value));
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void ReadBytesFromFile_FileDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            var path = "test/path.jpg";

            SetUp_FileSystem_FileReadAllBytes_Exception(path, new FileNotFoundException());

            // Act
            var result = _readerWriter.ReadBytesFromFile(path);

            // Assert
            Assert.True(result.IsFailure);
            Assert.IsType<NotFound>(result.FailureValue);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void ReadBytesFromArchive_ArchiveAndFileExists_ReturnsExpectedBytes()
        {
            // Arrange
            // This is a terribly written test but it does work. If someone else wants to abstract zip compression correctly then please be my guest :)
            var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var pathToArchive = Path.Combine(assemblyPath.Split(Path.DirectorySeparatorChar + "bin" + Path.DirectorySeparatorChar)[0], "archive_test.zip");
            var pathToFile = "file/sample.jpg";
            var expectedBytes = ImageUtilities.GetBytesFromResourceName("RDM.Services.ImageVault.Tests.sample.jpg");

            // Act
            var result = _readerWriter.ReadBytesFromArchive(pathToArchive, pathToFile);

            // Assert
            Log.Information($"Is success: {result.IsSuccess}");
            Assert.True(result.IsSuccess);
            Log.Information($"Comparing sequence");
            Assert.True(expectedBytes.SequenceEqual(result.Value));
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void ReadBytesFromArchive_ArchiveDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            var pathToArchive = "path/to/archive.zip";
            var pathToFile = "path/to/file.jpg";

            SetUp_FileSystem_FileReadAllBytes_Exception(pathToArchive, new FileNotFoundException());

            // Act
            var result = _readerWriter.ReadBytesFromArchive(pathToArchive, pathToFile);

            // Assert
            Assert.True(result.IsFailure);
            Assert.IsType<NotFound>(result.FailureValue);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void ReadBytesFromArchive_ArchiveExistsButFileDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            // This is a terribly written test but it does work. If someone else wants to abstract zip compression correctly then please be my guest :)
            var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var pathToArchive = Path.Combine(assemblyPath.Split("\\bin\\")[0], "archive_test.zip");
            var pathToFile = "doesnotexist/sample.jpg";


            // Act
            var result = _readerWriter.ReadBytesFromArchive(pathToArchive, pathToFile);

            // Assert
            Assert.True(result.IsFailure);
            Assert.IsType<NotFound>(result.FailureValue);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void WriteFile_DirectoryExists_FileWrittenToCorrectPath()
        {
            // Arrange
            var path = "test/path.jpg";
            var directory = "test/";
            var bytes = new byte[] { 1, 2, 3, 4 };

            SetUp_FileSystem_PathGetDirectoryName(path, directory);
            SetUp_FileSystem_DirectoryExists(directory, true);
            SetUp_FileSystem_FileExists(path, false);

            // Act
            var result = _readerWriter.WriteFile(path, bytes);

            // Assert
            Assert.True(result.IsSuccess);
            _fileBase.Received(1).WriteAllBytes(path, bytes);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void WriteFile_DirectoryDoesNotExist_DirectoryCreatedAndFileWrittenToCorrectPath()
        {
            // Arrange
            var path = "test/path.jpg";
            var directory = "test/";
            var bytes = new byte[] { 1, 2, 3, 4 };

            SetUp_FileSystem_PathGetDirectoryName(path, directory);
            SetUp_FileSystem_DirectoryExists(directory, false);
            SetUp_FileSystem_FileExists(path, false);

            // Act
            var result = _readerWriter.WriteFile(path, bytes);

            // Assert
            Assert.True(result.IsSuccess);
            _directoryBase.Received(1).CreateDirectory(directory);
            _fileBase.Received(1).WriteAllBytes(path, bytes);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void WriteFile_FileAlreadyExists_RenameFile()
        {
            // Arrange
            var dateTime = DateTime.Now;
            var duplicatePath = "test/path.jpg";
            var renamedPath = "test/path." + dateTime.ToString("HHmmss");
            var directory = "test/";
            var bytes = new byte[] { 1, 2, 3, 4 };

            SetUp_DateTime_Now(dateTime);
            SetUp_FileSystem_PathGetDirectoryName(duplicatePath, directory);
            SetUp_FileSystem_DirectoryExists(directory, true);
            SetUp_FileSystem_FileExists(duplicatePath, true);
            SetUp_FileSystem_FileExists(renamedPath, false);

            // Act
            var result = _readerWriter.WriteFile(duplicatePath, bytes);

            // Assert
            Assert.True(result.IsSuccess);
            _fileBase.Received(1).Exists(duplicatePath);
            _fileBase.Received(1).Move(duplicatePath, renamedPath);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void WriteFile_MultiFilesAlreadyExists_RenameFile()
        {
            // Arrange
            var rootPath = "test/path.";
            var dateTime = DateTime.Now;
            var duplicatePath = rootPath + "jpg";
            var duplicatePath2 = rootPath + dateTime.ToString("HHmmss");
            int duplicateFileCount = 10;
            var renamedPath  = rootPath + dateTime.ToString("HHmmss") + (duplicateFileCount + 1);

            List<string> duplicateFiles = new List<string>();
            for (int ii = 1; ii <= duplicateFileCount; ii++)
            {
                duplicateFiles.Add(rootPath + dateTime.ToString("HHmmss") + ii);
            }

            var directory = "test/";
            var bytes = new byte[] { 1, 2, 3, 4 };

            SetUp_DateTime_Now(dateTime);
            SetUp_FileSystem_PathGetDirectoryName(duplicatePath, directory);
            SetUp_FileSystem_DirectoryExists(directory, true);

            SetUp_FileSystem_FileExists(duplicatePath, true);
            SetUp_FileSystem_FileExists(duplicatePath2, true);
            foreach ( string fpath in duplicateFiles)
            {
                SetUp_FileSystem_FileExists(fpath, true);
            }

            SetUp_FileSystem_FileExists(renamedPath, false);

            // Act
            var result = _readerWriter.WriteFile(duplicatePath, bytes);

            // Assert
            Assert.True(result.IsSuccess);
            foreach (string fpath in duplicateFiles)
            {
                _fileBase.Received(1).Exists(fpath);
            }
            _fileBase.Received(1).Move(duplicatePath, renamedPath);
        }

        private void SetUp_DateTime_Now(DateTime dateTime)
        {
            _dateTime.Now().Returns(dateTime);
        }
        private void SetUp_FileSystem_FileExists(string path, bool exists)
        {
            _fileBase.Exists(path).Returns(exists);
        }

        private void SetUp_FileSystem_FileReadAllBytes_Exception(string path, Exception exception)
        {
            _fileBase.ReadAllBytes(path).Throws(exception);
        }

        private void SetUp_FileSystem_FileReadAllBytes(string path, byte[] bytes)
        {
            _fileBase.ReadAllBytes(path).Returns(bytes);
        }

        private void SetUp_FileSystem_DirectoryExists(string path, bool exists)
        {
            _directoryBase.Exists(path).Returns(exists);
        }

        private void SetUp_FileSystem_PathGetDirectoryName(string path, string directoryName)
        {
            _pathBase.GetDirectoryName(path).Returns(directoryName);
        }
    }
}
