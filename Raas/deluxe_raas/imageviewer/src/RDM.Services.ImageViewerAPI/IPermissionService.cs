using RDM.Model.Itms;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RDM.Services.ImageViewerAPI
{
    public interface IPermissionService
    {
        Task<bool> CanUpdateGeneralDocumentName(UserId userId, DateTime lastAuthenticationTimeUtc);
    }
}
