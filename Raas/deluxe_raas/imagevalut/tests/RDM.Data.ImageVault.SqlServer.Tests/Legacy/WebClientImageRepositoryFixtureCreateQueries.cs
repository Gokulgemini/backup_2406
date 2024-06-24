namespace RDM.Data.ImageVault.SqlServer.Tests.Legacy
{
    public static class WebClientImageRepositoryFixtureCreateQueries
    {
        public const string TblImage = @"CREATE TABLE [dbo].[tblImage_Base](
                [ImageID] [bigint] IDENTITY(1,1) NOT NULL,
                [fItemID] [int] NULL,
                [fURLID] [int] NULL,
                [ImageFilename] [varchar](255) NULL,
                [ImageSize] [int] NULL,
                [Surface] [varchar](15) NOT NULL,
                [Received] [bit] NULL,
                [fContentTypeID] [smallint] NULL,
                [fDispositionTypeID] [smallint] NULL,
                [Processed] [bit] NULL,
                [fDocSizeID] [int] NULL,
                [ImageWidth] [int] NULL,
                [ImageHeight] [int] NULL,
                [SheetNumber] [int] NULL,
                [PartKey] [datetime] NOT NULL,
                CONSTRAINT [tblImage_PK] PRIMARY KEY CLUSTERED
                ([ImageID] ASC,
                [PartKey] ASC)
                WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON));
                ALTER TABLE [dbo].[tblImage_Base] ADD  DEFAULT (getdate()) FOR [PartKey];";

        public const string TblItem = @"CREATE TABLE [dbo].[tblItem_Base](
                [ItemID] [int] IDENTITY(1,1) NOT NULL,
                [IRN] [varchar](25) NOT NULL,
                [SeqNum] [int] NOT NULL,
                [fTransactionID] [int] NULL,
                [SecondaryID] [varchar](40) NULL,
                [UserField1] [varchar](250) NULL,
                [UserField2] [varchar](250) NULL,
                [UserField3] [varchar](250) NULL,
                [UserField4] [varchar](250) NULL,
                [UserField5] [varchar](250) NULL,
                [UserField6] [varchar](250) NULL,
                [UserField7] [varchar](250) NULL,
                [UserField8] [varchar](250) NULL,
                [UserField9] [varchar](250) NULL,
                [UserField10] [varchar](250) NULL,
                [MerchantNumber] [varchar](30) NULL,
                [StoreNumber] [varchar](30) NULL,
                [TerminalNumber] [varchar](30) NULL,
                [CashierName] [varchar](30) NULL,
                [SECCode] [varchar](3) NULL,
                [ImageCount] [int] NULL,
                [fItemSubTypeID] [int] NOT NULL,
                [Inserttime] [datetime] NOT NULL,
                [fScannerID] [int] NULL,
                [fCheckTypeID] [int] NULL,
                [CaptureTime] [datetime] NULL,
                [fItemStatusID] [int] NULL,
                [IskeyComplete] [bit] NOT NULL,
                [fPayorID] [int] NULL,
                [LogicalSeq] [int] NULL,
                [fKeyDataSourceID] [int] NULL,
                [AmountDue] [money] NULL,
                [DiscountAmountDue] [money] NULL,
                [MinimumAmountDue] [money] NULL,
                [LateAmountDue] [money] NULL,
                [AmountPaid] [money] NULL,
                [KeyDataSources] [varchar](400) NULL,
                [AutoCompleteOverride] [bit] NULL,
                [fItemSourceId] [int] NULL,
                [UserID] [int] NULL,
                [CLI] [int] NULL,
                [IndividualID] [varchar](15) NULL,
                [AccountType] [char](1) NULL,
                [RecurringPayment] [bit] NULL,
                [fClientTypeID] [int] NULL,
                [fClientApplicationVersionID] [int] NULL,
                CONSTRAINT [tblItem_PK] PRIMARY KEY CLUSTERED
                (
                    [ItemID] ASC,
                    [Inserttime] ASC
                )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON));
                ALTER TABLE [dbo].[tblItem_Base] ADD  CONSTRAINT [DF_tblItem_CaptureTime]  DEFAULT (getdate()) FOR [CaptureTime];
                ALTER TABLE [dbo].[tblItem_Base] ADD  CONSTRAINT [DF_tblItem_fItemStatusID]  DEFAULT ((4)) FOR [fItemStatusID];
                ALTER TABLE [dbo].[tblItem_Base] ADD  CONSTRAINT [DF_tblItem_IskeyComplete]  DEFAULT ((0)) FOR [IskeyComplete];";

        public const string TblUrl = @"CREATE TABLE[dbo].[tblURL]
            ([URLID] [int] IDENTITY(1,1) NOT NULL,
            [URL] [varchar](255) NOT NULL,
            CONSTRAINT[PK_tblURL] PRIMARY KEY CLUSTERED([URLID] ASC)
            WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON));";

        public const string TblMember = @"CREATE TABLE [dbo].[tblMember_Base]
            ([MemberID] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
            [ParentID] [int] NULL,
            [Name] [varchar](100) NULL,
            [CreateDate] [datetime] NULL,
            [Type] [int] NOT NULL,
            [IsEnabled] [bit] NOT NULL,
            [Level] [int] NULL,
            [Hierarchy] [varchar](900) NULL,
            [OwnerCode] [varchar](17) NULL,
            [TransactionSource] [bit] NULL,
            [UpdateDate] [datetime] NOT NULL,
            [TSDisableDate] [datetime] NULL,
            [IsDeleted] [bit] NOT NULL,
            [SkipEmail] [bit] NOT NULL,
            [MakerCheckerUserID] [int] NULL,
            [Approved] [bit] NOT NULL,
            CONSTRAINT [MemberIDIndex_100] PRIMARY KEY CLUSTERED([MemberID] ASC)
            WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = OFF));ALTER TABLE [dbo].[tblMember_Base] ADD  CONSTRAINT [DF__tblMember__SkipE__5007AB49]  DEFAULT ((0)) FOR [SkipEmail];";

        public const string TblApplication = @"CREATE TABLE[dbo].[tblApplication]
            ([ApplicationID][int] NOT NULL,
            [ApplicationName] [varchar] (50) NOT NULL,
            [ApplicationVersion] [varchar] (50) NULL,
            CONSTRAINT[PK_tblApplication] PRIMARY KEY CLUSTERED([ApplicationID] ASC)
            WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON[PRIMARY]) ON[PRIMARY];";

        public const string TblModule = @"CREATE TABLE [dbo].[tblModule](
            [ModuleID] [int] NOT NULL,
            [ModuleName] [varchar](20) NOT NULL,
            [ModuleVersion] [varchar](50) NULL,
            [fApplicationID] [int] NULL,
            CONSTRAINT [tblModule_PK] PRIMARY KEY CLUSTERED([ModuleID] ASC)
            WITH (PAD_INDEX = ON, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = OFF, FILLFACTOR = 80));
            ALTER TABLE [dbo].[tblModule]  WITH CHECK ADD  CONSTRAINT [tblApplication_tblModule_FK1] FOREIGN KEY([fApplicationID])
            REFERENCES [dbo].[tblApplication] ([ApplicationID]);ALTER TABLE [dbo].[tblModule] CHECK CONSTRAINT [tblApplication_tblModule_FK1];";

        public const string TblUser = @"CREATE TABLE [dbo].[tblUser](
            [UserID] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
            [fMemberID] [int] NOT NULL,
            [Login] [varchar](20) NOT NULL,
            [FullName] [varchar](250) NULL,
            [CreateDate] [datetime] NULL,
            [IsEnabled] [bit] NOT NULL,
            [IsSystem] [bit] NOT NULL,
            [LogonAttempts] [int] NOT NULL,
            [IsLocked] [bit] NOT NULL,
            [LockedTime] [datetime] NULL,
            [Flag] [bit] NULL,
            [fUserClassID] [int] NULL,
            [UserStdID] [varchar](15) NULL,
            [CreationUserID] [int] NULL,
            [LastLoginDate] [datetime] NULL,
            [LastUpdateDate] [datetime] NULL,
            [PreviousLoginDate] [datetime] NULL,
            [EmailAddress] [varchar](100) NULL,
            [EmailVerified] [bit] NOT NULL,
             CONSTRAINT [PK_tblUser] PRIMARY KEY CLUSTERED([UserID] ASC)
            WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON));
            ALTER TABLE [dbo].[tblUser] ADD  CONSTRAINT [tblUser_EmailVerified_Default]  DEFAULT ((0)) FOR [EmailVerified];";

        public const string TblUserAllowedAccounts = @"CREATE TABLE [dbo].[tblUserAllowedAccounts](
            [fUserID] [int] NOT NULL,
            [AllowedAccountMemberID] [int] NOT NULL,
            CONSTRAINT [PK_UserAllowedAccounts] PRIMARY KEY CLUSTERED
            ([fUserID] ASC,
            [AllowedAccountMemberID] ASC)
            WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON));";

        public const string TblUserRestrictAccounts = @"CREATE TABLE [dbo].[tblUserRestrictAccounts](
            [fUserID] [int] NOT NULL,
            [RestrictAccounts] [bit] NULL,
            CONSTRAINT [PK_UserRestrictAccounts] PRIMARY KEY CLUSTERED([fUserID] ASC)
            WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON));";

        public const string TblTransaction = @"CREATE TABLE [dbo].[tblTransaction_Base](
                [TransactionID] [int] IDENTITY(1,1) NOT NULL,
                [fBatchID] [int] NULL,
                [TRN] [varchar](25) NOT NULL,
                [TransactionStatus] [varchar](30) NOT NULL,
                [ExceptionOverride] [bit] NOT NULL,
                [IsBalanceOverride] [bit] NULL,
                [IsAmountsApplied] [bit] NULL,
                [PartKey] [datetime] NOT NULL,
                CONSTRAINT [tblTransaction_PK] PRIMARY KEY CLUSTERED
                (
                    [TransactionID] ASC,
                    [PartKey] ASC)
                WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON));
                ALTER TABLE [dbo].[tblTransaction_Base] ADD  CONSTRAINT [DF_tblTransaction_ExceptionOverride]  DEFAULT ((0)) FOR [ExceptionOverride];
                ALTER TABLE [dbo].[tblTransaction_Base] ADD  DEFAULT (getdate()) FOR [PartKey];";

        public const string TblBatch = @"CREATE TABLE [dbo].[tblBatch_Base](
                [BatchID] [int] IDENTITY(1,1) NOT NULL,
                [fDepositID] [int] NULL,
                [BRN] [uniqueidentifier] NOT NULL,
                [BCN] [varchar](30) NULL,
                [BCT] [money] NULL,
                [ItemCount] [int] NOT NULL,
                [BatchCloseDateTime] [datetime] NULL,
                [BatchOpenDateTime] [datetime] NOT NULL,
                [ProcessingMode] [char](3) NULL,
                [OwnerCode] [varchar](17) NULL,
                [IsActive] [bit] NULL,
                [ClientGUID] [uniqueidentifier] NULL,
                [fMemberCentralizedConfigID] [int] NULL,
                [DSN] [varchar](10) NULL,
                [fCreationCodeID] [int] NULL,
                [ForwardPresent] [int] NULL,
                [fTransactionProfileID] [int] NULL,
                [IsRemittanceBalancingRequired] [bit] NOT NULL,
                [AutoClose] [bit] NULL,
                [fScanningModeID] [int] NULL,
                [IsMultiItemMode] [bit] NULL,
                [ApprovedUserID] [int] NULL,
                 CONSTRAINT [tblBatch_PK] PRIMARY KEY CLUSTERED
                (
                    [BatchID] ASC,
                    [BatchOpenDateTime] ASC
                )
                WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = ON, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80));
                ALTER TABLE [dbo].[tblBatch_Base] ADD  CONSTRAINT [DF_tblBatch_BRN]  DEFAULT (newid()) FOR [BRN];
                ALTER TABLE [dbo].[tblBatch_Base] ADD  CONSTRAINT [DF_tblBatch_BCT]  DEFAULT ((0)) FOR [BCT];
                ALTER TABLE [dbo].[tblBatch_Base] ADD  CONSTRAINT [DF_tblBatch_ItemCount]  DEFAULT ((0)) FOR [ItemCount];
                ALTER TABLE [dbo].[tblBatch_Base] ADD  CONSTRAINT [DF_tblBatch_IsActive]  DEFAULT ((0)) FOR [IsActive];
                ALTER TABLE [dbo].[tblBatch_Base] ADD  CONSTRAINT [DF_tblBatch_ForwardPresent]  DEFAULT ((1)) FOR [ForwardPresent];
                ALTER TABLE [dbo].[tblBatch_Base] ADD  CONSTRAINT [DF_tblBatch_IsRemittanceBalancingRequired]  DEFAULT ((0)) FOR [IsRemittanceBalancingRequired];
                ALTER TABLE [dbo].[tblBatch_Base] ADD  CONSTRAINT [DF_tblBatch_AutoClose]  DEFAULT ((0)) FOR [AutoClose];";

        public const string TblUow = @"CREATE TABLE [dbo].[tblUOW](
                [UOWID] [int] IDENTITY(1,1) NOT NULL,
                [fModuleID] [int] NOT NULL,
                [fUOWStatusID] [int] NULL,
                [ModuleStatusID] [int] NULL,
                [ModuleReturnCode] [varchar](20) NULL,
                [ItemCount] [int] NULL,
                [DollarAmount] [money] NULL,
                [fBatchID] [int] NULL,
                [fApplicationID] [int] NULL,
                [UserID] [int] NULL,
                [PartKey] [datetime] NOT NULL,
                 CONSTRAINT [tblUOW_PK] PRIMARY KEY CLUSTERED
                (
                    [UOWID] ASC,
                    [PartKey] ASC
                )
                WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = ON, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80)
                );
                ALTER TABLE [dbo].[tblUOW] ADD  DEFAULT (getdate()) FOR [PartKey];";

        public const string FnGetMemberIDs_ASC = @"CREATE FUNCTION  [dbo].[fnGetMemberIDs_ASC] (@MemberID Int )
                RETURNS @MembersUP TABLE ( MemberID Int, Level SMALLINT IDENTITY(0,1), PRIMARY KEY CLUSTERED (MemberID, LEVEL ))
                AS
                BEGIN

                        DECLARE  @MembersXML XML;
                        --DECLARE  @MembersUP TABLE (Level INT IDENTITY(0,1), MemberID SMALLINT, PRIMARY KEY (MemberID));
                        DECLARE  @MemberHierarchy VARCHAR(8000);
                        SELECT @MemberHierarchy = Hierarchy
                        FROM   tblMember_Base 
                        WHERE  MemberID = @MemberID
                            AND  IsDeleted = 0 ;

                        SET @MembersXML= CONVERT(xml,'<root><s>' + REPLACE(@MemberHierarchy,'.','</s><s>') + '</s></root>')

                        ;WITH x AS
                            (SELECT T.c.value('.','INT') AS MemberID
                                FROM @MembersXML.nodes('/root/s') T(c) )
                        INSERT INTO @MembersUP (MemberID)
                        SELECT MemberID
                        FROM x
                        WHERE MemberID !=0;

                  RETURN
                END;";
    }
}
