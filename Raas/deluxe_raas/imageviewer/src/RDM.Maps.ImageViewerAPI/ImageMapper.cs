using RDM.DataTransferObjects.ImageViewerAPI;
using RDM.Model.Itms;

namespace RDM.Maps.ImageViewerAPI
{
    public class ImageMapper : IMapper<Image, ImageDto>
    {
        public ImageDto Map(Image domainObj)
        {
            return new ImageDto()
            {
                Content = domainObj.Content,
                Width = domainObj.Width,
                Height = domainObj.Height
            };
        }
    }
}
