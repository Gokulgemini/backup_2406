using System;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;
using RDM.Model.ImageVault;
using RDM.Model.Itms;
using Xunit;

namespace RDM.Messaging.ImageVault.Tests
{
    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Test files are not styled.")]
    [SuppressMessage("StyleCop.CSharp.LayoutRules", "*", Justification = "Test files are not styled.")]
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "*", Justification = "Test files are not styled.")]
    [SuppressMessage("StyleCop.CSharp.OrderingRules", "*", Justification = "Test files are not styled.")]
    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "*", Justification = "Test files are not styled.")]
    [SuppressMessage("StyleCop.CSharp.SpacingRules", "*", Justification = "Test files are not styled.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "Test files are not styled.")]
    public class ReplyWriteImageToWebClientMessageUnitTests
    {
        [Fact]
        [Trait("Category", "Unit")]
        public void ReplyWriteTiffToShareMessage_Serialize_ToString()
        {
            // Arrange
            var item = CreateTestMessage();

            // Act
            var result = JsonConvert.SerializeObject(item);

            // Assert
            Assert.Equal("{\"Status\":\"Success\",\"ImageTiffInfo\":{\"ImageId\":{\"Value\":\"97c921f0ecfc411b9997bd6720ceb26b\"},\"ImageFilename\":\"testFileName\",\"ImageUrl\":\"testImageUrl\",\"TiffSize\":168961,\"TiffWidth\":1178,\"TiffHeight\":544}}", result);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void ReplyWriteTiffToShareMessage_Deserialize_FromString()
        {
            // Arrange
            var item = CreateTestMessage();
            var json = "{\"Status\":\"Success\",\"ImageTiffInfo\":{\"ImageId\":{\"Value\":\"97c921f0ecfc411b9997bd6720ceb26b\"},\"ImageFilename\":\"testFileName\",\"ImageUrl\":\"testImageUrl\",\"TiffSize\":168961,\"TiffWidth\":1178,\"TiffHeight\":544}}";

            // Act
            var result = JsonConvert.DeserializeObject<ReplyWriteImageToWebClientMessage>(json);

            // Assert
            Assert.Equal(item, result);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void Ctor_NullImageInfo_ThrowsException()
        {
            // Act
            var exception = Record.Exception(() => new ReplyWriteImageToWebClientMessage(WriteImageToWebClientStatus.Success, null));

            // Assert
            Assert.NotNull(exception);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void Ctor_Parameterless_MatchingProperties()
        {
            // Act
            var item = Activator.CreateInstance<ReplyWriteImageToWebClientMessage>();

            // Assert
            Assert.Equal(ReplyWriteImageToWebClientMessage.RabbitExchange, item.ExchangeName);
            Assert.Equal(ReplyWriteImageToWebClientMessage.RabbitQueue, item.DefaultQueueName);
            Assert.Equal(ReplyWriteImageToWebClientMessage.RabbitKey, item.RoutingKey);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void Ctor_ValidParameters_ObjectCreated()
        {
            // Arrange
            var imageInfo = CreateImageInfo();

            // Act
            var item = new ReplyWriteImageToWebClientMessage(WriteImageToWebClientStatus.Success, imageInfo);

            // Assert
            Assert.NotNull(item);
            Assert.Equal(imageInfo, item.ImageTiffInfo);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void Equals_SameInstance_ReturnsTrue()
        {
            // Arrange
            var item = CreateTestMessage();

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
            var item1 = CreateTestMessage();
            var item2 = CreateTestMessage();

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
            var item1 = CreateTestMessage();
            var item2 = CreateTestMessage(true);

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
            var item = CreateTestMessage();

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
            var item1 = CreateTestMessage();
            var item2 = CreateTestMessage();

            // Act
            // Assert
            Assert.Equal(item1.GetHashCode(), item2.GetHashCode());
        }

        private ImageTiffInfo CreateImageInfo(bool variant = false)
        {
            var imageId = variant ? new ImageId("77fbc7618e5c47a2a181ae5a9a74a01e") : new ImageId("97c921f0ecfc411b9997bd6720ceb26b");
            var fileName = "testFileName";
            var imageUrl = "testImageUrl";
            var tiffSize = 168961;
            var tiffWidth = 1178;
            var tiffHeight = 544;

            return new ImageTiffInfo(imageId, fileName, imageUrl, tiffSize, tiffWidth, tiffHeight);
        }

        private ReplyWriteImageToWebClientMessage CreateTestMessage(bool variant = false)
        {
            return new ReplyWriteImageToWebClientMessage(WriteImageToWebClientStatus.Success, CreateImageInfo(variant));
        }
    }
}
