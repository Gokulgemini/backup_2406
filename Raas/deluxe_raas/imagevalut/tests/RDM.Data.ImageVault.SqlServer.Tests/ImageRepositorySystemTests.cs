using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using NSubstitute;
using RDM.Model.Itms;
using Xunit;
using Serilog;

namespace RDM.Data.ImageVault.SqlServer.Tests
{
    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Test files are not styled.")]
    [SuppressMessage("StyleCop.CSharp.LayoutRules", "*", Justification = "Test files are not styled.")]
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "*", Justification = "Test files are not styled.")]
    [SuppressMessage("StyleCop.CSharp.OrderingRules", "*", Justification = "Test files are not styled.")]
    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "*", Justification = "Test files are not styled.")]
    [SuppressMessage("StyleCop.CSharp.SpacingRules", "*", Justification = "Test files are not styled.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "Test files are not styled.")]
    public class ImageRepositorySystemTests : IClassFixture<ImageVaultFixture>
    {
        private readonly ImageRepository _repo;
        private readonly ImageVaultFixture _testFixture;
        private readonly IRequestDataAccessor _requestDataAccessor;
        private readonly ILogger _logger;

        public ImageRepositorySystemTests(ImageVaultFixture fixture)
        {
            _testFixture = fixture;
            _requestDataAccessor = Substitute.For<IRequestDataAccessor>();
            _requestDataAccessor.RequestId.Returns(new RequestIdentifier("1"));
            _logger = Substitute.For<ILogger>();
            _repo = new ImageRepository(_requestDataAccessor, fixture.Options, _logger);
        }

        [Fact]
        [Trait("Category", "System")]
        public void Ctor_NullRequestDataAccessor_ThrowsException()
        {
            // Assert
            Assert.ThrowsAny<ArgumentNullException>(() => new ImageRepository(null, _testFixture.Options, _logger));
        }

        [Fact]
        [Trait("Category", "System")]
        public void AddImage_ValidData_Success()
        {
            // Arrange
            byte[] content = { 1, 2, 3 };
            var mimeType = "image/jpeg";
            var width = 100;
            var height = 200;

            // Act
            var imageVaultId = _repo.AddImage(content, mimeType, width, height);

            // Assert
            Assert.NotNull(imageVaultId);
        }

        [Fact]
        [Trait("Category", "System")]
        public void GetImage_ImageExists_ReturnsExpectedImage()
        {
            // Arrange
            byte[] content = { 1, 2, 3 };
            var mimeType = "image/jpeg";
            var width = 100;
            var height = 200;
            var expectedImage = new Image(content, mimeType, width, height);

            var imageVaultId = _repo.AddImage(content, mimeType, width, height);

            // Act
            var getImageResult = _repo.GetImage(imageVaultId);

            // Assert
            AssertImageExistsAndIsExpectedImage(imageVaultId, expectedImage);
        }

        [Fact]
        [Trait("Category", "System")]
        public void GetImage_NoImage_ReturnsNull()
        {
            // Arrange
            var imageVaultId = new ImageId("d0bbbbdd32d94a85816e8592dd53780a");

            // Act
            var image = _repo.GetImage(imageVaultId);

            // Assert
            AssertImageNotFound(imageVaultId);
        }

        [Fact]
        [Trait("Category", "System")]
        public void RemoveImage_ImageExists_ImageNotFound()
        {
            // Arrange
            byte[] content = { 1, 2, 3 };
            var mimeType = "image/jpeg";
            var width = 100;
            var height = 200;
            var imageVaultId = _repo.AddImage(content, mimeType, width, height);

            // Act
            _repo.RemoveImage(imageVaultId);

            // Assert
            AssertImageNotFound(imageVaultId);
        }

        [Fact]
        [Trait("Category", "System")]
        public void UpdateImage_ImageExists_ImageContainsUpdatedContent()
        {
            // Arrange
            byte[] content = { 1, 2, 3 };
            var mimeType = "image/jpeg";
            var width = 100;
            var height = 200;
            var imageVaultId = _repo.AddImage(content, mimeType, width, height);

            byte[] newContent = { 1, 2, 3, 4 };
            var expectedImage = new Image(newContent, mimeType, width, height);

            // Act
            _repo.UpdateImage(imageVaultId, newContent, mimeType, width, height);

            // Assert
            AssertImageExistsAndIsExpectedImage(imageVaultId, expectedImage);
        }

        [Fact]
        [Trait("Category", "System")]
        public void UpdateImage_ImageDoesNotExists_ImageNotFound()
        {
            // Arrange
            var imageVaultId = new ImageId("cf276195efcf4c5db5ff994a4a3e6505");
            var mimeType = "image/jpeg";
            var width = 100;
            var height = 200;
            byte[] newContent = { 1, 2, 3, 4 };

            // Act
            _repo.UpdateImage(imageVaultId, newContent, mimeType, width, height);

            // Assert
            AssertImageNotFound(imageVaultId);
        }

        private void AssertImageExistsAndIsExpectedImage(ImageId imageId, Image expectedImage)
        {
            var getImageResult = _repo.GetImage(imageId);
            Assert.True(getImageResult.IsSuccess);
            var image = getImageResult.Value;
            Assert.True(StructuralComparisons.StructuralEqualityComparer.Equals(expectedImage.Content, image.Content));
            Assert.Equal(expectedImage.MimeType, image.MimeType);
            Assert.Equal(expectedImage.Width, image.Width);
            Assert.Equal(expectedImage.Height, image.Height);
        }

        private void AssertImageNotFound(ImageId imageId)
        {
            var getImageResult = _repo.GetImage(imageId);
            Assert.True(getImageResult.IsFailure);
        }
    }
}
