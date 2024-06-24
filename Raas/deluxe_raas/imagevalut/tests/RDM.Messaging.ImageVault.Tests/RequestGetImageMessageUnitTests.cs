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
    public class RequestGetImageMessageUnitTests
    {
        private readonly RequestIdentifier _requestId;
        private readonly ImageId _imageId;

        public RequestGetImageMessageUnitTests()
        {
            _requestId = new RequestIdentifier("030833e9ffb84badab5f938b0dc9925f");
            _imageId = new ImageId("08a5971d949a4204be8b0f0e2eabc3ba");
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void RequestGetImageMessage_Serialize_ToString()
        {
            // Arrange
            var item = CreateTestMessage();

            // Act
            var result = JsonConvert.SerializeObject(item);

            // Assert
            Assert.Equal("{\"RequestId\":{\"Value\":\"030833e9ffb84badab5f938b0dc9925f\"},\"ImageId\":{\"Value\":\"97c921f0ecfc411b9997bd6720ceb26b\"},\"Width\":100}", result);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void RequestGetImageMessage_Deserialize_FromString()
        {
            // Arrange
            var item = CreateTestMessage();
            var json = "{\"RequestId\":{\"Value\":\"030833e9ffb84badab5f938b0dc9925f\"},\"ImageId\":{\"Value\":\"97c921f0ecfc411b9997bd6720ceb26b\"},\"Width\":100}";

            // Act
            var result = JsonConvert.DeserializeObject<RequestGetImageMessage>(json);

            // Assert
            Assert.Equal(item, result);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void Ctor_NullImageId_ThrowsException()
        {
            // Act
            var exception = Record.Exception(() => new RequestGetImageMessage(_requestId, null));

            // Assert
            Assert.NotNull(exception);
            Assert.IsType<ArgumentNullException>(exception);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void Ctor_EmptyImageId_ThrowsException()
        {
            // Act
            var exception = Record.Exception(() => new RequestGetImageMessage(_requestId, ImageId.Empty));

            // Assert
            Assert.NotNull(exception);
            Assert.IsType<ArgumentException>(exception);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void Ctor_NullRequestId_ThrowsException()
        {
            // Act
            var exception = Record.Exception(() => new RequestGetImageMessage(null, _imageId));

            // Assert
            Assert.NotNull(exception);
            Assert.IsType<ArgumentNullException>(exception);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void Ctor_EmptyRequestId_ThrowsException()
        {
            // Act
            var exception = Record.Exception(() => new RequestGetImageMessage(RequestIdentifier.Empty, _imageId));

            // Assert
            Assert.NotNull(exception);
            Assert.IsType<ArgumentException>(exception);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void Ctor_Parameterless_MatchingProperties()
        {
            // Act
            var item = Activator.CreateInstance<RequestGetImageMessage>();

            // Assert
            Assert.Equal(RequestGetImageMessage.RabbitExchange, item.ExchangeName);
            Assert.Equal(RequestGetImageMessage.RabbitQueue, item.DefaultQueueName);
            Assert.Equal(RequestGetImageMessage.RabbitKey, item.RoutingKey);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void Ctor_ValidParameters_ObjectCreated()
        {
            // Arrange

            // Act
            var item = new RequestGetImageMessage(_requestId, _imageId);

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

        private RequestGetImageMessage CreateTestMessage(bool variant = false)
        {
            return new RequestGetImageMessage(_requestId, variant ? new ImageId("77fbc7618e5c47a2a181ae5a9a74a01e") : new ImageId("97c921f0ecfc411b9997bd6720ceb26b"), 100);
        }
    }
}
