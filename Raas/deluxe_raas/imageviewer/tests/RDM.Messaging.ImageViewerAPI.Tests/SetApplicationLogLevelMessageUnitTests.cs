using System;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;
using Serilog.Events;
using Xunit;

namespace RDM.Messaging.ImageViewerAPI.Tests
{
    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Test files are not styled.")]
    [SuppressMessage("StyleCop.CSharp.LayoutRules", "*", Justification = "Test files are not styled.")]
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "*", Justification = "Test files are not styled.")]
    [SuppressMessage("StyleCop.CSharp.OrderingRules", "*", Justification = "Test files are not styled.")]
    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "*", Justification = "Test files are not styled.")]
    [SuppressMessage("StyleCop.CSharp.SpacingRules", "*", Justification = "Test files are not styled.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "Test files are not styled.")]
    public class SetApplicationLogLevelMessageUnitTests
    {
        [Fact]
        [Trait("Category", "Unit")]
        public void SetApplicationLogLevelMessage_Serialize_ToString()
        {
            // Arrange
            var item = CreateTestMessage();

            // Act
            var result = JsonConvert.SerializeObject(item);

            // Assert
            Assert.Equal("{\"NewLevel\":2}", result);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void SetApplicationLogLevelMessage_Deserialize_FromString()
        {
            // Arrange
            var item = CreateTestMessage();
            var json = "{\"NewLevel\":2}";

            // Act
            var result = JsonConvert.DeserializeObject<SetApplicationLogLevelMessage>(json);

            // Assert
            Assert.Equal(item, result);
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
        public void Equals_DifferentType_ReturnsFalse()
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

        [Fact]
        [Trait("Category", "Unit")]
        public void Ctor_Parameterless_MatchingProperties()
        {
            // Arrange

            // Act
            var item = Activator.CreateInstance<SetApplicationLogLevelMessage>();

            // Assert
            Assert.Equal(SetApplicationLogLevelMessage.RabbitKey, item.RoutingKey);
            Assert.Equal(SetApplicationLogLevelMessage.RabbitQueue, item.DefaultQueueName);
            Assert.Equal(SetApplicationLogLevelMessage.RabbitExchange, item.ExchangeName);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void Ctor_AllParameters_PropertiesSet()
        {
            // Arrange

            // Act
            var item = CreateTestMessage();

            // Assert
            Assert.Equal(LogEventLevel.Information, item.NewLevel);
        }

        private SetApplicationLogLevelMessage CreateTestMessage(bool variant = false)
        {
            var result = new SetApplicationLogLevelMessage(variant ? LogEventLevel.Fatal : LogEventLevel.Information);

            return result;
        }
    }
}
