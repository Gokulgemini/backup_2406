using RDM.DataTransferObjects.ImageViewerAPI;
using RDM.Model.Itms;
using RDM.Models.ImageViewerAPI;

namespace RDM.Maps.ImageViewerAPI
{
    public class ChequeMapper : IMapper<Cheque, ChequeDto>
    {
        private readonly IMapper<Image, ImageDto> _imageMapper;

        public ChequeMapper()
        {
            _imageMapper = new ImageMapper();
        }

        public ChequeDto Map(Cheque domainObj)
        {
            var dto = new ChequeDto()
            {
                FrontImage = _imageMapper.Map(domainObj.FrontImage)
            };

            if (domainObj.BackImage.HasValue)
            {
                dto.BackImage = _imageMapper.Map(domainObj.BackImage.Value);
            }

            return dto;
        }
    }
}
