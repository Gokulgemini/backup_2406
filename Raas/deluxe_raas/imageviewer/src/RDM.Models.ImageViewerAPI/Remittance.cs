using System;
using RDM.Core;
using RDM.Model.Itms;

namespace RDM.Models.ImageViewerAPI
{
    public class Remittance : IEquatable<Remittance>
    {
        public bool IsVirtual { get; private set; }

        public Image FrontImage { get; private set; }

        public Maybe<Image> BackImage { get; private set; } = Maybe<Image>.Empty();

        public static Remittance Virtual(Image frontImage)
        {
            return new Remittance { IsVirtual = true, FrontImage = frontImage };
        }

        public static Remittance OneSided(Image frontImage)
        {
            return new Remittance { FrontImage = frontImage };
        }

        public static Remittance TwoSided(Image frontImage, Image backImage)
        {
            return new Remittance { FrontImage = frontImage, BackImage = backImage };
        }

        public static bool operator ==(Remittance lhs, Remittance rhs)
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

        public static bool operator !=(Remittance lhs, Remittance rhs)
        {
            return !(lhs == rhs);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Remittance);
        }

        public bool Equals(Remittance other)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }

            if (ReferenceEquals(other, this))
            {
                return true;
            }

            return IsVirtual == other.IsVirtual
                && FrontImage.Equals(other.FrontImage)
                && (BackImage.HasValue == other.BackImage.HasValue)
                && ((!BackImage.HasValue && !other.BackImage.HasValue) || BackImage.Value.Equals(other.BackImage.Value));
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var result = IsVirtual.GetHashCode();
                result = (result * 31) + FrontImage.GetHashCode();

                if (BackImage.HasValue)
                {
                    result = (result * 31) + BackImage.Value.GetHashCode();
                }

                return result;
            }
        }
    }
}
