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
    public class RequestAddTiffMessageUnitTests
    {
        private readonly RequestIdentifier _requestId;

        public RequestAddTiffMessageUnitTests()
        {
            _requestId = new RequestIdentifier("030833e9ffb84badab5f938b0dc9925f");
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void RequestAddTiffMessage_Serialize_ToString()
        {
            // Arrange
            var item = CreateTestMessage();

            // Act
            var result = JsonConvert.SerializeObject(item);

            // Assert
            Assert.Equal("{\"RequestId\":{\"Value\":\"030833e9ffb84badab5f938b0dc9925f\"},\"Content\":\"AgYM\"}", result);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void ReplyAddTiffMessage_Deserialize_FromString()
        {
            // Arrange
            var item = CreateTestMessage();
            var json = "{\"RequestId\":{\"Value\":\"030833e9ffb84badab5f938b0dc9925f\"},\"Content\":\"AgYM\"}";

            // Act
            var result = JsonConvert.DeserializeObject<RequestAddTiffMessage>(json);

            // Assert
            Assert.Equal(item, result);
        }

        public static IEnumerable<object[]> CtorInvalidArgumentsException()
        {
            var requestId = new RequestIdentifier("030833e9ffb84badab5f938b0dc9925f");
            var content = new byte[] { 1, 2, 3, 4 };

            return new List<object[]>
            {
                new object[] { null, content },
                new object[] { RequestIdentifier.Empty, content },
                new object[] { requestId, null },
                new object[] { requestId, new byte[] { } }
            };
        }

        [Theory, MemberData(nameof(CtorInvalidArgumentsException))]
        [Trait("Category", "Unit")]
        public void Ctor_InvalidArguments_ThrowsException(
            RequestIdentifier requestId,
            byte[] content
            )
        {
            // Arrange

            // Act
            var exception = Record.Exception(() => new RequestAddTiffMessage(requestId, content));

            // Assert
            Assert.NotNull(exception);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void Ctor_Parameterless_MatchingProperties()
        {
            // Act
            var item = Activator.CreateInstance<RequestAddTiffMessage>();

            // Assert
            Assert.Equal(RequestAddTiffMessage.RabbitExchange, item.ExchangeName);
            Assert.Equal(RequestAddTiffMessage.RabbitQueue, item.DefaultQueueName);
            Assert.Equal(RequestAddTiffMessage.RabbitKey, item.RoutingKey);
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

        private RequestAddTiffMessage CreateTestMessage(bool variant = false)
        {
            var content = variant ? new byte[] { 3, 7, 11 } : new byte[] { 2, 6, 12 };

            return new RequestAddTiffMessage(_requestId, content);
        }
    }
}
