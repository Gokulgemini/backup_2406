using System;
using RDM.Core;

namespace RDM.Data.ImageVault.Legacy
{
    /// <summary>
    /// Contains information on where an image is stored in ITMS.
    /// </summary>
    public class ItmsImageFileInfo : IEquatable<ItmsImageFileInfo>
    {
        public ItmsImageFileInfo(string fileUrl, string filename, int fileId, Maybe<string> archiveUrl, Maybe<string> archiveFilename)
        {
            FileUrl = fileUrl;
            Filename = filename;
            FileId = fileId;
            ArchiveUrl = archiveUrl;
            ArchiveFilename = archiveFilename;
        }

        public string FileUrl { get; }

        public string Filename { get; }

        public int FileId { get; }

        public Maybe<string> ArchiveUrl { get; }

        public Maybe<string> ArchiveFilename { get; }

        public bool IsFileStoredInArchive => ArchiveUrl.HasValue && ArchiveFilename.HasValue;

        public bool Equals(ItmsImageFileInfo other)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }

            if (ReferenceEquals(other, this))
            {
                return true;
            }

            return string.CompareOrdinal(FileUrl, other.FileUrl) == 0
                && string.CompareOrdinal(Filename, other.Filename) == 0
                && FileId == other.FileId
                && IsFileStoredInArchive == other.IsFileStoredInArchive
                && (!IsFileStoredInArchive || ArchiveFilename.Value == other.ArchiveFilename.Value)
                && (!IsFileStoredInArchive || ArchiveUrl.Value == other.ArchiveUrl.Value);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ItmsImageFileInfo);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var result = FileUrl.GetHashCode();
                result = (result * 31) + Filename.GetHashCode();
                result = (result * 31) + FileId.GetHashCode();
                result = (result * 31) + (ArchiveFilename.HasValue ? ArchiveUrl.Value.GetHashCode() : 0);
                result = (result * 31) + (ArchiveUrl.HasValue ? ArchiveUrl.Value.GetHashCode() : 0);

                return result;
            }
        }
    }
}
