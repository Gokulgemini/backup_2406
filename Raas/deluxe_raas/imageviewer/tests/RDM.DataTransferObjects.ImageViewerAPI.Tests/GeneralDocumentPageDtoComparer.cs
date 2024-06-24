using System.Collections.Generic;

namespace RDM.DataTransferObjects.ImageViewerAPI.Tests
{
    public class GeneralDocumentPageDtoComparer : IEqualityComparer<GeneralDocumentPageDto>
    {
        private readonly IEqualityComparer<ImageDto> _imageDtoComparer = new ImageDtoComparer();

        public bool Equals(GeneralDocumentPageDto x, GeneralDocumentPageDto y)
        {
            return
                x.PageNumber == y.PageNumber &&
                _imageDtoComparer.Equals(x.FrontImage, y.FrontImage) &&
                _imageDtoComparer.Equals(x.BackImage, y.BackImage);
        }

        public int GetHashCode(GeneralDocumentPageDto obj)
        {
            return obj.GetHashCode();
        }
    }
}
