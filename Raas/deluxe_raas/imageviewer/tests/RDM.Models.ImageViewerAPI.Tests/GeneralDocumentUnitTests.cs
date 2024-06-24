using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using RDM.Core;
using RDM.Model.Itms;
using Xunit;

namespace RDM.Models.ImageViewerAPI.Tests
{
    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Test files are not styled.")]
    [SuppressMessage("StyleCop.CSharp.LayoutRules", "*", Justification = "Test files are not styled.")]
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "*", Justification = "Test files are not styled.")]
    [SuppressMessage("StyleCop.CSharp.OrderingRules", "*", Justification = "Test files are not styled.")]
    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "*", Justification = "Test files are not styled.")]
    [SuppressMessage("StyleCop.CSharp.SpacingRules", "*", Justification = "Test files are not styled.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "Test files are not styled.")]
    public class GeneralDocumentUnitTests
    {
        public static IEnumerable<object[]> EqualInstances()
        {
            return new List<object[]>
            {
                new object[] { new GeneralDocument("testDoc", OneSidedPages), new GeneralDocument("testDoc", OneSidedPages) },
                new object[] { new GeneralDocument("testDoc", TwoSidedPages), new GeneralDocument("testDoc", TwoSidedPages) },
                new object[] { new GeneralDocument(EmptyName, TwoSidedPages), new GeneralDocument(EmptyName, TwoSidedPages) }
            };
        }

        [Theory]
        [MemberData(nameof(EqualInstances))]
        [Trait("Category", "Unit")]
        public void Equals_EqualInstances_ReturnsTrue(GeneralDocument left, GeneralDocument right)
        {
            // Arrange

            // Act, Assert
            Assert.True(left.Equals(right));
        }

        public static IEnumerable<object[]> UnequalInstances()
        {
            return new List<object[]>
            {
                new object[] { new GeneralDocument("testDoc", OneSidedPages), new GeneralDocument(EmptyName, OneSidedPages) },
                new object[] { new GeneralDocument("testDoc", OneSidedPages), new GeneralDocument("testDoc", TwoSidedPages) },
                new object[] { new GeneralDocument("testDoc", TwoSidedPages), new GeneralDocument(EmptyName, TwoSidedPages) },
                new object[] { new GeneralDocument("testDoc", TwoSidedPages), new GeneralDocument("testDoc", OneSidedPages) }
            };
        }

        [Theory]
        [MemberData(nameof(UnequalInstances))]
        [Trait("Category", "Unit")]
        public void Equals_UnequalInstances_ReturnsFalse(GeneralDocument left, GeneralDocument right)
        {
            // Arrange

            // Act, Assert
            Assert.False(left.Equals(right));
        }

        private static Maybe<string> EmptyName => Maybe<string>.Empty();

        private static List<GeneralDocumentPage> OneSidedPages = new List<GeneralDocumentPage>()
        {
            new GeneralDocumentPage(
                0,
                new Image(new byte[] { 1, 2, 3, 4 }, "image/png", 10, 20),
                Maybe<Image>.Empty()
            )
        };

        private static List<GeneralDocumentPage> TwoSidedPages = new List<GeneralDocumentPage>()
        {
            new GeneralDocumentPage(
                0,
                new Image(new byte[] { 1, 2, 3, 4 }, "image/png", 10, 20),
                new Image(new byte[] { 5, 6, 7, 8 }, "image/png", 10, 20)
            )
        };
    }
}
