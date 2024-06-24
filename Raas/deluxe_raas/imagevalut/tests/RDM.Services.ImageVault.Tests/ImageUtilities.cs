using System.Reflection;

namespace RDM.Services.ImageVault.Tests
{
    public static class ImageUtilities
    {
        public static byte[] GetBytesFromResourceName(string resourceName)
        {
            var fileStream = typeof(ImageUtilities).GetTypeInfo().Assembly.GetManifestResourceStream(resourceName);
            var bytes = new byte[fileStream.Length];
            fileStream.Read(bytes, 0, bytes.Length);

            return bytes;
        }
    }
}
