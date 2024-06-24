using System.Collections.Generic;
using System.Linq;

namespace RDM.DataTransferObjects.ImageViewerAPI.Tests
{
    public class GeneralDocumentDtoComparer : IEqualityComparer<GeneralDocumentDto>
    {
        private readonly IEqualityComparer<GeneralDocumentPageDto> _pageDtoComparer = new GeneralDocumentPageDtoComparer();

        public bool Equals(GeneralDocumentDto x, GeneralDocumentDto y)
        {
            return
                x.DocumentName == y.DocumentName &&
                x.Pages.SequenceEqual(y.Pages, _pageDtoComparer);
        }

        public int GetHashCode(GeneralDocumentDto obj)
        {
            return obj.GetHashCode();
        }
    }
}
