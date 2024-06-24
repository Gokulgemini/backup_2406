using System.Diagnostics.CodeAnalysis;
using RDM.Core.SqlServer;

namespace RDM.Data.ImageVault.SqlServer.Tests
{
    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Test files are not styled.")]
    [SuppressMessage("StyleCop.CSharp.LayoutRules", "*", Justification = "Test files are not styled.")]
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "*", Justification = "Test files are not styled.")]
    [SuppressMessage("StyleCop.CSharp.OrderingRules", "*", Justification = "Test files are not styled.")]
    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "*", Justification = "Test files are not styled.")]
    [SuppressMessage("StyleCop.CSharp.SpacingRules", "*", Justification = "Test files are not styled.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "Test files are not styled.")]
    public class ImageVaultFixture : DatabaseFixture
    {
        protected override string Host => @"DWA00120.deluxe.com\DD01";

        protected override string Password => "Z5uRXkOWclQ3dj5ixziL";

        protected override string User => "unittest";

        protected override void CreateRepository()
        {
            var initializer = new ImageVaultRepositoryInitializer(Options);
            initializer.Initialize();
        }
    }
}
