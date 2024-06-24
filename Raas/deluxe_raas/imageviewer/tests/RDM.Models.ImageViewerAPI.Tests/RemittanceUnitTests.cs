using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using RDM.Model.Itms;
using Xunit;

namespace RDM.Models.ImageViewerAPI.Tests
{
    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Test files are not styled.")]
    [SuppressMessage("StyleCop.CSharp.LayoutRules", "*", Justification = "Test files are not styled.")]
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "*", Justification = "Test files are not styled.")]
    [SuppressMessage("StyleCop.CSharp.OrderingRules", "*", Justification = "Test files are not styled.")]
    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "*", Justification = "Test files are not styled.")]
    [SuppressMessage("StyleCop.CSharp.SpacingRules", "*", Justification = "Test files are not styled.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "Test files are not styled.")]
    public class RemittanceUnitTests
    {
        public static IEnumerable<object[]> EqualInstances()
        {
            var frontImage = new Image(new byte[] { 1, 2, 3, 4 }, "image/png", 10, 20);
            var backImage = new Image(new byte[] { 5, 6, 7, 8 }, "image/png", 10, 20);

            return new List<object[]>
            {
                new object[] { Remittance.Virtual(frontImage), Remittance.Virtual(frontImage) },
                new object[] { Remittance.OneSided(frontImage), Remittance.OneSided(frontImage) },
                new object[] { Remittance.TwoSided(frontImage, backImage), Remittance.TwoSided(frontImage, backImage) }
            };
        }

        [Theory]
        [MemberData(nameof(EqualInstances))]
        [Trait("Category", "Unit")]
        public void Equals_EqualInstances_ReturnsTrue(Remittance left, Remittance right)
        {
            // Arrange

            // Act, Assert
            Assert.True(left.Equals(right));
        }

        public static IEnumerable<object[]> UnequalInstances()
        {
            var frontImage = new Image(new byte[] { 1, 2, 3, 4 }, "image/png", 10, 20);
            var backImage = new Image(new byte[] { 5, 6, 7, 8 }, "image/png", 10, 20);

            return new List<object[]>
            {
                new object[] { Remittance.Virtual(frontImage), Remittance.OneSided(frontImage) },
                new object[] { Remittance.Virtual(frontImage), Remittance.TwoSided(frontImage, backImage) },
                new object[] { Remittance.OneSided(frontImage), Remittance.Virtual(frontImage) },
                new object[] { Remittance.OneSided(frontImage), Remittance.TwoSided(frontImage, backImage) },
                new object[] { Remittance.TwoSided(frontImage, backImage), Remittance.Virtual(frontImage) },
                new object[] { Remittance.TwoSided(frontImage, backImage), Remittance.OneSided(frontImage) }
            };
        }

        [Theory]
        [MemberData(nameof(UnequalInstances))]
        [Trait("Category", "Unit")]
        public void Equals_UnequalInstances_ReturnsFalse(Remittance left, Remittance right)
        {
            // Arrange

            // Act, Assert
            Assert.False(left.Equals(right));
        }
    }
}
