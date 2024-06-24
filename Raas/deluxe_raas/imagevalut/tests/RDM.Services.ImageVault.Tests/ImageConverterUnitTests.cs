using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using NSubstitute;
using RDM.Model.Itms;
using Xunit;

namespace RDM.Services.ImageVault.Tests
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
        public static IEnumerable<object[]> ImageResourceNames()
        {
            return new[]
            {
                new object[] { "RDM.Services.ImageVault.Tests.large_front_image.jpg" },
                new object[] { "RDM.Services.ImageVault.Tests.LargePortrait.jpg" },
                new object[] { "RDM.Services.ImageVault.Tests.sample.jpg" },
                new object[] { "RDM.Services.ImageVault.Tests.very_large_png.png" },
                new object[] { "RDM.Services.ImageVault.Tests.widesample.jpg" }
            };
        }

        [Theory, MemberData(nameof(ImageResourceNames))]
        [Trait("Category", "Unit")]
        public void ConvertToBinaryTiff_WithTestImage_ProducesTiff(string resourceName)
        {
            var image = GetImageBytes(resourceName);

            var converter = new ImageConverter(
                Substitute.For<IRequestDataAccessor>());

            var output = converter.ConvertToBinaryTiff(image);

            Assert.NotNull(output);
        }

        private byte[] GetImageBytes(string filename)
        {
            using (var fileStream = typeof(ImageConverterUnitTests).GetTypeInfo().Assembly.GetManifestResourceStream(filename))
            {
                var bytes = new byte[fileStream.Length];
                fileStream.Read(bytes, 0, bytes.Length);

                return bytes;
            }
        }
    }
}
