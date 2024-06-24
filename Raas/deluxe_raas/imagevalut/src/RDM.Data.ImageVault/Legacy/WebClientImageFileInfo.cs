using System;

namespace RDM.Data.ImageVault.Legacy
{
    /// <summary>
    /// Contains information on where an image is stored in WebClient.
    /// </summary>
    public class WebClientImageFileInfo : IEquatable<WebClientImageFileInfo>
    {
        public WebClientImageFileInfo(string fileUrl, string filename)
        {
            FileUrl = fileUrl;
            Filename = filename;
        }

        public string FileUrl { get; }

        public string Filename { get; }

        public bool Equals(WebClientImageFileInfo other)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }

            if (ReferenceEquals(other, this))
            {
                return true;
            }

            return string.CompareOrdinal(FileUrl, other.FileUrl) == 0 && string.CompareOrdinal(Filename, other.Filename) == 0;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as WebClientImageFileInfo);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var result = FileUrl.GetHashCode();
                result = (result * 31) + Filename.GetHashCode();

                return result;
            }
        }
    }
}
