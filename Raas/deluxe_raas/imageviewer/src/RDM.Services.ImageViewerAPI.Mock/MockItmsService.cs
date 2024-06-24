using System;
using System.Threading.Tasks;
using RDM.Core;
using RDM.Model.Itms;
using RDM.Models.ImageViewerAPI;

namespace RDM.Services.ImageViewerAPI.Mock
{
    public class MockItmsService : IItmsItemService
    {
        public Task<Result<Error, Cheque>> GetChequeByIrnAsync(IrnId irn, int seqNum, UserId userId, string tenantId, DateTime startInsertTime, DateTime endInsertTime)
        {
            throw new System.NotImplementedException();
        }

        public Task<Result<Error, GeneralDocument>> GetGeneralDocumentByIrnAsync(IrnId irn, int seqNum, UserId userId, string tenantId, DateTime startInsertTime, DateTime endInsertTime)
        {
            throw new System.NotImplementedException();
        }

        public Task<Result<Error, Remittance>> GetRemittanceByIrnAsync(IrnId irn, int seqNum, UserId userId, string tenantId, DateTime startInsertTime, DateTime endInsertTime)
        {
            throw new System.NotImplementedException();
        }
    }
}
