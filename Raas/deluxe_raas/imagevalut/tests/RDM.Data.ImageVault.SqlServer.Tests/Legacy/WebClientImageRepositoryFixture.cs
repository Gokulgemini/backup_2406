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
    public class WebClientImageRepositoryFixture : IDisposable
    {
        private readonly string _dbName;
        private readonly string _dbSource = @"DWA00120.deluxe.com\DD01";
        private readonly string _dbUserid = @"unittest";
        private readonly string _dbPassword = @"Z5uRXkOWclQ3dj5ixziL";

        public WebClientImageRepositoryFixture()
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
                    command.CommandText = $"CREATE DATABASE {_dbName};";
                    command.ExecuteNonQuery();
                }
            }


            using (var sqlConnection = new SqlConnection(ConnectionString))
            {
                sqlConnection.Open();

                // Create Tables
                using (var command = sqlConnection.CreateCommand())
                {
                    command.CommandText = GetCreateStatementsForTables();
                    command.ExecuteNonQuery();
                }

                // Create Functions
                using (var command = sqlConnection.CreateCommand())
                {
                    command.CommandText = WebClientImageRepositoryFixtureCreateQueries.FnGetMemberIDs_ASC;
                    command.ExecuteNonQuery();
                }
            }

            PopulateTableTblApplication();
            PopulateTableTblModule();

            AddTblMemberRecords();
            AddTblTransactionRecord();
            AddTblBatchRecord();
            AddTbluowRecord();
        }

        public int AddTblItemRecord(string irn)
        {
            const string query =
                @"SET IDENTITY_INSERT tblItem_Base ON;INSERT [dbo].[tblItem_Base] ([ItemID],[IRN], [SeqNum], [fTransactionID], [SecondaryID],
            [UserField1], [UserField2], [UserField3], [UserField4], [UserField5],
            [UserField6], [UserField7], [UserField8], [UserField9], [UserField10], [MerchantNumber], [StoreNumber], [TerminalNumber], [CashierName],
            [SECCode], [ImageCount], [fItemSubTypeID], [Inserttime],
            [fScannerID], [fCheckTypeID], [CaptureTime], [fItemStatusID], [IskeyComplete], [fPayorID], [LogicalSeq],
            [fKeyDataSourceID], [AmountDue], [DiscountAmountDue], [MinimumAmountDue],
            [LateAmountDue], [AmountPaid], [KeyDataSources], [AutoCompleteOverride], [fItemSourceId], [UserID], [CLI],
            [IndividualID], [AccountType], [RecurringPayment], [fClientTypeID], [fClientApplicationVersionID])
            VALUES (1, @irn, 1, 27102621, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL,
            N'promote20', N'BOC', 2, 1, getDate(), 11508, NULL, CAST(N'2015-02-09T13:59:09.000' AS DateTime),
            1, 1, 27385002, 1, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL,  NULL, 10, NULL, NULL, NULL, NULL, NULL);";

            using (var sqlConnection = new SqlConnection(ConnectionString))
            {
                sqlConnection.Open();

                using (var command = sqlConnection.CreateCommand())
                {
                    command.CommandText = query;
                    command.Parameters.AddWithValue("@irn", irn);

                    return command.ExecuteNonQuery();
                }
            }
        }

        public int AddTblImageRecord(string surface, int pageNumber, string fileName = "70474911-e2de-4eed-9202-d53498f7e0d8.tiff")
        {
            const string query =
                @"SET IDENTITY_INSERT tblImage_Base ON;INSERT[dbo].[tblImage_Base]([ImageID], [fItemID], [fURLID], [ImageFilename], [ImageSize], [Surface],
            [Received], [fContentTypeID], [fDispositionTypeID], [Processed], [fDocSizeID], [ImageWidth], [ImageHeight], [SheetNumber], [PartKey])
            VALUES(54716536, 1, 6154, @fileName, 5140, @surface, NULL, 1, 3, NULL, 2, 6235, 2775, @pageNumber, getDate());";

            using (var sqlConnection = new SqlConnection(ConnectionString))
            {
                sqlConnection.Open();

                using (var command = sqlConnection.CreateCommand())
                {
                    command.CommandText = query;
                    command.Parameters.AddWithValue("@surface", surface);
                    command.Parameters.AddWithValue("@pageNumber", pageNumber);
                    command.Parameters.AddWithValue("@fileName", fileName);

                    return command.ExecuteNonQuery();
                }
            }
        }

        public int AddTblUrlRecord()
        {
            const string query = @"SET IDENTITY_INSERT tblURL ON;INSERT[dbo].[tblURL]([URLID], [URL])
            VALUES(6154, N'\\DITWCVFS01\WCStoredItems\2015\02\11\000000');";

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

        #region Extra baggage for scope checking

        private void PopulateTableTblModule()
        {
            using (var sqlConnection = new SqlConnection(ConnectionString))
            {
                sqlConnection.Open();

                using (var command = sqlConnection.CreateCommand())
                {
                    command.CommandText = @"INSERT [dbo].[tblModule] ([ModuleID], [ModuleName], [ModuleVersion], [fApplicationID])
                        VALUES (1, N'SCAN', N'1.0', 1),
                        (2, N'KEY', N'1.0', 1),
                        (3, N'BALANCE', N'1.0', 1),
                        (4, N'APPROVE', N'1.0', 1),
                        (5, N'TRANSFER', N'1.0', 1),
                        (6, N'ACKNOWLEDGE', N'1.0', 1),
                        (7, N'SCAN', N'1.1', 2),
                        (8, N'KEY', N'1.1', 2),
                        (9, N'BALANCE', N'1.1', 2),
                        (10, N'APPROVE', N'1.1', 2),
                        (11, N'TRANSFER', N'1.1', 2),
                        (12, N'ACKNOWLEDGE', N'1.1', 2),
                        (13, N'SCAN', N'1.2', 3),
                        (14, N'KEY', N'1.2', 3),
                        (15, N'BALANCE', N'1.2', 3),
                        (16, N'APPROVE', N'1.2', 3),
                        (17, N'TRANSFER', N'1.2', 3),
                        (18, N'ACKNOWLEDGE', N'1.2', 3),
                        (20, N'APPROVESUSPENSE', N'1.2', 3),
                        (21, N'SCAN', N'2.0', 4),
                        (22, N'KEY', N'2.0', 4),
                        (23, N'BALANCE', N'2.0', 4),
                        (24, N'APPROVE', N'2.0', 4),
                        (25, N'TRANSFER', N'2.0', 4),
                        (26, N'ACKNOWLEDGE', N'2.0', 4),
                        (27, N'APPROVESUSPENSE', N'2.0', 4),
                        (28, N'SCAN', N'1.4', 5),
                        (29, N'KEY', N'1.4', 5),
                        (30, N'BALANCE', N'1.4', 5),
                        (31, N'APPROVE', N'1.4', 5),
                        (32, N'TRANSFER', N'1.4', 5),
                        (33, N'ACKNOWLEDGE', N'1.4', 5),
                        (34, N'AUTHORIZE', N'1.3', 4),
                        (35, N'PREDEPOSITPROCESSING', N'1.3', 4),
                        (36, N'PREDEPOSITPROCESSING', N'1.4', 5),
                        (37, N'AUTOAPPROVE', N'1.3', 4);";
                    command.ExecuteNonQuery();
                }
            }
        }

        private void PopulateTableTblApplication()
        {
            using (var sqlConnection = new SqlConnection(ConnectionString))
            {
                sqlConnection.Open();

                using (var command = sqlConnection.CreateCommand())
                {
                    command.CommandText = @"INSERT [dbo].[tblApplication] ([ApplicationID],[ApplicationName],[ApplicationVersion])
                        VALUES (1, N'WebClient', N'1.0.0.6'),
                        (2, N'WebClient', N'1.1.0.0'),
                        (3, N'WebClient', N'1.2.0.0'),
                        (4, N'WebClient', N'2.0.0.0'),
                        (5, N'SimplyDeposit', N'1.1.0.0');";
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        ///To understand how hierachy works, see https://deluxe.atlassian.net/wiki/spaces/ITMS/pages/417399125/TestFixture+2+Base+objects+and+Samples
        /// </summary>
        /// <returns>Number of Records added</returns>
        private void AddTblMemberRecords()
        {
            const string query = @"SET IDENTITY_INSERT tblMember_Base ON;
            INSERT INTO[tblMember_Base]([MemberID], [ParentID], [Name], [CreateDate], [Type], [IsEnabled], [Level], [Hierarchy], [OwnerCode], [TransactionSource], [UpdateDate], [TSDisableDate], [IsDeleted], [SkipEmail], [Approved])
            VALUES(1, NULL, 'RDMRoot', '2003-07-19', 0, 1, 0, '.1.', 'RDMRoot', NULL, '2016-07-05', NULL, 0, 0, 1),
            (2, 1, 'Customer1', '2003-07-19', 1, 1, 1, '.1.2.', 'RDM0000002', NULL, '2016-07-05', NULL, 0, 0, 1),
            (3, 2, 'Merchant1', '2003-07-19', 2, 1, 0, '.1.2.3.', 'RDM0000003', NULL, '2016-07-05', NULL, 0, 0, 1),
            (4, 3, 'Location1', '2003-07-19', 6, 1, 0, '.1.2.3.4.', 'RDM0000004', NULL, '2016-07-05', NULL, 0, 0, 1),
            (5, 4, 'Account1', '2003-07-19', 7, 1, 0, '.1.2.3.4.5.', 'RDM0000005', NULL, '2016-07-05', NULL, 0, 0, 1),
            (6, 4, 'Account2', '2003-07-19', 7, 1, 0, '.1.2.3.4.6.', 'RDM0000006', NULL, '2016-07-05', NULL, 0, 0, 1);";

            using (var sqlConnection = new SqlConnection(ConnectionString))
            {
                sqlConnection.Open();

                using (var command = sqlConnection.CreateCommand())
                {
                    command.CommandText = query;
                    command.ExecuteNonQuery();
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

        private void AddTblTransactionRecord(int batchId = 2198367)
        {
            const string query = @"SET IDENTITY_INSERT tblTransaction_Base ON; INSERT[dbo].[tblTransaction_Base] ([TransactionID],
            [fBatchID], [TRN], [TransactionStatus],
            [ExceptionOverride], [IsBalanceOverride], [IsAmountsApplied], [PartKey])
            VALUES(27102621, @batchId, N'65IVTWMXK2G60DPP7XKYBLV6R', N'PENDINGCOMMIT', 0, NULL, NULL, getDate())";

            using (var sqlConnection = new SqlConnection(ConnectionString))
            {
                sqlConnection.Open();

                using (var command = sqlConnection.CreateCommand())
                {
                    command.CommandText = query;
                    command.Parameters.AddWithValue("@batchId", batchId);
                    command.ExecuteNonQuery();
                }
            }
        }

        public int AddTblBatchRecord(
            string brn = "EDD1A00E-49A0-4A4C-873C-A2753B9DEF6E",
            bool isActive = true,
            string ownerCode = "RDM0000005",
            int batchId = 2198367)
        {
            var isActiveBit = isActive ? 1 : 0;

            const string query =
                @"SET IDENTITY_INSERT tblBatch_Base ON; INSERT[dbo].[tblBatch_Base] ([BatchID], [fDepositID], [BRN], [BCN], [BCT],
            [ItemCount], [BatchCloseDateTime],
            [BatchOpenDateTime], [ProcessingMode], [OwnerCode], [IsActive], [ClientGUID],
            [fMemberCentralizedConfigID], [DSN], [fCreationCodeID], [ForwardPresent], [fTransactionProfileID],
            [IsRemittanceBalancingRequired], [AutoClose], [fScanningModeID], [IsMultiItemMode], [ApprovedUserID])
            VALUES(@batchId, NULL, @brn, NULL, 100.0000, 0, NULL,
            getDate(), N'BOC', @ownerCode, @isActive,
            N'4f424a3f-002c-479b-8f5d-2909079324e1', 69171, NULL, NULL, 1, 1, 0, 0, 2, 0, NULL)";

            using (var sqlConnection = new SqlConnection(ConnectionString))
            {
                sqlConnection.Open();

                using (var command = sqlConnection.CreateCommand())
                {
                    command.CommandText = query;
                    command.Parameters.AddWithValue("@brn", brn);
                    command.Parameters.AddWithValue("@isActive", isActiveBit);
                    command.Parameters.AddWithValue("@ownerCode", ownerCode);
                    command.Parameters.AddWithValue("@batchId", batchId);

                    return command.ExecuteNonQuery();
                }
            }
        }

        public int AddTbluowRecord(int fModuleId = 21, int batchId = 2198367)
        {
            const string query = @"SET IDENTITY_INSERT tblUOW ON; INSERT[dbo].[tblUOW] ([UOWID], [fModuleID],
            [fUOWStatusID], [ModuleStatusID], [ModuleReturnCode],
            [ItemCount], [DollarAmount], [fBatchID], [fApplicationID], [UserID], [PartKey])
            VALUES(2199091, @fModuleId, 2, NULL, NULL, 0, 0.0000, @batchId, 4, 54656, getDate())";

            using (var sqlConnection = new SqlConnection(ConnectionString))
            {
                sqlConnection.Open();

                using (var command = sqlConnection.CreateCommand())
                {
                    command.CommandText = query;
                    command.Parameters.AddWithValue("@fModuleId", fModuleId);
                    command.Parameters.AddWithValue("@batchId", batchId);

                    return command.ExecuteNonQuery();
                }
            }
        }

        #endregion

        public string ConnectionString { get; }

        private string GetCreateStatementsForTables()
        {
            var sb = new StringBuilder();

            sb.Append(WebClientImageRepositoryFixtureCreateQueries.TblItem);
            sb.Append(WebClientImageRepositoryFixtureCreateQueries.TblUrl);
            sb.Append(WebClientImageRepositoryFixtureCreateQueries.TblImage);
            sb.Append(WebClientImageRepositoryFixtureCreateQueries.TblMember);
            sb.Append(WebClientImageRepositoryFixtureCreateQueries.TblUser);
            sb.Append(WebClientImageRepositoryFixtureCreateQueries.TblUserAllowedAccounts);
            sb.Append(WebClientImageRepositoryFixtureCreateQueries.TblUserRestrictAccounts);
            sb.Append(WebClientImageRepositoryFixtureCreateQueries.TblApplication);
            sb.Append(WebClientImageRepositoryFixtureCreateQueries.TblModule);
            sb.Append(WebClientImageRepositoryFixtureCreateQueries.TblTransaction);
            sb.Append(WebClientImageRepositoryFixtureCreateQueries.TblBatch);
            sb.Append(WebClientImageRepositoryFixtureCreateQueries.TblUow);

            return sb.ToString();
        }

        public int TruncateAllTables()
        {
            using (var sqlConnection = new SqlConnection(ConnectionString))
            {
                sqlConnection.Open();

                using (var command = sqlConnection.CreateCommand())
                {
                    command.CommandText = "TRUNCATE TABLE [dbo].[tblImage_Base];"
                        + "TRUNCATE TABLE [dbo].[tblItem_Base]; "
                        + "TRUNCATE TABLE [dbo].[tblURL];"
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
