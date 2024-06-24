using System;
using System.Threading.Tasks;
using RDM.Core;
using RDM.Model.Itms;
using RDM.Models.ImageViewerAPI;

namespace RDM.Services.ImageViewerAPI.Mock
{
    public class MockWebClientService : IWebClientItemService
    {
        public Task<Result<Error, Cheque>> GetChequeByIrnAsync(IrnId irn, UserId userId, string tenantId)
        {
            throw new NotImplementedException();
        }

        public Task<Result<Error, GeneralDocument>> GetGeneralDocumentByIrnAsync(IrnId irn, UserId userId, string tenantId)
        {
            throw new NotImplementedException();
        }

        public Task<Result<Error, Remittance>> GetRemittanceByIrnAsync(IrnId irn, UserId userId, string tenantId)
        {
            throw new NotImplementedException();
        }

        public Task<Result<Error, bool>> UpdateDocumentNameByIrnAsync(IrnId irn, string documentName, UserId userId, Module module)
        {
            throw new NotImplementedException();
        }
    }
}
