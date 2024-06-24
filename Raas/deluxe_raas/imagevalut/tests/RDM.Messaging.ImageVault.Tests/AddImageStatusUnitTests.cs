using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;
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
    public class AddImageStatusUnitTests
    {
        [Fact]
        [Trait("Category", "Unit")]
        public void AddImageStatus_Serialize_ToString()
        {
            // Arrange
            var item = AddImageStatus.Success;

            // Act
            var result = JsonConvert.SerializeObject(item);

            // Assert
            Assert.Equal("\"Success\"", result);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void AddImageStatus_Deserialize_FromString()
        {
            // Arrange
            var item = AddImageStatus.Success;
            var json = "\"Success\"";

            // Act
            var result = JsonConvert.DeserializeObject<AddImageStatus>(json);

            // Assert
            Assert.Equal(item, result);
        }
    }
}
