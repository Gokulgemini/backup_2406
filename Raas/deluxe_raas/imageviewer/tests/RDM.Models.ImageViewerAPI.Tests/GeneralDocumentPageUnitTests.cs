using System.Diagnostics.CodeAnalysis;
using RDM.Core;
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
    public class GeneralDocumentPageUnitTests
    {
        [Fact]
        [Trait("Category", "Unit")]
        public void Equals_EqualInstances_ReturnsTrue()
        {
            // Arrange
            var left = new GeneralDocumentPage(
                0,
                new Image(new byte[] { 1, 2, 3, 4 }, "image/png", 10, 20),
                new Image(new byte[] { 5, 6, 7, 8 }, "image/png", 10, 20)
            );

            var right = new GeneralDocumentPage(
                0,
                new Image(new byte[] { 1, 2, 3, 4 }, "image/png", 10, 20),
                new Image(new byte[] { 5, 6, 7, 8 }, "image/png", 10, 20)
            );

            // Act, Assert
            Assert.True(left.Equals(right));
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void Equals_UnequalInstances_ReturnsFalse()
        {
            // Arrange
            var left = new GeneralDocumentPage(
                0,
                new Image(new byte[] { 1, 2, 3, 4 }, "image/png", 10, 20),
                new Image(new byte[] { 5, 6, 7, 8 }, "image/png", 10, 20)
            );

            var right = new GeneralDocumentPage(
                0,
                new Image(new byte[] { 1, 2, 3, 4 }, "image/png", 10, 20),
                Maybe<Image>.Empty()
            );

            // Act, Assert
            Assert.False(left.Equals(right));
        }
    }
}
