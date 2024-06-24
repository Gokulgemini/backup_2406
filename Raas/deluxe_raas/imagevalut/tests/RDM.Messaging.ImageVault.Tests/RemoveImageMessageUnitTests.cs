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
    public class RemoveImageMessageUnitTests
    {
        private readonly RequestIdentifier _requestId;
        private readonly ImageId _imageId;

        public RemoveImageMessageUnitTests()
        {
            _requestId = new RequestIdentifier("030833e9ffb84badab5f938b0dc9925f");
            _imageId = new ImageId("30086f1616034d4ba7012554856ddd51");
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void RemoveImageMessage_Serialize_ToString()
        {
            // Arrange
            var item = CreateTestMessage();

            // Act
            var result = JsonConvert.SerializeObject(item);

            // Assert
            Assert.Equal("{\"RequestId\":{\"Value\":\"030833e9ffb84badab5f938b0dc9925f\"},\"ImageId\":{\"Value\":\"97c921f0ecfc411b9997bd6720ceb26b\"}}", result);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void RemoveImageMessage_Deserialize_FromString()
        {
            // Arrange
            var item = CreateTestMessage();
            var json = "{\"RequestId\":{\"Value\":\"030833e9ffb84badab5f938b0dc9925f\"},\"ImageId\":{\"Value\":\"97c921f0ecfc411b9997bd6720ceb26b\"}}";

            // Act
            var result = JsonConvert.DeserializeObject<RemoveImageMessage>(json);

            // Assert
            Assert.Equal(item, result);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void Ctor_NullImageId_ThrowsException()
        {
            // Act
            var exception = Record.Exception(() => new RemoveImageMessage(_requestId, null));

            // Assert
            Assert.NotNull(exception);
            Assert.IsType<ArgumentNullException>(exception);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void Ctor_EmptyImageId_ThrowsException()
        {
            // Act
            var exception = Record.Exception(() => new RemoveImageMessage(_requestId, ImageId.Empty));

            // Assert
            Assert.NotNull(exception);
            Assert.IsType<ArgumentException>(exception);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void Ctor_NullRequestId_ThrowsException()
        {
            // Act
            var exception = Record.Exception(() => new RemoveImageMessage(null, _imageId));

            // Assert
            Assert.NotNull(exception);
            Assert.IsType<ArgumentNullException>(exception);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void Ctor_EmptyRequestId_ThrowsException()
        {
            // Act
            var exception = Record.Exception(() => new RemoveImageMessage(RequestIdentifier.Empty, _imageId));

            // Assert
            Assert.NotNull(exception);
            Assert.IsType<ArgumentException>(exception);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void Ctor_Parameterless_MatchingProperties()
        {
            // Act
            var item = Activator.CreateInstance<RemoveImageMessage>();

            // Assert
            Assert.Equal(RemoveImageMessage.RabbitExchange, item.ExchangeName);
            Assert.Equal(RemoveImageMessage.RabbitQueue, item.DefaultQueueName);
            Assert.Equal(RemoveImageMessage.RabbitKey, item.RoutingKey);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void Ctor_ValidParameters_ObjectCreated()
        {
            // Arrange

            // Act
            var item = new RemoveImageMessage(_requestId, _imageId);

            // Assert
            Assert.NotNull(item);
            Assert.Equal(_imageId, item.ImageId);
            Assert.Equal(_requestId, item.RequestId);
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

        private RemoveImageMessage CreateTestMessage(bool variant = false)
        {
            return new RemoveImageMessage(_requestId, variant ? new ImageId("77fbc7618e5c47a2a181ae5a9a74a01e") : new ImageId("97c921f0ecfc411b9997bd6720ceb26b"));
        }
    }
}
