namespace RDM.Data.ImageVault.SqlServer.Tests.Legacy
{
    public static class ItmsImageRepositoryFixtureCreateTable
    {
        public const string TblImage = @"
            CREATE TABLE [dbo].[tblImage]
            (
                [ImageID] [bigint] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
                [fItemID] [bigint] NOT NULL,
                [fURLID] [int] NULL,
                [ImageFilename] [varchar](255) NULL,
                [ImageSize] [int] NULL,
                [Surface] [varchar](15) NOT NULL,
                [Received] [bit] NULL,
                [fContentTypeID] [int] NULL,
                [fDispositionTypeID] [int] NULL,
                [fFileID] [int] NULL,
                [Timestamp] [datetime] NOT NULL,
                [SheetNumber] [smallint] NULL,
                [fDocSizeID] [int] NULL,
                [IMGESEQ] [int] NULL,
                CONSTRAINT [PK_tblImage_18223] PRIMARY KEY CLUSTERED
                (
                    [ImageID] ASC,
                    [Timestamp] ASC
                )
                WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
            );";

        public const string TblItem = @"
            CREATE TABLE [dbo].[tblItem]
            (
                [ItemID] [bigint] NOT NULL,
                [IRN] [varchar](25) NOT NULL,
                [SeqNum] [int] NOT NULL,
                [fPayorID] [int] NULL,
                [fScannerID] [int] NULL,
                [fTransactionID] [bigint] NULL,
                [fFileID] [int] NULL,
                [CaptureDateTime] [datetime] NOT NULL,
                [InsertTime] [datetime] NOT NULL,
                [SecondaryID] [varchar](40) NULL,
                [MerchantNumber] [varchar](30) NULL,
                [StoreNumber] [varchar](30) NULL,
                [TerminalNumber] [varchar](30) NULL,
                [IndividualName] [varchar](22) NULL,
                [SECCode] [varchar](3) NULL,
                [KeyedCheckSerialNumber] [varchar](30) NULL,
                [ImageCount] [int] NULL,
                [fEndpointID] [int] NULL,
                [fMemberID] [int] NOT NULL,
                [fItemTypeID] [int] NOT NULL,
                [fPayeeID] [int] NULL,
                [PaymentDueDate] [datetime] NULL,
                [CashierName] [varchar](30) NULL,
                [ONUSRT] [bit] NULL,
                [PaymentType] [bit] NULL,
                [ACCOUNTTYPE] [varchar](1) NULL,
                [RECURRINGPAYMENT] [bit] NULL,
                [IndividualID] [varchar](15) NULL,
                [fClientTypeID] [int] NULL,
                CONSTRAINT [tblItem_PK] PRIMARY KEY CLUSTERED
                (
                    [ItemID] ASC,
                    [InsertTime] ASC
                )
                WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
            );";

        public const string TblURL = @"
            CREATE TABLE [dbo].[tblURL]
            (
                [URLID] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
                [URL] [varchar](255) NOT NULL,
                CONSTRAINT [aaaaatblURL_PK_100] PRIMARY KEY CLUSTERED
                (
                    [URLID] ASC
                )
                WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
            );";

        public const string TblIfmArchiveFiles = @"
            CREATE TABLE [dbo].[tblIFM_ArchiveFiles]
            (
                [fIFM_ArchiveFileID] [int] NOT NULL,
                [fFileID] [int] NOT NULL,
                [InsertTime] [datetime] NOT NULL,
                [FinalReceiptTime] [datetime] NOT NULL,
                CONSTRAINT [PK_tblIFM_ArchiveFiles] PRIMARY KEY CLUSTERED 
                (
                    [FinalReceiptTime] ASC,
                    [fIFM_ArchiveFileID] ASC,
                    [fFileID] ASC
                )
                WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
            );";

        public const string TblIfmArchiveFile = @"
            CREATE TABLE [dbo].[tblIFM_ArchiveFile]
            (
                [IFM_ArchiveFileID] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
                [fURLID] [int] NOT NULL,
                [ArchiveFileName] [varchar](255) NOT NULL,
                [TotalFileCount] [int] NULL,
                [TotalImageCount] [int] NULL,
                [SizeInBytes] [int] NULL,
                [InsertTime] [datetime] NULL,
                [fIFM_TaskID] [int] NOT NULL,
                CONSTRAINT [PK_tblIFM_ArchiveFile] PRIMARY KEY CLUSTERED
                (
                    [IFM_ArchiveFileID] ASC
                )
                WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
            );";

        public const string TblMember = @"CREATE TABLE [dbo].[tblMember]
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
            [Location] [varchar](100) NULL,
            [SkipEmail] [bit] NOT NULL,
            CONSTRAINT [MemberIDIndex_100] PRIMARY KEY CLUSTERED([MemberID] ASC)
            WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = OFF));ALTER TABLE [dbo].[tblMember] ADD  CONSTRAINT [DF__tblMember__SkipE__5007AB49]  DEFAULT ((0)) FOR [SkipEmail];";

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
    }
}
