using System.Collections.Generic;
using RDM.DataTransferObjects.ImageViewerAPI;
using RDM.Model.Itms;
using RDM.Models.ImageViewerAPI;

namespace RDM.Maps.ImageViewerAPI
{
    public class GeneralDocumentMapper : IMapper<GeneralDocument, GeneralDocumentDto>
    {
        private readonly IMapper<Image, ImageDto> _imageMapper;

        public GeneralDocumentMapper()
        {
            _imageMapper = new ImageMapper();
        }

        public GeneralDocumentDto Map(GeneralDocument domainObj)
        {
            var dto = new GeneralDocumentDto();

            if (domainObj.DocumentName.HasValue)
            {
                dto.DocumentName = domainObj.DocumentName.Value;
            }

            var pages = new List<GeneralDocumentPageDto>();
            foreach (var page in domainObj.Pages)
            {
                var pageDto = new GeneralDocumentPageDto()
                {
                    PageNumber = page.PageNumber,
                    FrontImage = _imageMapper.Map(page.FrontImage)
                };

                if (page.BackImage.HasValue)
                {
                    pageDto.BackImage = _imageMapper.Map(page.BackImage.Value);
                }

                pages.Add(pageDto);
            }

            dto.Pages = pages.ToArray();

            return dto;
        }
    }
}
