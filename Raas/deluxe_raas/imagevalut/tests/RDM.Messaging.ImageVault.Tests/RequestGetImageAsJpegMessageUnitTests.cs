using System;
using System.Collections.Generic;
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
    public class RequestGetImageAsJpegMessageUnitTests
    {
        private readonly RequestIdentifier _requestId;
        private readonly ImageId _imageId;
        private readonly int _width;

        public RequestGetImageAsJpegMessageUnitTests()
        {
            _requestId = new RequestIdentifier("030833e9ffb84badab5f938b0dc9925f");
            _imageId = new ImageId("08a5971d949a4204be8b0f0e2eabc3ba");
            _width = 5;
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void RequestGetImageAsJpegMessage_Serialize_ToString()
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
        public void RequestGetImageAsJpegMessage_Deserialize_FromString()
        {
            // Arrange
            var item = CreateTestMessage();
            var json = "{\"RequestId\":{\"Value\":\"030833e9ffb84badab5f938b0dc9925f\"},\"ImageId\":{\"Value\":\"97c921f0ecfc411b9997bd6720ceb26b\"},\"Width\":100}";

            // Act
            var result = JsonConvert.DeserializeObject<RequestGetImageAsJpegMessage>(json);

            // Assert
            Assert.Equal(item, result);
        }

        public static IEnumerable<object[]> CtorInvalidArguments()
        {
            var requestId = new RequestIdentifier("030833e9ffb84badab5f938b0dc9925f");
            var imageId = new ImageId("589ecab9fef749dc9095074a836674d9");
            var width = 5;

            return new List<object[]>
            {
                new object[] { null, imageId, width },
                new object[] { RequestIdentifier.Empty, imageId, width },
                new object[] { requestId, null, width },
                new object[] { requestId, ImageId.Empty, width }
            };
        }

        [Theory, MemberData(nameof(CtorInvalidArguments))]
        [Trait("Category", "Unit")]
        public void Ctor_InvalidArguments_ThrowsException(
            RequestIdentifier requestId,
            ImageId imageId,
            int width)
        {
            // Arrange

            // Act
            var exception = Record.Exception(() => new RequestGetImageAsJpegMessage(requestId, imageId, width));

            // Assert
            Assert.NotNull(exception);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void Ctor_Parameterless_MatchingProperties()
        {
            // Act
            var item = Activator.CreateInstance<RequestGetImageAsJpegMessage>();

            // Assert
            Assert.Equal(RequestGetImageAsJpegMessage.RabbitExchange, item.ExchangeName);
            Assert.Equal(RequestGetImageAsJpegMessage.RabbitQueue, item.DefaultQueueName);
            Assert.Equal(RequestGetImageAsJpegMessage.RabbitKey, item.RoutingKey);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void Ctor_ValidParameters_ObjectCreated()
        {
            // Arrange

            // Act
            var item = new RequestGetImageAsJpegMessage(_requestId, _imageId, _width);

            // Assert
            Assert.NotNull(item);
            Assert.Equal(_imageId, item.ImageId);
            Assert.Equal(_requestId, item.RequestId);
            Assert.Equal(_width, item.Width);
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

        private RequestGetImageAsJpegMessage CreateTestMessage(bool variant = false)
        {
            return new RequestGetImageAsJpegMessage(_requestId, variant ? new ImageId("77fbc7618e5c47a2a181ae5a9a74a01e") : new ImageId("97c921f0ecfc411b9997bd6720ceb26b"), 100);
        }
    }
}
