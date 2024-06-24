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
    public class ItmsImageRepositorySystemTests : IClassFixture<ItmsImageRepositoryFixture>, IDisposable
    {
        private readonly IRequestDataAccessor _requestDataAccessor;
        private readonly Dictionary<string, string> _options;
        private readonly IItmsImageRepository _repo;
        private readonly ItmsImageRepositoryFixture _fixture;

        public ItmsImageRepositorySystemTests(ItmsImageRepositoryFixture fixture)
        {
            _fixture = fixture;
            _requestDataAccessor = Substitute.For<IRequestDataAccessor>();
            _requestDataAccessor.RequestId.Returns(new RequestIdentifier("1"));
            _options = new Dictionary<string, string>
            {
                {SqlServerRepositoryOption.ConnectionString, _fixture.ConnectionString}
            };

            _repo = new ItmsImageRepository(_options, _requestDataAccessor);
        }

        public void Dispose()
        {
            _fixture.TruncateAllTables();
        }

        [Fact(Skip = "Query updated to use functions in deployed SQL")]
        [Trait("Category", "System")]
        public void GetImageFileInfo_FileAndArchiveInfoExists_ReturnsExpected()
        {
            // Arrange
            var expectedFileInfo = new ItmsImageFileInfo(
                "fileUrl\\",
                "imageFilename",
                4321,
                Maybe<string>.WithValue("archiveUrl\\"),
                Maybe<string>.WithValue("archiveFilename"));

            var irn = "EYMG6NTRKU80C04SW4CWWGKW8";
            var seqNum = 1;
            var itemId = _fixture.AddTblItemRecord(irn);
            var fileUrlId = _fixture.AddTblUrlRecord(expectedFileInfo.FileUrl);
            var archiveUrlId = _fixture.AddTblUrlRecord(expectedFileInfo.ArchiveUrl.Value);
            var archiveFileId = _fixture.AddTblIFM_ArchiveFile(archiveUrlId, expectedFileInfo.ArchiveFilename.Value);
            _fixture.AddTblIFM_ArchiveFiles(archiveFileId, expectedFileInfo.FileId);
            _fixture.AddTblImageRecord(itemId, fileUrlId, expectedFileInfo.FileId, "Front", 0, expectedFileInfo.Filename);
            var userId = 59835;
            _fixture.AddTblUserRecord(userId);

            // Act
            var result = _repo.GetImageFileInfo(new UserId(userId), irn, seqNum, "Front", 0);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(expectedFileInfo, result.Value);
        }

        [Fact(Skip = "Query updated to use functions in deployed SQL")]
        [Trait("Category", "System")]
        public void GetImageFileInfo_NoArchiveInfo_ReturnsExpected()
        {
            // Arrange
            var expectedFileInfo = new ItmsImageFileInfo(
                "fileUrl\\",
                "imageFilename",
                4321,
                Maybe<string>.Empty(),
                Maybe<string>.Empty());

            var irn = "EYMG6NTRKU80C04SW4CWWGKW8";
            var seqNum = 1;
            var itemId = _fixture.AddTblItemRecord(irn);
            var fileUrlId = _fixture.AddTblUrlRecord(expectedFileInfo.FileUrl);
            var userId = 59835;
            _fixture.AddTblUserRecord(userId);
            _fixture.AddTblImageRecord(itemId, fileUrlId, expectedFileInfo.FileId, "Front", 0, expectedFileInfo.Filename);

            // Act
            var result = _repo.GetImageFileInfo(new UserId(userId), irn, seqNum, "Front", 0);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(expectedFileInfo, result.Value);
        }

        [Fact(Skip = "Query updated to use functions in deployed SQL")]
        [Trait("Category", "System")]
        public void GetImageFileInfo_NoInfoFound_ReturnsNotFound()
        {
            // Arrange
            var irn = "EYMG6NTRKU80C04SW4CWWGKW8";
            var seqNum = 1;
            var userId = 59835;

            // Act
            var result = _repo.GetImageFileInfo(new UserId(userId), irn, seqNum, "Front", 0);

            // Assert
            Assert.True(result.IsFailure);
            Assert.IsType<NotFound>(result.FailureValue);
        }
    }
}
