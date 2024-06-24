using System.Threading.Tasks;
using RDM.Core;
using RDM.Model.Itms;
using RDM.Models.ImageViewerAPI;

namespace RDM.Services.ImageViewerAPI
{
    /// <summary>
    /// The service called on by the ImagesController to perform operations in WebClient host.
    /// </summary>
    public interface IWebClientItemService
    {
        /// <summary>
        /// Updates document name by irn.
        /// </summary>
        /// <param name="irn">The irn.</param>
        /// <param name="documentName">The document name.</param>
        /// <param name="userId">The identifier of the user attempting to get the image.</param>
        /// <param name="module">The module name to save audit trails.</param>
        /// <returns>True if updated otherwise Error</returns>
        Task<Result<Error, bool>> UpdateDocumentNameByIrnAsync(IrnId irn, string documentName, UserId userId, Module module);

        /// <summary>
        /// Gets the cheque images by irn.
        /// </summary>
        /// <param name="irn">The irn.</param>
        /// <param name="userId">The identifier of the user attempting to get the image.</param>
        /// <param name="tenantId">The identifier of the tenant to access data from.</param>
        /// <returns>Images if found otherwise Error</returns>
        Task<Result<Error, Cheque>> GetChequeByIrnAsync(IrnId irn, UserId userId, string tenantId);

        /// <summary>
        /// Gets the remittance images by irn.
        /// </summary>
        /// <param name="irn">The irn.</param>
        /// <param name="userId">The identifier of the user attempting to get the image.</param>
        /// <param name="tenantId">The identifier of the tenant to access data from.</param>
        /// <returns>Images if found otherwise Error</returns>
        Task<Result<Error, Remittance>> GetRemittanceByIrnAsync(IrnId irn, UserId userId, string tenantId);

        /// <summary>
        /// Gets the general document images by irn.
        /// </summary>
        /// <param name="irn">The irn.</param>
        /// <param name="userId">The identifier of the user attempting to get the image.</param>
        /// <param name="tenantId">The identifier of the tenant to access data from.</param>
        /// <returns>Images if found otherwise Error</returns>
        Task<Result<Error, GeneralDocument>> GetGeneralDocumentByIrnAsync(IrnId irn, UserId userId, string tenantId);
    }
}