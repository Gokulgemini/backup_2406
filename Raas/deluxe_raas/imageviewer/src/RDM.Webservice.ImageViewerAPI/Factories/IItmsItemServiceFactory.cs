using RDM.Services.ImageViewerAPI;

namespace RDM.Webservice.ImageViewerAPI.Factories
{
    public interface IItmsItemServiceFactory
    {
        IItmsItemService Create(IRequestData requestData);
    }
}
