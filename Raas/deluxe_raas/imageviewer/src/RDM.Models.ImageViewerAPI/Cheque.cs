using System;
using RDM.Core;
using RDM.Model.Itms;

namespace RDM.Models.ImageViewerAPI
{
    public class Cheque : IEquatable<Cheque>
    {
        public Image FrontImage { get; private set; }

        public Maybe<Image> BackImage { get; private set; } = Maybe<Image>.Empty();

        public static Cheque OneSided(Image frontImage)
        {
            return new Cheque() { FrontImage = frontImage };
        }

        public static Cheque TwoSided(Image frontImage, Image backImage)
        {
            return new Cheque() { FrontImage = frontImage, BackImage = backImage };
        }

        public static bool operator ==(Cheque lhs, Cheque rhs)
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

        public static bool operator !=(Cheque lhs, Cheque rhs)
        {
            return !(lhs == rhs);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Cheque);
        }

        public bool Equals(Cheque other)
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
                 (FrontImage.Equals(other.FrontImage) &&
                 (BackImage.HasValue == other.BackImage.HasValue) &&
                 ((!BackImage.HasValue && !other.BackImage.HasValue) || BackImage.Value.Equals(other.BackImage.Value)));
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var result = FrontImage.GetHashCode();

                if (BackImage.HasValue)
                {
                    result = (result * 31) + BackImage.Value.GetHashCode();
                }

                return result;
            }
        }
    }
}
