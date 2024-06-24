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
    public class ChequeUnitTests
    {
        public static IEnumerable<object[]> EqualInstances()
        {
            var frontImage = new Image(new byte[] { 1, 2, 3, 4 }, "image/png", 10, 20);
            var backImage = new Image(new byte[] { 5, 6, 7, 8 }, "image/png", 10, 20);

            return new List<object[]>
            {
                new object[] { Cheque.OneSided(frontImage), Cheque.OneSided(frontImage) },
                new object[] { Cheque.TwoSided(frontImage, backImage), Cheque.TwoSided(frontImage, backImage) }
            };
        }

        [Theory]
        [MemberData(nameof(EqualInstances))]
        [Trait("Category", "Unit")]
        public void Equals_EqualInstances_ReturnsTrue(Cheque left, Cheque right)
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
                new object[] { Cheque.OneSided(frontImage), Cheque.TwoSided(frontImage, backImage) },
                new object[] { Cheque.TwoSided(frontImage, backImage), Cheque.OneSided(frontImage) }
            };
        }

        [Theory]
        [MemberData(nameof(UnequalInstances))]
        [Trait("Category", "Unit")]
        public void Equals_UnequalInstances_ReturnsFalse(Cheque left, Cheque right)
        {
            // Arrange

            // Act, Assert
            Assert.False(left.Equals(right));
        }
    }
}
