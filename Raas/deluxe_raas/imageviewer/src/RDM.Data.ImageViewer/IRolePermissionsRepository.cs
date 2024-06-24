using RDM.Model.Itms;
using System.Threading.Tasks;

namespace RDM.Data.ImageViewer
{
    public interface IRolePermissionsRepository
    {
        Task<bool> CanUpdateGeneralDocumentName(UserId userId);
    }
}