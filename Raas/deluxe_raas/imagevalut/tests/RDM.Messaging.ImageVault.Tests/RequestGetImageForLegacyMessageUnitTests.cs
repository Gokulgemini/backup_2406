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
    public class RequestGetImageForLegacyMessageUnitTests
    {
        private readonly RequestIdentifier _requestId;
        private readonly LegacyTarget _legacyTarget;
        private readonly string _tenantId;
        private readonly UserId _userId;
        private readonly IrnId _irnId;
        private readonly int _seqNum;
        private readonly ImageSurface _surface;
        private readonly int _page;

        public RequestGetImageForLegacyMessageUnitTests()
        {
            _requestId = new RequestIdentifier("030833e9ffb84badab5f938b0dc9925f");
            _legacyTarget = LegacyTarget.Itms;
            _tenantId = "Default";
            _userId = new UserId(1);
            _irnId = new IrnId("EYMG6NTRKU80C04SW4CWWGKW8");
            _seqNum = 1;
            _surface = ImageSurface.Front;
            _page = 0;
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void RequestGetImageForLegacyMessage_Serialize_ToString()
        {
            // Arrange
            var item = CreateTestMessage();

            // Act
            var result = JsonConvert.SerializeObject(item);

            // Assert
            Assert.Equal(
                "{\"RequestId\":{\"Value\":\"030833e9ffb84badab5f938b0dc9925f\"},\"LegacyTarget\":1,\"TenantId\":\"Default\",\"UserId\":{\"Value\":\"1\"},\"IrnId\":{\"Value\":\"BYMG6NTRKU80C04SW4CWWGKW8\"},\"SeqNum\":1,\"Surface\":\"Front\",\"Page\":0}",
                result);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void RequestGetImageForLegacyMessage_Deserialize_FromString()
        {
            // Arrange
            var item = CreateTestMessage();
            var json =
                "{\"RequestId\":{\"Value\":\"030833e9ffb84badab5f938b0dc9925f\"},\"LegacyTarget\":1,\"TenantId\":\"Default\",\"UserId\":{\"Value\":\"1\"},\"IrnId\":{\"Value\":\"BYMG6NTRKU80C04SW4CWWGKW8\"},\"SeqNum\":1,\"Surface\":\"Front\",\"Page\":0}";

            // Act
            var result = JsonConvert.DeserializeObject<RequestGetImageForLegacyMessage>(json);

            // Assert
            Assert.Equal(item, result);
        }

        public static IEnumerable<object[]> CtorInvalidArgumentsException()
        {
            var requestId = new RequestIdentifier("123");
            var legacyTarget = LegacyTarget.Itms;
            var tenantId = "Default";
            var userId = new UserId(1);
            var irnId = new IrnId("EYMG6NTRKU80C04SW4CWWGKW8");
            var seqNum = 1;
            var surface = ImageSurface.Front;
            var page = 0;

            return new List<object[]>
            {
                new object[] { null, legacyTarget, tenantId, userId, irnId, seqNum, surface, page },
                new object[] { RequestIdentifier.Empty, legacyTarget, tenantId, userId, irnId, seqNum, surface, page },
                new object[] { requestId, legacyTarget, tenantId, userId, null, seqNum, surface, page },
                new object[] { requestId, legacyTarget, tenantId, userId, IrnId.Empty, seqNum, surface, page },
                new object[] { requestId, legacyTarget, tenantId, null, irnId, seqNum, surface, page },
                new object[] { requestId, legacyTarget, tenantId, UserId.InvalidUser, irnId, seqNum, surface, page }
            };
        }

        [Theory, MemberData(nameof(CtorInvalidArgumentsException))]
        [Trait("Category", "Unit")]
        public void Ctor_InvalidArguments_ThrowsException(
            RequestIdentifier requestId,
            LegacyTarget legacyTarget,
            string tenantId,
            UserId userId,
            IrnId irnId,
            int seqNum,
            ImageSurface surface,
            int page)
        {
            // Arrange

            // Act
            var exception = Record.Exception(
                () => new RequestGetImageForLegacyMessage(
                    requestId,
                    legacyTarget,
                    tenantId,
                    userId,
                    irnId,
                    seqNum,
                    surface,
                    page));

            // Assert
            Assert.NotNull(exception);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void Ctor_Parameterless_MatchingProperties()
        {
            // Act
            var item = Activator.CreateInstance<RequestGetImageForLegacyMessage>();

            // Assert
            Assert.Equal(RequestGetImageForLegacyMessage.RabbitExchange, item.ExchangeName);
            Assert.Equal(RequestGetImageForLegacyMessage.RabbitQueue, item.DefaultQueueName);
            Assert.Equal(RequestGetImageForLegacyMessage.RabbitKey, item.RoutingKey);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void Ctor_ValidParameters_ObjectCreated()
        {
            // Arrange

            // Act
            var item = new RequestGetImageForLegacyMessage(
                _requestId,
                _legacyTarget,
                _tenantId,
                _userId,
                _irnId,
                _seqNum,
                _surface,
                _page);

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

        private RequestGetImageForLegacyMessage CreateTestMessage(bool variant = false)
        {
            return new RequestGetImageForLegacyMessage(
                _requestId,
                _legacyTarget,
                _tenantId,
                _userId,
                variant ? new IrnId("EYMG6NTRKU80C04SW4CWWGKW8") : new IrnId("BYMG6NTRKU80C04SW4CWWGKW8"),
                _seqNum,
                _surface,
                _page);
        }
    }
}
