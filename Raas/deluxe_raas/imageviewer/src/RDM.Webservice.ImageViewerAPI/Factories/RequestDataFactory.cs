using Microsoft.AspNetCore.Http;

namespace RDM.Webservice.ImageViewerAPI.Factories
{
    public class RequestDataFactory : IRequestDataFactory
    {
        private readonly IHttpContextAccessor _accessor;

        public RequestDataFactory(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }

        public IRequestData Create()
        {
            return new RequestData(_accessor.HttpContext);
        }
    }
}
