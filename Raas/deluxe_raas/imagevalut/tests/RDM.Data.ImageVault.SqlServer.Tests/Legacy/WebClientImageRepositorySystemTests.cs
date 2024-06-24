using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using NSubstitute;
using RDM.Core;
using RDM.Core.SqlServer;
using RDM.Data.ImageVault.Legacy;
using RDM.Data.ImageVault.SqlServer.Legacy;
using RDM.Model.Itms;
using Xunit;

namespace RDM.Data.ImageVault.SqlServer.Tests.Legacy
{
    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Test files are not styled.")]
    [SuppressMessage("StyleCop.CSharp.LayoutRules", "*", Justification = "Test files are not styled.")]
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "*", Justification = "Test files are not styled.")]
    [SuppressMessage("StyleCop.CSharp.OrderingRules", "*", Justification = "Test files are not styled.")]
    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "*", Justification = "Test files are not styled.")]
    [SuppressMessage("StyleCop.CSharp.SpacingRules", "*", Justification = "Test files are not styled.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "Test files are not styled.")]
    public class WebClientImageRepositorySystemTests : IClassFixture<WebClientImageRepositoryFixture>, IDisposable
    {
        private readonly IRequestDataAccessor _requestDataAccessor;
        private readonly Dictionary<string, string> _options;
        private readonly IWebClientImageRepository _repo;
        private readonly WebClientImageRepositoryFixture _fixture;

        public WebClientImageRepositorySystemTests(WebClientImageRepositoryFixture fixture)
        {
            _fixture = fixture;
            _requestDataAccessor = Substitute.For<IRequestDataAccessor>();
            _requestDataAccessor.RequestId.Returns(new RequestIdentifier("1"));
            _options = new Dictionary<string, string>
            {
                { SqlServerRepositoryOption.ConnectionString, _fixture.ConnectionString }
            };

            _repo = new WebClientImageRepository(_options, _requestDataAccessor);
        }

        public void Dispose()
        {
            _fixture.TruncateAllTables();
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void Ctor_NullRequestDataAccessor_ThrowsException()
        {
            // Arrange

            // Act

            // Assert
            Assert.ThrowsAny<ArgumentNullException>(() => new WebClientImageRepository(_options, null));
        }

        [Fact]
        [Trait("Category", "System")]
        public void GetImageFileInfo_ItemNotFound_ReturnsNotFound()
        {
            // Arrange
            var irnId = "12345678901234567890";
            var surface = "Front";
            var pageNumber = 4;

            // Act
            var result = _repo.GetImageFileInfo(irnId, surface, pageNumber);

            // Assert
            Assert.True(result.IsFailure);
            Assert.IsType<NotFound>(result.FailureValue);
        }

        [Fact]
        [Trait("Category", "System")]
        public void GetImageFileInfo_ItemFound_ReturnsSuccess()
        {
            // Arrange
            var irnId = "C6104JGERO86KBOOEWCRUOIIM";
            var surface = "Front";
            var pageNumber = 0;

            _fixture.AddTblUrlRecord();
            _fixture.AddTblItemRecord(irnId);
            _fixture.AddTblImageRecord(surface, pageNumber);

            // Act
            var result = _repo.GetImageFileInfo(irnId, surface, pageNumber);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        [Trait("Category", "System")]
        public void GetImageFileInfo_ForLegacy_ItemNotFound_ReturnsNotFound()
        {
            // Arrange
            var userId = 59835;
            var irnId = "12345678901234567890";
            var seqNum = 1;
            var surface = "Front";
            var pageNumber = 4;

            // Act
            var result = _repo.GetImageFileInfo(new UserId(userId), irnId, seqNum, surface, pageNumber);

            // Assert
            Assert.True(result.IsFailure);
            Assert.IsType<NotFound>(result.FailureValue);
        }

        [Fact]
        [Trait("Category", "System")]
        public void GetImageFileInfo_ForLegacy_ItemFound_ReturnsSuccess()
        {
            // Arrange
            var userId = 59835;
            var irnId = "C6104JGERO86KBOOEWCRUOIIM";
            var seqNum = 1;
            var surface = "Front";
            var pageNumber = 0;

            var expectedFilename = "88884911-e2de-4eed-9202-d53498f7e0d8.tiff";

            _fixture.AddTblUrlRecord();
            _fixture.AddTblItemRecord(irnId);
            _fixture.AddTblImageRecord(surface, pageNumber, expectedFilename);
            _fixture.AddTblUserRecord(userId);

            // Act
            var result = _repo.GetImageFileInfo(new UserId(userId), irnId, seqNum, surface, pageNumber);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(expectedFilename, result.Value.Filename);
        }
    }
}
