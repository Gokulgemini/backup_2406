using System.Collections.Generic;

namespace RDM.DataTransferObjects.ImageViewerAPI.Tests
{
    public class RemittanceDtoComparer : IEqualityComparer<RemittanceDto>
    {
        private readonly IEqualityComparer<ImageDto> _imageDtoComparer = new ImageDtoComparer();

        public bool Equals(RemittanceDto x, RemittanceDto y)
        {
            return
                x.IsVirtual == y.IsVirtual &&
                _imageDtoComparer.Equals(x.FrontImage, y.FrontImage) &&
                _imageDtoComparer.Equals(x.BackImage, y.BackImage);
        }

        public int GetHashCode(RemittanceDto obj)
        {
            return obj.GetHashCode();
        }
    }
}
