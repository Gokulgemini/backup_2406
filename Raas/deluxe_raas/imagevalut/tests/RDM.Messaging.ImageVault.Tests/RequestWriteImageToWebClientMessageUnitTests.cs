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
    public class RequestWriteImageToWebClientMessageUnitTests
    {
        private readonly RequestIdentifier _requestId;
        private readonly string _tenantId;
        private readonly ImageId _imageId;
        private readonly string _filepath;
        private readonly string _filename;

        public RequestWriteImageToWebClientMessageUnitTests()
        {
            _requestId = new RequestIdentifier("030833e9ffb84badab5f938b0dc9925f");
            _tenantId = "Default";
            _imageId = new ImageId("08a5971d949a4204be8b0f0e2eabc3ba");
            _filepath = "C:\\Temp\\";
            _filename = "IRN_SeqNum_SheetNumber_Surface.tif";
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void RequestWriteTiffToShareMessage_Serialize_ToString()
        {
            // Arrange
            var item = CreateTestMessage();

            // Act
            var result = JsonConvert.SerializeObject(item);

            // Assert
            Assert.Equal(
                "{\"RequestId\":{\"Value\":\"030833e9ffb84badab5f938b0dc9925f\"},\"TenantId\":\"Default\",\"ImageId\":{\"Value\":\"97c921f0ecfc411b9997bd6720ceb26b\"},\"Filepath\":\"C:\\\\Temp\\\\\",\"Filename\":\"IRN_SeqNum_SheetNumber_Surface.tif\"}",
                result);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void RequestWriteTiffToShareMessage_Deserialize_FromString()
        {
            // Arrange
            var item = CreateTestMessage();
            var json =
                "{\"RequestId\":{\"Value\":\"030833e9ffb84badab5f938b0dc9925f\"},\"TenantId\":\"Default\",\"ImageId\":{\"Value\":\"97c921f0ecfc411b9997bd6720ceb26b\"},\"Filepath\":\"C:\\\\Temp\\\\\",\"Filename\":\"IRN_SeqNum_SheetNumber_Surface.tif\"}";

            // Act
            var result = JsonConvert.DeserializeObject<RequestWriteImageToWebClientMessage>(json);

            // Assert
            Assert.Equal(item, result);
        }

        public static IEnumerable<object[]> CtorInvalidArgumentsException()
        {
            var requestId = new RequestIdentifier("030833e9ffb84badab5f938b0dc9925f");
            var tenantId = "Default";
            var imageId = new ImageId("08a5971d949a4204be8b0f0e2eabc3ba");
            var filepath = "C:\\Temp\\";
            var filename = "IRN_SeqNum_SheetNumber_Surface.tif";

            return new List<object[]>
            {
                new object[] { null, tenantId, imageId, filepath, filename },
                new object[] { RequestIdentifier.Empty, tenantId, imageId, filepath, filename },
                new object[] { requestId, tenantId, null, filepath, filename },
                new object[] { requestId, tenantId, ImageId.Empty, filepath, filename },
                new object[] { requestId, tenantId, imageId, null, filename },
                new object[] { requestId, tenantId, imageId, string.Empty, filename },
                new object[] { requestId, tenantId, imageId, "           ", filename },
                new object[] { requestId, tenantId, imageId, filepath, null },
                new object[] { requestId, tenantId, imageId, filepath, string.Empty },
                new object[] { requestId, tenantId, imageId, filepath, "           " }
            };
        }

        [Theory, MemberData(nameof(CtorInvalidArgumentsException))]
        [Trait("Category", "Unit")]
        public void Ctor_InvalidArguments_ThrowsException(
            RequestIdentifier requestId,
            string tenantId,
            ImageId imageId,
            string filepath,
            string filename)
        {
            // Arrange

            // Act
            var exception = Record.Exception(
                () => new RequestWriteImageToWebClientMessage(requestId, tenantId, imageId, filepath, filename));

            // Assert
            Assert.NotNull(exception);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void Ctor_Parameterless_MatchingProperties()
        {
            // Act
            var item = Activator.CreateInstance<RequestWriteImageToWebClientMessage>();

            // Assert
            Assert.Equal(RequestWriteImageToWebClientMessage.RabbitExchange, item.ExchangeName);
            Assert.Equal(RequestWriteImageToWebClientMessage.RabbitQueue, item.DefaultQueueName);
            Assert.Equal(RequestWriteImageToWebClientMessage.RabbitKey, item.RoutingKey);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void Ctor_ValidParameters_ObjectCreated()
        {
            // Arrange

            // Act
            var item = new RequestWriteImageToWebClientMessage(_requestId, _tenantId, _imageId, _filepath, _filename);

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

        private RequestWriteImageToWebClientMessage CreateTestMessage(bool variant = false)
        {
            return new RequestWriteImageToWebClientMessage(
                _requestId,
                _tenantId,
                variant ? new ImageId("77fbc7618e5c47a2a181ae5a9a74a01e") : new ImageId("97c921f0ecfc411b9997bd6720ceb26b"),
                _filepath,
                _filename);
        }
    }
}
