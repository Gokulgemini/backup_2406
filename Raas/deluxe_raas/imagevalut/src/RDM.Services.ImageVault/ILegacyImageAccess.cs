using RDM.Core;
using RDM.Model.ImageVault;
using RDM.Model.Itms;

namespace RDM.Services.ImageVault
{
    public interface ILegacyImageAccess
    {
        /// <summary>
        /// Get an image from the legacy ITMS environment.
        /// </summary>
        Result<Error, Image> GetImageFromItms(UserId userId, IrnId irn, int seqNum, ImageSurface surface, int page);

        /// <summary>
        /// Get an image from the legacy WebClient environment.
        /// </summary>
        Result<Error, Image> GetImageFromWebClient(string tenantId, IrnId irn, ImageSurface surface, int page);

        /// <summary>
        /// Get an image from the legacy WebClient environment.
        /// </summary>
        Result<Error, Image> GetImageFromWebClient(
            string tenantId,
            UserId userId,
            IrnId irn,
            int seqNum,
            ImageSurface surface,
            int page);

        /// <summary>
        /// Write an image from the legacy WebClient environment.
        /// </summary>
        Result<Error, ImageTiffInfo> WriteImageToWebClient(
            Image image,
            ImageId imageId,
            string tenantId,
            string filepath,
            string filename);
    }
}
