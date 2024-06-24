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
    public class GetImageStatusUnitTests
    {
        [Fact]
        [Trait("Category", "Unit")]
        public void GetImageStatus_Serialize_ToString()
        {
            // Arrange
            var item = GetImageStatus.Success;

            // Act
            var result = JsonConvert.SerializeObject(item);

            // Assert
            Assert.Equal("\"Success\"", result);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void GetImageStatus_Deserialize_FromString()
        {
            // Arrange
            var item = GetImageStatus.Success;
            var json = "\"Success\"";

            // Act
            var result = JsonConvert.DeserializeObject<GetImageStatus>(json);

            // Assert
            Assert.Equal(item, result);
        }
    }
}
