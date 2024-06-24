using System.Collections.Generic;
using System.Linq;

namespace RDM.DataTransferObjects.ImageViewerAPI.Tests
{
    public class ImageDtoComparer : IEqualityComparer<ImageDto>
    {
        public bool Equals(ImageDto x, ImageDto y)
        {
            if (x.Content == null)
            {
                return y.Content == null;
            }

            return x.Content.SequenceEqual(y.Content) && x.Width == y.Width && x.Height == y.Height;
        }

        public int GetHashCode(ImageDto obj)
        {
            return obj.GetHashCode();
        }
    }
}
