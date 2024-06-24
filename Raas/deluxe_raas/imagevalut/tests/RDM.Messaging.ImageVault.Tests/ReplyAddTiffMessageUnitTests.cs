using System;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;
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
    public class ReplyAddTiffMessageUnitTests
    {
        [Fact]
        [Trait("Category", "Unit")]
        public void ReplyAddTiffMessage_Serialize_ToString()
        {
            // Arrange
            var item = CreateTestMessage();

            // Act
            var result = JsonConvert.SerializeObject(item);

            // Assert
            Assert.Equal("{\"Status\":\"Success\",\"ImageId\":{\"Value\":\"97c921f0ecfc411b9997bd6720ceb26b\"}}", result);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void ReplyAddTiffMessage_Deserialize_FromString()
        {
            // Arrange
            var item = CreateTestMessage();
            var json = "{\"Status\":\"Success\",\"ImageId\":{\"Value\":\"97c921f0ecfc411b9997bd6720ceb26b\"}}";

            // Act
            var result = JsonConvert.DeserializeObject<ReplyAddTiffMessage>(json);

            // Assert
            Assert.Equal(item, result);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void Ctor_NullImageId_ThrowsException()
        {
            // Act
            var exception = Record.Exception(() => new ReplyAddTiffMessage(AddImageStatus.Success, null));

            // Assert
            Assert.NotNull(exception);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void Ctor_Parameterless_MatchingProperties()
        {
            // Act
            var item = Activator.CreateInstance<ReplyAddTiffMessage>();

            // Assert
            Assert.Equal(ReplyAddTiffMessage.RabbitExchange, item.ExchangeName);
            Assert.Equal(ReplyAddTiffMessage.RabbitQueue, item.DefaultQueueName);
            Assert.Equal(ReplyAddTiffMessage.RabbitKey, item.RoutingKey);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void Ctor_ValidParameters_ObjectCreated()
        {
            // Arrange
            var imageId = new ImageId("30086f1616034d4ba7012554856ddd51");

            // Act
            var item = new ReplyAddTiffMessage(AddImageStatus.Success, imageId);

            // Assert
            Assert.NotNull(item);
            Assert.Equal(imageId, item.ImageId);
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

        private ReplyAddTiffMessage CreateTestMessage(bool variant = false)
        {
            return new ReplyAddTiffMessage(AddImageStatus.Success, variant ? new ImageId("77fbc7618e5c47a2a181ae5a9a74a01e") : new ImageId("97c921f0ecfc411b9997bd6720ceb26b"));
        }
    }
}
