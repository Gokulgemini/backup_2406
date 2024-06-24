using System;
using System.Collections.Generic;
using System.Linq;
using RDM.Core;
using RDM.Core.SqlServer;
using RDM.Data.ImageVault.Legacy;
using RDM.Model.Itms;

namespace RDM.Data.ImageVault.SqlServer.Legacy
{
    /// <summary>
    /// Access Legacy ITMS DB, this Repo connection string is Read-only.
    /// If this needs to change in the future we either need a separate Repo
    /// or to discuss with DevOps and IT(Dan McMicheal) about changing this to full access
    /// </summary>
    public class ItmsImageRepository : SqlServerRepository, IItmsImageRepository
    {
        private readonly IRequestDataAccessor _requestDataAccessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="ItmsImageRepository" /> class
        /// with the supplied options.
        /// </summary>
        /// <param name="options">The options used to configure the database access.</param>
        /// <param name="requestDataAccessor">The request data accessor.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="options" /> or <param name="requestDataAccessor" /> is <c>null</c>.
        /// </exception>
        public ItmsImageRepository(IDictionary<string, string> options, IRequestDataAccessor requestDataAccessor)
            : base(options)
        {
            Contract.Requires<ArgumentNullException>(requestDataAccessor != null, nameof(requestDataAccessor));

            _requestDataAccessor = requestDataAccessor;
        }

        /// <inheritdoc />
        public Result<Error, ItmsImageFileInfo> GetImageFileInfo(UserId userId, string irn, int seqNum, string surface, int page)
        {
            _requestDataAccessor.PerformanceMonitor?.Start("ItmsImageRepository.GetImageFileInfo");
            using (RDM.Statistician.Factory.TraceLogger?.Tracer.BuildSpan("GetImageFileInfo")
                .WithTag(OpenTracing.Tag.Tags.Component, "RDM.Data.ImageVault.SqlServer.Legacy.ItmsImageRepository")
                .StartActive())
            {
                // query provided by Igor Krouglov and uses ITMS functions
                var query = @"
                    SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

                    DECLARE
                         @ItemID   BIGINT
                        ,@DateFrom DATETIME
                        ,@DateTo   DATETIME
                        ,@ItemMemberID INT
                        ,@UserMemberID INT
                        ,@AccessAllowed INT = 0;

                    SELECT
                         @ItemID   = ItemID
                        ,@DateFrom = DATEADD(dd,-1,InsertTime)
                        ,@DateTo   = DATEADD(dd,1,InsertTime)
                        ,@ItemMemberID = fMemberID
                    FROM tblItem_Base
                    WHERE irn = @IRN
                    AND SeqNum = @SeqNum
                    ;

                    SELECT @UserMemberID = fMemberID
                    FROM   tbluser u
                    WHERE  u.UserID = @UserID ;

                    SELECT @AccessAllowed = 1
                    FROM fnGetChildren(@UserMemberID) f
                    WHERE f.MemberID = @ItemMemberID

                    SELECT @AccessAllowed = 0
                    FROM tblUserRestrictAccounts
                    WHERE fUserID = @UserID
                        AND RestrictAccounts = 1
                        AND @AccessAllowed = 1 ;

                    SELECT @AccessAllowed = 1
                    FROM  fnGetAllowedAccounts(@UserID,7) f
                    WHERE f.MemberID = @ItemMemberID
                        AND @AccessAllowed = 0 ;

                    SELECT
                         u.URL AS FileUrl
                        ,im.ImageFilename AS Filename
                        ,im.fFileID AS FileId
                        ,uar.URL AS ArchiveUrl
                        ,ar.ArchiveFileName
                    FROM tblImage im  WITH (index = IX_tblImage_Part_fItemID)
                    JOIN tblURL  u WITH (index = aaaaatblURL_PK_100)
                        ON u.URLID = im.fURLID
                    LEFT OUTER JOIN tblIFM_ArchiveFiles  a
                        WITH (index = IDX_tblIFM_ArchiveFiles_fFileID_fIFM_ArchiveFileID)
                        ON a.fFileID = im.fFileID
                    LEFT OUTER JOIN tblIFM_ArchiveFile ar
                        WITH (index = PK_tblIFM_ArchiveFile)
                        ON ar.IFM_ArchiveFileID = a.fIFM_ArchiveFileID
                    LEFT OUTER JOIN tblURL uar
                        WITH (index = aaaaatblURL_PK_100)
                        ON uar.URLID = ar.fURLID
                    WHERE
                            im.fItemID = @ItemID
                        AND im.Timestamp BETWEEN @DateFrom AND @DateTo
                        AND im.Surface IN (@Surface)
                        AND im.SheetNumber = @Page
                        AND @AccessAllowed = 1
                    OPTION (FORCE ORDER)
                    ;";

                var parameters = new Dictionary<string, object>
                {
                    { "@UserId", int.Parse(userId.Value) },
                    { "@IRN", irn },
                    { "@SeqNum", seqNum },
                    { "@Surface", surface },
                    { "@Page", page }
                };

                var results = ExecuteLegacyReader(
                        query,
                        parameters,
                        reader =>
                        {
                            var fileUrl = reader.GetString(reader.GetOrdinal("FileUrl"));
                            var filename = reader.GetString(reader.GetOrdinal("Filename"));
                            var fileId = reader.GetInt32(reader.GetOrdinal("FileId"));

                            var archiveUrlCol = reader.GetOrdinal("ArchiveUrl");
                            var archiveUrl = reader.IsDBNull(archiveUrlCol) ? Maybe<string>.Empty() : reader.GetString(archiveUrlCol);

                            var archiveFilenameCol = reader.GetOrdinal("ArchiveFilename");
                            var archiveFilename = reader.IsDBNull(archiveFilenameCol)
                                ? Maybe<string>.Empty()
                                : reader.GetString(archiveFilenameCol);

                            return new ItmsImageFileInfo(fileUrl, filename, fileId, archiveUrl, archiveFilename);
                        })
                    .ToList();

                _requestDataAccessor.PerformanceMonitor?.Stop("ItmsImageRepository.GetImageFileInfo");

                return results.Any()
                    ? results.First()
                    : Result<Error, ItmsImageFileInfo>.Failure(
                        new NotFound("Could not find any information on the requested image."));
            }
        }
    }
}
