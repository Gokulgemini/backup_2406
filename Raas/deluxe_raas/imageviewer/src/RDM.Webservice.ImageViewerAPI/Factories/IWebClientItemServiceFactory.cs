using RDM.Services.ImageViewerAPI;

namespace RDM.Webservice.ImageViewerAPI.Factories
{
    public interface IWebClientItemServiceFactory
    {
        IWebClientItemService Create(IRequestData requestData);
    }
}
