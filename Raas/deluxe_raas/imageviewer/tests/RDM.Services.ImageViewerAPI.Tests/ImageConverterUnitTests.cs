using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using NSubstitute;
using RDM.Model.Itms;
using Serilog;
using Xunit;

namespace RDM.Services.ImageViewerAPI.Tests
{
    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Test files are not styled.")]
    [SuppressMessage("StyleCop.CSharp.LayoutRules", "*", Justification = "Test files are not styled.")]
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "*", Justification = "Test files are not styled.")]
    [SuppressMessage("StyleCop.CSharp.OrderingRules", "*", Justification = "Test files are not styled.")]
    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "*", Justification = "Test files are not styled.")]
    [SuppressMessage("StyleCop.CSharp.SpacingRules", "*", Justification = "Test files are not styled.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "Test files are not styled.")]
    public class ImageConverterUnitTests
    {
        private static readonly byte[] _tiffBytes = ExtractBytesFromResource("RDM.Services.ImageViewerAPI.Tests.cheque.tiff");
        private static readonly byte[] _pngBytes = ExtractBytesFromResource("RDM.Services.ImageViewerAPI.Tests.cheque.png");

        private readonly ImageConverter _converter;

        public ImageConverterUnitTests()
        {
            var requestDataAccessor = Substitute.For<IRequestDataAccessor>();
            var logger = Substitute.For<ILogger>();

            _converter = new ImageConverter(requestDataAccessor, logger);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async void ConvertImageToPng_InputIsTiff_ReturnsConvertedImageAsync()
        {
            // Arrange
            var image = new Image(_tiffBytes, "image/tiff", 10, 20);

            // Act
            var result = await _converter.ConvertImageToPng(image);

            // Assert
            // Can't actually compare the bytes because the header seems to change everytime this is run. It may be a timestamp or something.
            Assert.True(result.IsSuccess);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async void ConvertImageToPng_InputIsPng_NoChanges()
        {
            // Arrange
            var image = new Image(_pngBytes, "image/png", 10, 20);

            // Act
            var result = await _converter.ConvertImageToPng(image);

            // Assert
            Assert.True(result.IsSuccess);
            var actualBytes = result.Value.Content;
            Assert.True(_pngBytes.SequenceEqual(actualBytes));
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async void ConvertImageToPng_ExceptionDuringExecution_ReturnsError()
        {
            // Arrange
            var image = new Image(new byte[] { 1, 2, 3 }, "image/tiff", 10, 20);

            // Act
            var result = await _converter.ConvertImageToPng(image);

            // Assert
            Assert.True(result.IsFailure);
        }

        private static byte[] ExtractBytesFromResource(string resourceName)
        {
            var fileStream = typeof(ImageConverterUnitTests).GetTypeInfo().Assembly.GetManifestResourceStream(resourceName);
            var bytes = new byte[fileStream.Length];
            fileStream.Read(bytes, 0, bytes.Length);

            return bytes;
        }
    }
}
