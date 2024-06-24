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
    public class ReplyGetImageMessageUnitTests
    {
        [Fact]
        [Trait("Category", "Unit")]
        public void ReplyGetImageMessage_Serialize_ToString()
        {
            // Arrange
            var item = CreateTestMessage();

            // Act
            var result = JsonConvert.SerializeObject(item);

            // Assert
            Assert.Equal("{\"Status\":\"Success\",\"Image\":{\"Content\":\"AgQH\",\"MimeType\":\"image/tiff\",\"Width\":10,\"Height\":20}}", result);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void ReplyGetImageMessage_Deserialize_FromString()
        {
            // Arrange
            var item = CreateTestMessage();
            var json = "{\"Status\":\"Success\",\"Image\":{\"Content\":\"AgQH\",\"MimeType\":\"image/tiff\",\"Width\":10,\"Height\":20}}";

            // Act
            var result = JsonConvert.DeserializeObject<ReplyGetImageMessage>(json);

            // Assert
            Assert.Equal(item, result);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void Ctor_NullImage_ThrowsException()
        {
            // Act
            var exception = Record.Exception(() => new ReplyGetImageMessage(GetImageStatus.Success, null));

            // Assert
            Assert.NotNull(exception);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void Ctor_Parameterless_MatchingProperties()
        {
            // Act
            var item = Activator.CreateInstance<ReplyGetImageMessage>();

            // Assert
            Assert.Equal(ReplyGetImageMessage.RabbitExchange, item.ExchangeName);
            Assert.Equal(ReplyGetImageMessage.RabbitQueue, item.DefaultQueueName);
            Assert.Equal(ReplyGetImageMessage.RabbitKey, item.RoutingKey);
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

        private Image CreateTestImage(bool variant = false)
        {
            var content = new byte[] { 2, 4, 7 };
            var mimeType = variant? "image/jpeg" : "image/tiff";
            var image = new Image(content, mimeType, 10, 20);

            return image;
        }

        private ReplyGetImageMessage CreateTestMessage(bool variant = false)
        {
            var image = CreateTestImage(variant);
            return new ReplyGetImageMessage(GetImageStatus.Success, image);
        }
    }
}
