using RDM.Core;
using RDM.Model.Itms;

namespace RDM.Data.ImageVault.Legacy
{
    public interface IWebClientImageRepository
    {
        /// <summary>
        /// Gets the image file information for images stored in WebClient
        /// </summary>
        /// <param name="irn">The irn.</param>
        /// <param name="surface">The surface.</param>
        /// <param name="page">The page.</param>
        /// <returns>The resulting file info for images stored in WebClient or an Error</returns>
        /// <remarks>For ITMSAPI since the do scope checking and don't need SeqNum</remarks>
        Result<Error, WebClientImageFileInfo> GetImageFileInfo(string irn, string surface, int page);

        /// <summary>
        /// Gets the image file information for images stored in WebClient.
        /// </summary>
        /// <param name="userId">The identifier of the user trying to access image info</param>
        /// <param name="irn">The irn.</param>
        /// <param name="seqNum">The item's sequential number within a transaction.</param>
        /// <param name="surface">The surface.</param>
        /// <param name="page">The page.</param>
        /// <returns>The resulting file info for images stored in WebClient or an Error</returns>
        /// <remarks>Multiple items may use the same IRN, to differentiate them, a seqNum is then used.</remarks>
        Result<Error, WebClientImageFileInfo> GetImageFileInfo(UserId userId, string irn, int seqNum, string surface, int page);
    }
}
