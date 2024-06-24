using System.Collections.Generic;
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
    public class GeneralDocumentMapperUnitTests
    {
        private readonly GeneralDocumentMapper _mapper = new GeneralDocumentMapper();

        [Fact]
        [Trait("Category", "Unit")]
        public void Map_GeneralDocument_ReturnsExpectedDto()
        {
            // Arrange
            var documentName = "testDoc";
            var frontImage = new Image(new byte[] { 1, 2, 3, 4 }, "image/png", 10, 20);
            var backImage = new Image(new byte[] { 5, 6, 7, 8 }, "image/png", 30, 40);
            var pages = new List<GeneralDocumentPage>()
            {
                new GeneralDocumentPage(0, frontImage, backImage)
            };

            var genDoc = new GeneralDocument(documentName, pages);

            var expectedDto = new GeneralDocumentDto()
            {
                DocumentName = documentName,
                Pages = new GeneralDocumentPageDto[]
                {
                    new GeneralDocumentPageDto()
                    {
                        PageNumber = 0,
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
                    }
                }
            };

            // Act
            var dto = _mapper.Map(genDoc);

            // Assert
            Assert.Equal(expectedDto, dto, new GeneralDocumentDtoComparer());
        }
    }
}
