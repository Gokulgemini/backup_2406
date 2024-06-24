using System;
using System.Collections.Generic;
using System.Linq;
using OpenTracing.Tag;
using RDM.Core;
using RDM.Core.SqlServer;
using RDM.Data.ImageVault.Legacy;
using RDM.Model.Itms;
using RDM.Statistician;

namespace RDM.Data.ImageVault.SqlServer.Legacy
{
    /// <summary>
    /// Access Legacy WebClient DB, this Repo connection string is Read-only.
    /// If this needs to change in the future we either need a separate Repo
    /// or to discuss with DevOps and IT(Dan McMicheal) about changing this to full access
    /// </summary>
    public class WebClientImageRepository : SqlServerRepository, IWebClientImageRepository
    {
        private readonly IRequestDataAccessor _requestDataAccessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebClientImageRepository" /> class
        /// with the supplied options.
        /// </summary>
        /// <param name="options">The options used to configure the database access.</param>
        /// <param name="requestDataAccessor">The request data accessor.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="options" /> or <param name="requestDataAccessor" /> is <c>null</c>.
        /// </exception>
        public WebClientImageRepository(IDictionary<string, string> options, IRequestDataAccessor requestDataAccessor)
            : base(options)
        {
            Contract.Requires<ArgumentNullException>(requestDataAccessor != null, nameof(requestDataAccessor));

            _requestDataAccessor = requestDataAccessor;
        }

        /// <inheritdoc />
        public Result<Error, WebClientImageFileInfo> GetImageFileInfo(string irn, string surface, int page)
        {
            _requestDataAccessor.PerformanceMonitor?.Start("WebClientImageRepository.GetImageFileInfo");

            var query = @"
                    SELECT
                        url.URL AS FileUrl,
                        image.ImageFilename AS Filename
                    FROM
                        tblItem_Base AS item
                        INNER JOIN tblImage_Base AS image ON item.ItemID = image.fItemID
                        INNER JOIN tblURL AS url ON image.fURLID = url.URLID
                    WHERE
                        item.IRN = @IRN
                        AND image.Surface IN (@Surface)
                        AND (image.SheetNumber = @Page OR (@Page = 0 AND SheetNumber IS NULL)) --sometimes SheetNumber is null instead of 0
                    ;";

            var parameters = new Dictionary<string, object>
            {
                { "@IRN", irn },
                { "@Surface", surface },
                { "@Page", page }
            };

            var result = ExecuteLegacyReader(
                    query,
                    parameters,
                    reader =>
                    {
                        var fileUrl = reader.GetString(reader.GetOrdinal("FileUrl"));
                        var filename = reader.GetString(reader.GetOrdinal("Filename"));

                        return new WebClientImageFileInfo(fileUrl, filename);
                    })
                .FirstOrDefault();

            _requestDataAccessor.PerformanceMonitor?.Stop("WebClientImageRepository.GetImageFileInfo");

            return result
                ?? Result<Error, WebClientImageFileInfo>.Failure(
                    new NotFound("Could not find any information on the requested image."));
        }

        /// <inheritdoc />
        public Result<Error, WebClientImageFileInfo> GetImageFileInfo(
            UserId userId,
            string irn,
            int seqNum,
            string surface,
            int page)
        {
            _requestDataAccessor.PerformanceMonitor?.Start("WebClientImageRepository.GetImageFileInfo");

            using (Factory.TraceLogger?.Tracer.BuildSpan("GetImageFileInfo")
                          .WithTag(Tags.Component, "RDM.Data.ImageVault.SqlServer.Legacy.WebClientImageRepository")
                          .StartActive())
            {
                var query = @"
                    SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

                    DECLARE @UserMemberID INT;
                    SELECT @UserMemberID =fMemberID
                    FROM  tblUser
                    WHERE UserID= @UserID ;

                    DECLARE @Items TABLE (FileUrl VARCHAR(255),ImageFilename VARCHAR(255), MemberID INT) ;

                    INSERT INTO @Items (FileUrl ,ImageFilename , MemberID )
                    SELECT
                         u.URL AS FileUrl
                        ,im.ImageFilename AS Filename
                        ,mb.MemberID
                    FROM
                        tblItem_Base AS i
                    JOIN tblImage_Base AS im 
                      ON i.ItemID = im.fItemID
                    JOIN tblURL AS u 
                      ON im.fURLID = u.URLID
                    JOIN tblTransaction_Base AS t
                      ON  t.TransactionID = i.fTransactionID
                    JOIN tblBatch_Base AS b
                      ON t.fBatchID = b.BatchID
                    JOIN tblMember_Base mb
                      ON mb.OwnerCode = b.OwnerCode
                    WHERE
                            i.irn=@IRN
                        AND i.SeqNum = @SeqNum
                        AND im.Surface IN (@Surface)
                        AND ISNULL(im.SheetNumber,0) = @Page ;

                    IF EXISTS (
                            SELECT 1
                            FROM tblUserRestrictAccounts
                            WHERE fUserID =@UserID
                              )
                        BEGIN
                            DELETE i 
                            FROM @Items i
                            LEFT OUTER JOIN tblUserAllowedAccounts a
                              ON (a.AllowedAccountMemberID = i.MemberID
                               AND  a.fUserID = @UserID)
                            WHERE a.AllowedAccountMemberID IS NULL
                        END

                    SELECT 
                           i.FileUrl AS FileUrl
                          ,i.ImageFilename AS Filename
                    FROM @Items i
                    CROSS APPLY fnGetMemberIDs_ASC(i.MemberID) f
                    WHERE f.MemberID = @UserMemberID
                    ;";

                var parameters = new Dictionary<string, object>
                {
                    { "@UserId", int.Parse(userId.Value) },
                    { "@IRN", irn },
                    { "@SeqNum", seqNum },
                    { "@Surface", surface },
                    { "@Page", page }
                };

                var result = ExecuteLegacyReader(
                        query,
                        parameters,
                        reader =>
                        {
                            var fileUrl = reader.GetString(reader.GetOrdinal("FileUrl"));
                            var filename = reader.GetString(reader.GetOrdinal("Filename"));

                            return new WebClientImageFileInfo(fileUrl, filename);
                        })
                    .FirstOrDefault();

                _requestDataAccessor.PerformanceMonitor?.Stop("WebClientImageRepository.GetImageFileInfo");

                return result
                    ?? Result<Error, WebClientImageFileInfo>.Failure(
                        new NotFound("Could not find any information on the requested image."));
            }
        }
    }
}
