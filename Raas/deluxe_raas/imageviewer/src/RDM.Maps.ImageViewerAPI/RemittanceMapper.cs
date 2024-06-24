using RDM.DataTransferObjects.ImageViewerAPI;
using RDM.Model.Itms;
using RDM.Models.ImageViewerAPI;

namespace RDM.Maps.ImageViewerAPI
{
    public class RemittanceMapper : IMapper<Remittance, RemittanceDto>
    {
        private readonly IMapper<Image, ImageDto> _imageMapper;

        public RemittanceMapper()
        {
            _imageMapper = new ImageMapper();
        }

        public RemittanceDto Map(Remittance domainObj)
        {
            var dto = new RemittanceDto
            {
                IsVirtual = domainObj.IsVirtual,
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
