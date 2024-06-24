using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using RDM.Model.Itms;
using Xunit;

namespace RDM.Model.ImageVault.Tests
{
    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Test files are not styled.")]
    [SuppressMessage("StyleCop.CSharp.LayoutRules", "*", Justification = "Test files are not styled.")]
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "*", Justification = "Test files are not styled.")]
    [SuppressMessage("StyleCop.CSharp.OrderingRules", "*", Justification = "Test files are not styled.")]
    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "*", Justification = "Test files are not styled.")]
    [SuppressMessage("StyleCop.CSharp.SpacingRules", "*", Justification = "Test files are not styled.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "Test files are not styled.")]
    public class ImageTiffInfoUnitTests
    {
        private readonly ImageId _imageId;
        private readonly string _imageUrl;
        private readonly string _imageFilename;
        private readonly string _imageFilename2;
        private readonly int _size;
        private readonly int _width;
        private readonly int _height;

        public ImageTiffInfoUnitTests()
        {
            _imageId = new ImageId("030833e9ffb84badab5f938b0dc9925f");
            _width = 100;
            _height = 200;
            _size = 1500;
            _imageUrl = @"\\fshost01\api";
            _imageFilename = "etst.tiff";
            _imageFilename2 = "etst2.tiff";
        }

        public static IEnumerable<object[]> CtorInvalidArgumentsException()
        {
            var imageId = new ImageId("030833e9ffb84badab5f938b0dc9925f");
            var imageFilename = "abc.tiff";
            var imageUrl = @"\\itmsfs02\api\";
            var tiffSize = 1123;
            var tiffWidth = 500;
            var tiffHeight = 200;

            return new List<object[]>
            {
                new object[] { default(ImageId), imageFilename, imageUrl, tiffSize, tiffWidth, tiffHeight },
                new object[] { imageId, string.Empty, imageUrl, tiffSize, tiffWidth, tiffHeight },
                new object[] { imageId, imageFilename, string.Empty, tiffSize, tiffWidth, tiffHeight },
                new object[] { imageId, imageFilename, imageUrl, 0, tiffWidth, tiffHeight },
                new object[] { imageId, imageFilename, imageUrl, tiffSize, 0, tiffHeight },
                new object[] { imageId, imageFilename, imageUrl, tiffSize, tiffWidth, 0 }
            };
        }

        [Theory, MemberData(nameof(CtorInvalidArgumentsException))]
        [Trait("Category", "Unit")]
        public void Ctor_InvalidArguments_ThrowsException(
            ImageId imageId,
            string imageFilename,
            string imageUrl,
            int tiffSize,
            int tiffWidth,
            int tiffHeight)
        {
            // Arrange

            // Act
            var exception = Record.Exception(() => new ImageTiffInfo(imageId, imageFilename, imageUrl, tiffSize, tiffWidth, tiffHeight));

            // Assert
            Assert.NotNull(exception);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void Ctor_ValidParameters_PropertiesSet()
        {
            // Act
            var result = new ImageTiffInfo(_imageId, _imageFilename, _imageUrl, _size, _width, _height);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(_imageId, result.ImageId);
            Assert.Equal(_imageFilename, result.ImageFilename);
            Assert.Equal(_imageUrl, result.ImageUrl);
            Assert.Equal(_size, result.TiffSize);
            Assert.Equal(_width, result.TiffWidth);
            Assert.Equal(_height, result.TiffHeight);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void Equals_Null_ReturnsFalse()
        {
            // Arrange
            var item = CreateTestImage();

            // Act
            var result = item.Equals(null);

            // Assert
            Assert.False(result);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void Equals_SameInstance_ReturnsTrue()
        {
            // Arrange
            var item = CreateTestImage();

            // Act
            var result = item.Equals(item);

            // Assert
            Assert.True(result);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void Equals_DifferentInstanceSameValues_ReturnsTrue()
        {
            // Arrange
            var item1 = CreateTestImage();
            var item2 = CreateTestImage();

            // Act
            var result = item1.Equals(item2);

            // Assert
            Assert.True(result);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void Equals_DifferentValues_ReturnsFalse()
        {
            // Arrange
            var item1 = CreateTestImage();
            var item2 = CreateTestImage(true);

            // Act
            var result = item1.Equals(item2);

            // Assert
            Assert.False(result);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void Equals_DifferentTypes_ReturnsFalse()
        {
            // Arrange
            var item = CreateTestImage();

            // Act
            var result = item.Equals("123");

            // Assert
            Assert.False(result);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void GetHashCode_SameValues_SameCode()
        {
            // Arrange
            var item1 = CreateTestImage();
            var item2 = CreateTestImage();

            // Act
            // Assert
            Assert.Equal(item1.GetHashCode(), item2.GetHashCode());
        }

        private ImageTiffInfo CreateTestImage(bool variant = false)
        {
            return new ImageTiffInfo(_imageId, variant ? _imageFilename : _imageFilename2,  _imageUrl, _size, _width, _height);
        }
    }
}
