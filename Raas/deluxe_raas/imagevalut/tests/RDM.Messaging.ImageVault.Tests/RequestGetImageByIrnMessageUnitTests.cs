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
    public class RequestGetImageByIrnMessageUnitTests
    {
        private readonly RequestIdentifier _requestId;
        private readonly string _tenantId;
        private readonly IrnId _irnId;
        private readonly ImageSurface _surface;
        private readonly int _page;

        public RequestGetImageByIrnMessageUnitTests()
        {
            _requestId = new RequestIdentifier("030833e9ffb84badab5f938b0dc9925f");
            _tenantId = "Default";
            _irnId = new IrnId("EYMG6NTRKU80C04SW4CWWGKW8");
            _surface = ImageSurface.Front;
            _page = 0;
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void RequestGetImageByIrnMessage_Serialize_ToString()
        {
            // Arrange
            var item = CreateTestMessage();

            // Act
            var result = JsonConvert.SerializeObject(item);

            // Assert
            Assert.Equal(
                "{\"RequestId\":{\"Value\":\"030833e9ffb84badab5f938b0dc9925f\"},\"TenantId\":\"Default\",\"IrnId\":{\"Value\":\"BYMG6NTRKU80C04SW4CWWGKW8\"},\"Surface\":\"Front\",\"Page\":0}",
                result);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void RequestGetImageByIrnMessage_Deserialize_FromString()
        {
            // Arrange
            var item = CreateTestMessage();
            var json =
                "{\"RequestId\":{\"Value\":\"030833e9ffb84badab5f938b0dc9925f\"},\"TenantId\":\"Default\",\"IrnId\":{\"Value\":\"BYMG6NTRKU80C04SW4CWWGKW8\"},\"Surface\":\"Front\",\"Page\":0}";

            // Act
            var result = JsonConvert.DeserializeObject<RequestGetImageByIrnMessage>(json);

            // Assert
            Assert.Equal(item, result);
        }

        public static IEnumerable<object[]> CtorInvalidArgumentsException()
        {
            var requestId = new RequestIdentifier("123");
            var tenantId = "Default";
            var irnId = new IrnId("EYMG6NTRKU80C04SW4CWWGKW8");
            var surface = ImageSurface.Front;
            var page = 0;

            return new List<object[]>
            {
                new object[] { null, tenantId, irnId, surface, page },
                new object[] { RequestIdentifier.Empty, tenantId, irnId, surface, page },
                new object[] { requestId, tenantId, null, surface, page },
                new object[] { requestId, tenantId, IrnId.Empty, surface, page },
            };
        }

        [Theory, MemberData(nameof(CtorInvalidArgumentsException))]
        [Trait("Category", "Unit")]
        public void Ctor_InvalidArguments_ThrowsException(
            RequestIdentifier requestId,
            string tenantId,
            IrnId irnId,
            ImageSurface surface,
            int page)
        {
            // Arrange

            // Act
            var exception = Record.Exception(() => new RequestGetImageByIrnMessage(requestId, tenantId, irnId, surface, page));

            // Assert
            Assert.NotNull(exception);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void Ctor_Parameterless_MatchingProperties()
        {
            // Act
            var item = Activator.CreateInstance<RequestGetImageByIrnMessage>();

            // Assert
            Assert.Equal(RequestGetImageByIrnMessage.RabbitExchange, item.ExchangeName);
            Assert.Equal(RequestGetImageByIrnMessage.RabbitQueue, item.DefaultQueueName);
            Assert.Equal(RequestGetImageByIrnMessage.RabbitKey, item.RoutingKey);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void Ctor_ValidParameters_ObjectCreated()
        {
            // Arrange

            // Act
            var item = new RequestGetImageByIrnMessage(_requestId, _tenantId, _irnId, _surface, _page);

            // Assert
            Assert.NotNull(item);
            Assert.Equal(_irnId, item.IrnId);
            Assert.Equal(_surface, item.Surface);
            Assert.Equal(_page, item.Page);
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

        private RequestGetImageByIrnMessage CreateTestMessage(bool variant = false)
        {
            return new RequestGetImageByIrnMessage(
                _requestId,
                _tenantId,
                variant ? new IrnId("EYMG6NTRKU80C04SW4CWWGKW8") : new IrnId("BYMG6NTRKU80C04SW4CWWGKW8"),
                _surface,
                _page);
        }
    }
}
