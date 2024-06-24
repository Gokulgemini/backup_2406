using System;
using RDM.Core;
using RDM.Model.Itms;

namespace RDM.Models.ImageViewerAPI
{
    public class GeneralDocumentPage : IEquatable<GeneralDocumentPage>
    {
        public GeneralDocumentPage(int pageNumber, Image frontImage, Maybe<Image> backImage)
        {
            Contract.Requires<ArgumentNullException>(frontImage != null, nameof(frontImage));
            Contract.Requires<ArgumentNullException>(backImage != null, nameof(backImage));

            PageNumber = pageNumber;
            FrontImage = frontImage;
            BackImage = backImage;
        }

        public int PageNumber { get; }

        public Image FrontImage { get; }

        public Maybe<Image> BackImage { get; }

        public static GeneralDocumentPage OneSided(int pageNumber, Image frontImage)
        {
            return new GeneralDocumentPage(pageNumber, frontImage, Maybe<Image>.Empty());
        }

        public static GeneralDocumentPage TwoSided(int pageNumber, Image frontImage, Image backImage)
        {
            return new GeneralDocumentPage(pageNumber, frontImage, backImage);
        }

        public static bool operator ==(GeneralDocumentPage lhs, GeneralDocumentPage rhs)
        {
            if (ReferenceEquals(lhs, null) && ReferenceEquals(rhs, null))
            {
                return true;
            }

            if (ReferenceEquals(lhs, null))
            {
                return false;
            }

            return lhs.Equals(rhs);
        }

        public static bool operator !=(GeneralDocumentPage lhs, GeneralDocumentPage rhs)
        {
            return !(lhs == rhs);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as GeneralDocumentPage);
        }

        public bool Equals(GeneralDocumentPage other)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }

            if (ReferenceEquals(other, this))
            {
                return true;
            }

            return
                PageNumber == other.PageNumber &&
                FrontImage.Equals(other.FrontImage) &&
                (BackImage.HasValue == other.BackImage.HasValue) &&
                (!BackImage.HasValue || BackImage.Value.Equals(other.BackImage.Value));
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var result = PageNumber.GetHashCode();
                result = (result * 31) + FrontImage.GetHashCode();

                if (BackImage.HasValue)
                {
                    result = (result * 31) + BackImage.GetHashCode();
                }

                return result;
            }
        }
    }
}
