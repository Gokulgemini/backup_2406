using System;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace RDM.Data.ImageVault.SqlServer.Tests.Legacy
{
    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Test files are not styled.")]
    [SuppressMessage("StyleCop.CSharp.LayoutRules", "*", Justification = "Test files are not styled.")]
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "*", Justification = "Test files are not styled.")]
    [SuppressMessage("StyleCop.CSharp.OrderingRules", "*", Justification = "Test files are not styled.")]
    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "*", Justification = "Test files are not styled.")]
    [SuppressMessage("StyleCop.CSharp.SpacingRules", "*", Justification = "Test files are not styled.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "Test files are not styled.")]
    public class ItmsImageRepositoryFixture : IDisposable
    {
        private readonly string _dbName;
        private readonly string _dbSource = @"DWA00120.deluxe.com\DD01";
        private readonly string _dbUserid = @"unittest";
        private readonly string _dbPassword = @"Z5uRXkOWclQ3dj5ixziL";

        public ItmsImageRepositoryFixture()
        {
            _dbName = '_' + Guid.NewGuid().ToString("N");

            ConnectionString = $"data source={_dbSource};initial catalog={_dbName};persist security info=True;user id={_dbUserid};password={_dbPassword};MultipleActiveResultSets=True";

            using (
                var sqlConnection =
                    new SqlConnection(
                        $"data source={_dbSource};persist security info=True;user id={_dbUserid};password={_dbPassword};MultipleActiveResultSets=True")
            )
            {
                sqlConnection.Open();

                using (var command = sqlConnection.CreateCommand())
                {
                    command.CommandText = string.Format("CREATE DATABASE {0};", _dbName);
                    command.ExecuteNonQuery();
                }
            }


            using (var sqlConnection = new SqlConnection(ConnectionString))
            {
                sqlConnection.Open();

                using (var command = sqlConnection.CreateCommand())
                {
                    command.CommandText = GetCreateStatementsForTables();
                    command.ExecuteNonQuery();
                }
            }

            AddTblMemberRecords();
        }

        #region Extra baggage for scope checking

        /// <summary>
        ///To understand how hierachy works, see https://deluxe.atlassian.net/wiki/spaces/ITMS/pages/417399125/TestFixture+2+Base+objects+and+Samples
        /// </summary>
        /// <returns>Number of Records added</returns>
        private int AddTblMemberRecords()
        {
            const string query = @"SET IDENTITY_INSERT tblMember ON;
            INSERT INTO[tblMember]([MemberID], [ParentID], [Name], [CreateDate], [Type], [IsEnabled], [Level], [Hierarchy], [OwnerCode], [TransactionSource], [UpdateDate], [TSDisableDate], [Location], [SkipEmail])
            VALUES(1, NULL, 'RDMRoot', '2003-07-19', 0, 1, 0, '.1.', 'RDMRoot', NULL, '2016-07-05', NULL, '', 0),
            (2, 1, 'Customer1', '2003-07-19', 1, 1, 1, '.1.2.', 'RDM0000002', NULL, '2016-07-05', NULL, '', 0),
            (3, 2, 'Merchant1', '2003-07-19', 2, 1, 0, '.1.2.3.', 'RDM0000003', NULL, '2016-07-05', NULL, '', 0),
            (4, 3, 'Location1', '2003-07-19', 6, 1, 0, '.1.2.3.4.', 'RDM0000004', NULL, '2016-07-05', NULL, '', 0),
            (5, 4, 'Account1', '2003-07-19', 7, 1, 0, '.1.2.3.4.5.', 'RDM0000005', NULL, '2016-07-05', NULL, '', 0),
            (6, 4, 'Account2', '2003-07-19', 7, 1, 0, '.1.2.3.4.6.', 'RDM0000006', NULL, '2016-07-05', NULL, '', 0);";

            using (var sqlConnection = new SqlConnection(ConnectionString))
            {
                sqlConnection.Open();

                using (var command = sqlConnection.CreateCommand())
                {
                    command.CommandText = query;

                    return command.ExecuteNonQuery();
                }
            }
        }

        public int AddTblUserRecord(int userId, int? fMemberId = 5)
        {
            const string query =
                @"SET IDENTITY_INSERT tblUser ON; INSERT[dbo].[tblUser] ([UserID], [fMemberID], [Login], [FullName], [CreateDate], [IsEnabled],
                [IsSystem], [LogonAttempts], [IsLocked], [LockedTime], [Flag], [fUserClassID],
                [UserStdID], [CreationUserID], [LastLoginDate], [LastUpdateDate], [PreviousLoginDate], [EmailAddress], [EmailVerified])
                VALUES(@userID, @fMemberId, N'RDMOTest', N'RDMOTest', CAST(N'2003-07-19T10:06:03.000' AS DateTime),
                1, 1, 0, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0)";

            using (var sqlConnection = new SqlConnection(ConnectionString))
            {
                sqlConnection.Open();

                using (var command = sqlConnection.CreateCommand())
                {
                    command.CommandText = query;
                    command.Parameters.AddWithValue("@userID", userId);
                    command.Parameters.AddWithValue("@fMemberId", fMemberId);

                    return command.ExecuteNonQuery();
                }
            }
        }

        #endregion

        public int AddTblItemRecord(string irn)
        {
            const string query = @"
                INSERT INTO [dbo].[tblItem]
                (
                    [ItemID],
                    [IRN],
                    [SeqNum],
                    [fPayorID],
                    [fScannerID],
                    [fTransactionID],
                    [fFileID],
                    [CaptureDateTime],
                    [InsertTime],
                    [SecondaryID],
                    [MerchantNumber],
                    [StoreNumber],
                    [TerminalNumber],
                    [IndividualName],
                    [SECCode],
                    [KeyedCheckSerialNumber],
                    [ImageCount],
                    [fEndpointID],
                    [fMemberID],
                    [fItemTypeID],
                    [fPayeeID],
                    [PaymentDueDate],
                    [CashierName],
                    [ONUSRT],
                    [PaymentType],
                    [ACCOUNTTYPE],
                    [RECURRINGPAYMENT],
                    [IndividualID],
                    [fClientTypeID]
                )
                VALUES
                (
                    @itemID,       /* [ItemID],                  */
                    @irn,          /* [IRN],                     */
                    1,             /* [SeqNum],                  */
                    NULL,          /* [fPayorID],                */
                    NULL,          /* [fScannerID],              */
                    NULL,          /* [fTransactionID],          */
                    NULL,          /* [fFileID],                 */
                    getDate(),     /* [CaptureDateTime],         */
                    getDate(),     /* [InsertTime],              */
                    NULL,          /* [SecondaryID],             */
                    NULL,          /* [MerchantNumber],          */
                    NULL,          /* [StoreNumber],             */
                    NULL,          /* [TerminalNumber],          */
                    NULL,          /* [IndividualName],          */
                    NULL,          /* [SECCode],                 */
                    NULL,          /* [KeyedCheckSerialNumber],  */
                    NULL,          /* [ImageCount],              */
                    NULL,          /* [fEndpointID],             */
                    5,             /* [fMemberID],               */
                    1,             /* [fItemTypeID],             */
                    NULL,          /* [fPayeeID],                */
                    NULL,          /* [PaymentDueDate],          */
                    NULL,          /* [CashierName],             */
                    NULL,          /* [ONUSRT],                  */
                    NULL,          /* [PaymentType],             */
                    NULL,          /* [ACCOUNTTYPE],             */
                    NULL,          /* [RECURRINGPAYMENT],        */
                    NULL,          /* [IndividualID],            */
                    NULL           /* [fClientTypeID]            */
                );";

            using (var sqlConnection = new SqlConnection(ConnectionString))
            {
                sqlConnection.Open();

                var itemId = 1;
                using (var command = sqlConnection.CreateCommand())
                {
                    command.CommandText = "SELECT MAX(ItemID) AS ItemID FROM tblItem;";
                    var reader = command.ExecuteReader();

                    if (reader.Read() && !reader.IsDBNull(0))
                    {
                        itemId = (int)reader.GetInt64(0) + 1;
                    }
                }

                using (var command = sqlConnection.CreateCommand())
                {
                    command.CommandText = query;
                    command.Parameters.AddWithValue("@irn", irn);
                    command.Parameters.AddWithValue("@itemID", itemId);

                    command.ExecuteNonQuery();
                }

                return itemId;
            }
        }

        public void AddTblImageRecord(int itemId, int urlId, int fileId, string surface, int page, string filename)
        {
            const string query = @"
                INSERT INTO [dbo].[tblImage]
                (
                    [fItemID],
                    [fURLID],
                    [ImageFilename],
                    [ImageSize],
                    [Surface],
                    [Received],
                    [fContentTypeID],
                    [fDispositionTypeID],
                    [fFileID],
                    [Timestamp],
                    [SheetNumber],
                    [fDocSizeID],
                    [IMGESEQ]
                )
                VALUES
                (
                    @itemID,           /* [fItemID],            */
                    @urlID,            /* [fURLID],             */
                    @filename,         /* [ImageFilename],      */
                    NULL,              /* [ImageSize],          */
                    @surface,          /* [Surface],            */
                    NULL,              /* [Received],           */
                    NULL,              /* [fContentTypeID],     */
                    NULL,              /* [fDispositionTypeID], */
                    @fileID,           /* [fFileID],            */
                    getDate(),         /* [Timestamp],          */
                    @page,             /* [SheetNumber],        */
                    NULL,              /* [fDocSizeID],         */
                    NULL               /* [IMGESEQ]             */
                );";

            using (var sqlConnection = new SqlConnection(ConnectionString))
            {
                sqlConnection.Open();

                using (var command = sqlConnection.CreateCommand())
                {
                    command.CommandText = query;
                    command.Parameters.AddWithValue("@itemID", itemId);
                    command.Parameters.AddWithValue("@urlID", urlId);
                    command.Parameters.AddWithValue("@filename", filename);
                    command.Parameters.AddWithValue("@surface", surface);
                    command.Parameters.AddWithValue("@fileID", fileId);
                    command.Parameters.AddWithValue("@page", page);

                    command.ExecuteNonQuery();
                }
            }
        }

        public int AddTblUrlRecord(string url)
        {
            const string query = @"
                INSERT INTO [dbo].[tblURL]
                (
                    [URL]
                )
                VALUES
                (
                    @url
                );
                SELECT SCOPE_IDENTITY();";

            using (var sqlConnection = new SqlConnection(ConnectionString))
            {
                sqlConnection.Open();

                using (var command = sqlConnection.CreateCommand())
                {
                    command.CommandText = query;
                    command.Parameters.AddWithValue("@url", url);

                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }

        public int AddTblIFM_ArchiveFile(int urlId, string archiveFilename)
        {
            const string query = @"
                INSERT INTO [dbo].[tblIFM_ArchiveFile]
                (
                    [fURLID],
                    [ArchiveFileName],
                    [TotalFileCount],
                    [TotalImageCount],
                    [SizeInBytes],
                    [InsertTime],
                    [fIFM_TaskID]
                )
                VALUES
                (
                    @urlID,              /* [fURLID],          */
                    @archiveFilename,    /* [ArchiveFileName], */
                    NULL,                /* [TotalFileCount],  */
                    NULL,                /* [TotalImageCount], */
                    NULL,                /* [SizeInBytes],     */
                    NULL,                /* [InsertTime],      */
                    1                    /* [fIFM_TaskID]      */
                );
                SELECT SCOPE_IDENTITY();";

            using (var sqlConnection = new SqlConnection(ConnectionString))
            {
                sqlConnection.Open();

                using (var command = sqlConnection.CreateCommand())
                {
                    command.CommandText = query;
                    command.Parameters.AddWithValue("@urlId", urlId);
                    command.Parameters.AddWithValue("@archiveFilename", archiveFilename);

                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }

        public void AddTblIFM_ArchiveFiles(int archiveFileId, int fileId)
        {
            const string query = @"
                INSERT INTO [dbo].[tblIFM_ArchiveFiles]
                (
                    [fIFM_ArchiveFileID],
                    [fFileID],
                    [InsertTime],
                    [FinalReceiptTime]
                )
                VALUES
                (
                    @archiveFileID,   /* [fIFM_ArchiveFileID], */
                    @fileID,          /* [fFileID],            */
                    getDate(),        /* [InsertTime],         */
                    getDate()         /* [FinalReceiptTime]    */
                );";

            using (var sqlConnection = new SqlConnection(ConnectionString))
            {
                sqlConnection.Open();

                using (var command = sqlConnection.CreateCommand())
                {
                    command.CommandText = query;
                    command.Parameters.AddWithValue("@archiveFileID", archiveFileId);
                    command.Parameters.AddWithValue("@fileID", fileId);

                    command.ExecuteNonQuery();
                }
            }
        }

        public string ConnectionString { get; }

        private string GetCreateStatementsForTables()
        {
            var sb = new StringBuilder();

            sb.Append(ItmsImageRepositoryFixtureCreateTable.TblItem);
            sb.Append(ItmsImageRepositoryFixtureCreateTable.TblImage);
            sb.Append(ItmsImageRepositoryFixtureCreateTable.TblURL);
            sb.Append(ItmsImageRepositoryFixtureCreateTable.TblIfmArchiveFile);
            sb.Append(ItmsImageRepositoryFixtureCreateTable.TblIfmArchiveFiles);
            sb.Append(ItmsImageRepositoryFixtureCreateTable.TblMember);
            sb.Append(ItmsImageRepositoryFixtureCreateTable.TblUser);
            sb.Append(ItmsImageRepositoryFixtureCreateTable.TblUserAllowedAccounts);
            sb.Append(ItmsImageRepositoryFixtureCreateTable.TblUserRestrictAccounts);

            return sb.ToString();
        }

        public int TruncateAllTables()
        {
            using (var sqlConnection = new SqlConnection(ConnectionString))
            {
                sqlConnection.Open();

                using (var command = sqlConnection.CreateCommand())
                {
                    command.CommandText = "TRUNCATE TABLE [dbo].[tblImage];"
                        + "TRUNCATE TABLE [dbo].[tblItem]; "
                        + "TRUNCATE TABLE [dbo].[tblURL]; "
                        + "TRUNCATE TABLE [dbo].[tblIFM_ArchiveFile]; "
                        + "TRUNCATE TABLE [dbo].[tblIFM_ArchiveFiles]; "
                        + "TRUNCATE TABLE [dbo].[tblUser]; "
                        + "TRUNCATE TABLE [dbo].[tblUserAllowedAccounts];"
                        + "TRUNCATE TABLE [dbo].[tblUserRestrictAccounts]; ";

                    return command.ExecuteNonQuery();
                }
            }
        }

        public void Dispose()
        {
            using (
                var sqlConnection =
                    new SqlConnection(
                         $"data source={_dbSource};persist security info=True;user id={_dbUserid};password={_dbPassword};MultipleActiveResultSets=True")
           )
            {
                sqlConnection.Open();

                using (var command = sqlConnection.CreateCommand())
                {
                    command.CommandText = string.Format(
                        @"
                    ALTER DATABASE {0}
                    SET SINGLE_USER
                    WITH ROLLBACK IMMEDIATE;
                    DROP DATABASE {0};",
                        _dbName);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
