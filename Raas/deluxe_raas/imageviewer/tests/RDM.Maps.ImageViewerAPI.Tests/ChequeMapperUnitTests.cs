using System.Diagnostics.CodeAnalysis;
using RDM.DataTransferObjects.ImageViewerAPI;
using RDM.DataTransferObjects.ImageViewerAPI.Tests;
using RDM.Model.Itms;
using RDM.Models.ImageViewerAPI;
using Xunit;

namespace RDM.Maps.ImageViewerAPI.Tests
{
    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Test files are not styled.")]
    [SuppressMessage("StyleCop.CSharp.LayoutRules", "*", Justification = "Test files are not styled.")]
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "*", Justification = "Test files are not styled.")]
    [SuppressMessage("StyleCop.CSharp.OrderingRules", "*", Justification = "Test files are not styled.")]
    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "*", Justification = "Test files are not styled.")]
    [SuppressMessage("StyleCop.CSharp.SpacingRules", "*", Justification = "Test files are not styled.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "Test files are not styled.")]
    public class ChequeMapperUnitTests
    {
        private readonly ChequeMapper _mapper = new ChequeMapper();

        [Fact]
        [Trait("Category", "Unit")]
        public void Map_OneSidedCheque_ReturnsExpectedDto()
        {
            // Arrange
            var frontImage = new Image(new byte[] { 1, 2, 3, 4 }, "image/png", 10, 20);
            var cheque = Cheque.OneSided(frontImage);

            var expectedDto = new ChequeDto()
            {
                FrontImage = new ImageDto()
                {
                    Content = frontImage.Content,
                    Width = frontImage.Width,
                    Height = frontImage.Height
                }
            };

            // Act
            var dto = _mapper.Map(cheque);

            // Assert
            Assert.Equal(expectedDto, dto, new ChequeDtoComparer());
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void Map_TwoSidedCheque_ReturnsExpectedDto()
        {
            // Arrange
            var frontImage = new Image(new byte[] { 1, 2, 3, 4 }, "image/png", 10, 20);
            var backImage = new Image(new byte[] { 5, 6, 7, 8 }, "image/png", 30, 40);
            var cheque = Cheque.TwoSided(frontImage, backImage);

            var expectedDto = new ChequeDto()
            {
                FrontImage = new ImageDto()
                {
                    Content = frontImage.Content,
                    Width = frontImage.Width,
                    Height = frontImage.Height
                },
                BackImage = new ImageDto()
                {
                    Content = backImage.Content,
                    Width = backImage.Width,
                    Height = backImage.Height
                }
            };

            // Act
            var dto = _mapper.Map(cheque);

            // Assert
            Assert.Equal(expectedDto, dto, new ChequeDtoComparer());
        }
    }
}
