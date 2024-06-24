using System.Collections.Generic;

namespace RDM.DataTransferObjects.ImageViewerAPI.Tests
{
    public class ChequeDtoComparer : IEqualityComparer<ChequeDto>
    {
        private readonly IEqualityComparer<ImageDto> _imageDtoComparer = new ImageDtoComparer();

        public bool Equals(ChequeDto x, ChequeDto y)
        {
            return _imageDtoComparer.Equals(x.FrontImage, y.FrontImage) &&
                   _imageDtoComparer.Equals(x.BackImage, y.BackImage);
        }

        public int GetHashCode(ChequeDto obj)
        {
            return obj.GetHashCode();
        }
    }
}
