using RDM.Core;

namespace RDM.Services.ImageVault
{
    public interface IBinaryFileReaderWriter
    {
        /// <summary>
        /// Gets the bytes from a file.
        /// </summary>
        /// <param name="pathToFile">The path to file.</param>
        /// <returns>The bytes or an Error</returns>
        Result<Error, byte[]> ReadBytesFromFile(string pathToFile);

        /// <summary>
        /// Gets the bytes from a file stored in an archive.
        /// </summary>
        /// <param name="pathToArchive">The path to archive.</param>
        /// <param name="pathToFile">The path to file.</param>
        /// /// <returns>The bytes or an Error</returns>
        Result<Error, byte[]> ReadBytesFromArchive(string pathToArchive, string pathToFile);

        /// <summary>
        /// Writes a binary file to disk at the specified location
        /// </summary>
        /// <param name="path">The path to write the file to</param>
        /// <param name="content">The binary content to write</param>
        /// <returns>The nothing for success or an Error</returns>
        Result<Error, None> WriteFile(string path, byte[] content);
    }
}
