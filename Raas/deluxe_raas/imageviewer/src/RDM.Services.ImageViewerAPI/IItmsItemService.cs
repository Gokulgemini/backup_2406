using System;
using System.Threading.Tasks;
using RDM.Core;
using RDM.Model.Itms;
using RDM.Models.ImageViewerAPI;

namespace RDM.Services.ImageViewerAPI
{
    /// <summary>
    /// The service called on by the ImagesController to perform operations in ITMS host.
    /// </summary>
    public interface IItmsItemService
    {
        /// <summary>
        /// Gets the cheque images by irn.
        /// </summary>
        /// <param name="irn">The irn.</param>
        /// <param name="seqNum">The item's sequential number within a transaction.</param>
        /// <param name="userId">The identifier of the user attempting to get the image.</param>
        /// <param name="tenantId">The identifier of the tentant to access data from.</param>
        /// <param name="startInsertTime">Start insert date to get the image.</param>
        /// <param name="endInsertTime">End insert date to get the image..</param>
        /// <returns>Images if found otherwise Error</returns>
        Task<Result<Error, Cheque>> GetChequeByIrnAsync(IrnId irn, int seqNum, UserId userId, string tenantId, DateTime startInsertTime, DateTime endInsertTime);

        /// <summary>
        /// Gets the remittance images by irn.
        /// </summary>
        /// <param name="irn">The irn.</param>
        /// <param name="seqNum">The item's sequential number within a transaction.</param>
        /// <param name="userId">The identifier of the user attempting to get the image.</param>
        /// <param name="tenantId">The identifier of the tentant to access data from.</param>
        /// <param name="startInsertTime">Start insert date to get the image.</param>
        /// <param name="endInsertTime">End insert date to get the image..</param>
        /// <returns>Images if found otherwise Error</returns>
        Task<Result<Error, Remittance>> GetRemittanceByIrnAsync(IrnId irn, int seqNum, UserId userId, string tenantId, DateTime startInsertTime, DateTime endInsertTime);

        /// <summary>
        /// Gets the general document images by irn.
        /// </summary>
        /// <param name="irn">The irn.</param>
        /// <param name="seqNum">The item's sequential number within a transaction.</param>
        /// <param name="userId">The identifier of the user attempting to get the image.</param>
        /// <param name="tenantId">The identifier of the tentant to access data from.</param>
        /// <param name="startInsertTime">Start insert date to get the image.</param>
        /// <param name="endInsertTime">End insert date to get the image..</param>
        /// <returns>Images if found otherwise Error</returns>
        Task<Result<Error, GeneralDocument>> GetGeneralDocumentByIrnAsync(IrnId irn, int seqNum, UserId userId, string tenantId, DateTime startInsertTime, DateTime endInsertTime);
    }
}
