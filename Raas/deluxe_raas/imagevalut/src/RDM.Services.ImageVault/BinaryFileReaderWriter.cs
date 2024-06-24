using System;
using System.IO;
using System.IO.Abstractions;
using RDM.Core;
using RDM.Model.Itms;
using Serilog;

namespace RDM.Services.ImageVault
{
    /// <summary>
    /// An implementation of the <see cref="IBinaryFileReaderWriter"/> interface that uses .NET's standard IO functionality.
    /// </summary>
    /// <seealso cref="IBinaryFileReaderWriter" />
    public class BinaryFileReaderWriter : IBinaryFileReaderWriter
    {
        private readonly IFileSystem _fileSystem;
        private readonly IRequestDataAccessor _requestDataAccessor;
        private readonly IDateTime _dateTime;
        private readonly ILogger _logger;

        public BinaryFileReaderWriter(IRequestDataAccessor requestDataAccessor, IFileSystem fileSystem, IDateTime dateTime, ILogger logger)
        {
            Contract.Requires<ArgumentNullException>(requestDataAccessor != null, nameof(requestDataAccessor));
            Contract.Requires<ArgumentNullException>(fileSystem != null, nameof(fileSystem));
            Contract.Requires<ArgumentNullException>(dateTime != null, nameof(dateTime));
            Contract.Requires<ArgumentNullException>(logger != null, nameof(logger));

            _requestDataAccessor = requestDataAccessor;
            _fileSystem = fileSystem;
            _dateTime = dateTime;
            _logger = logger;
        }

        /// <inheritdoc />
        public Result<Error, byte[]> ReadBytesFromFile(string pathToFile)
        {
            Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(pathToFile), nameof(pathToFile));

            try
            {
                _requestDataAccessor.PerformanceMonitor?.Start("BinaryFileReaderWriter.ReadBytesFromFile");
                try
                {
                    return _fileSystem.File.ReadAllBytes(pathToFile);
                }
                catch (DirectoryNotFoundException)
                {
                    return new NotFound(
                        $"Directory '{pathToFile}' does not exist. Please contact deployment about this issue");
                }
                catch (FileNotFoundException)
                {
                    return new NotFound($"File '{pathToFile}' was not found. Please contact deployment about this issue.");
                }
            }
            finally
            {
                _requestDataAccessor.PerformanceMonitor?.Stop("BinaryFileReaderWriter.ReadBytesFromFile");
            }
        }

        /// <inheritdoc />
        public Result<Error, byte[]> ReadBytesFromArchive(string pathToArchive, string pathToFile)
        {
            Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(pathToArchive), nameof(pathToArchive));
            Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(pathToFile), nameof(pathToFile));

            try
            {
                _requestDataAccessor.PerformanceMonitor?.Start("BinaryFileReaderWriter.ReadBytesFromArchive");
                try
                {
                    var archive = System.IO.Compression.ZipFile.OpenRead(pathToArchive);
                    var entry = archive.GetEntry(pathToFile);

                    if (entry == null)
                    {
                        return new NotFound(
                            $"Unable to read from archive. The file '{pathToFile}' could not be found in the archive '{pathToArchive}'.");
                    }

                    using (var stream = entry.Open())
                    using (var ms = new MemoryStream())
                    {
                        stream.CopyTo(ms);

                        return ms.ToArray();
                    }
                }
                catch (DirectoryNotFoundException)
                {
                    return new NotFound(
                        $"Directory '{pathToFile}' does not exist. Please contact deployment about this issue");
                }
                catch (FileNotFoundException)
                {
                    return new NotFound($"File '{pathToFile}' was not found. Please contact deployment about this issue.");
                }
            }
            finally
            {
                _requestDataAccessor.PerformanceMonitor?.Stop("BinaryFileReaderWriter.ReadBytesFromArchive");
            }
        }

        /// <inheritdoc />
        public Result<Error, None> WriteFile(string pathToFile, byte[] content)
        {
            try
            {
                _requestDataAccessor.PerformanceMonitor?.Start("BinaryFileReaderWriter.WriteFile");

                // create folder if needed
                var directory = _fileSystem.Path.GetDirectoryName(pathToFile);
                if (!_fileSystem.Directory.Exists(directory))
                {
                    _fileSystem.Directory.CreateDirectory(directory);
                }

                if (_fileSystem.File.Exists(pathToFile))
                {
                    RenameFile(pathToFile);
                }

                _fileSystem.File.WriteAllBytes(pathToFile, content);

                return None.Value;
            }
            finally
            {
                _requestDataAccessor.PerformanceMonitor?.Stop("BinaryFileReaderWriter.WriteFile");
            }
        }

        private Result<Error, None> RenameFile(string pathToFile)
        {
            try
            {
                _requestDataAccessor.PerformanceMonitor?.Start("BinaryFileReaderWriter.RenameFile");

                var extension = _dateTime.Now().ToString("HHmmss");
                var pathToNewFile = Path.ChangeExtension(pathToFile, extension);

                var dupplicateCount = 0;
                while (_fileSystem.File.Exists(pathToNewFile))
                {
                    dupplicateCount++;
                    pathToNewFile = Path.ChangeExtension(pathToFile, extension + dupplicateCount);
                }

                _fileSystem.File.Move(pathToFile, pathToNewFile);

                return None.Value;
            }
            catch (DirectoryNotFoundException)
            {
                _logger.Fatal($"BinaryFileReaderWriter.RenameFile -- Directory '{pathToFile}' does not exist.");

                return new NotFound(
                    $"BinaryFileReaderWriter.RenameFile -- Directory '{pathToFile}' does not exist. Please contact deployment about this issue");
            }
            catch (FileNotFoundException)
            {
                _logger.Fatal($"BinaryFileReaderWriter.RenameFile -- File '{pathToFile}' was not found.");

                return new NotFound($"BinaryFileReaderWriter.RenameFile -- File '{pathToFile}' was not found. Please contact deployment about this issue.");
            }
            catch (Exception ex)
            {
                _logger.Fatal(ex,$"BinaryFileReaderWriter.RenameFile ran into an exception when attempting to rename file '{pathToFile}'. Error --> {ex}");

                return new ServiceFailure($"BinaryFileReaderWriter.RenameFile ran into an exception when attempting to rename file '{pathToFile}'. Error --> {ex}");
            }
            finally
            {
                _requestDataAccessor.PerformanceMonitor?.Stop("BinaryFileReaderWriter.RenameFile");
            }
        }

    }
}
