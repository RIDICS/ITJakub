USE ITJakubDB

ALTER DATABASE ITJakubDB
SET ALLOW_SNAPSHOT_ISOLATION ON

ALTER DATABASE ITJakubDB
SET READ_COMMITTED_SNAPSHOT ON WITH ROLLBACK IMMEDIATE



SET XACT_ABORT ON
BEGIN TRAN


--create table for storing database version
	CREATE TABLE [dbo].[DatabaseVersion] 
	(
		[Id] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_DatabaseVersion(Id)] PRIMARY KEY CLUSTERED,
		[DatabaseVersion] varchar(50) NOT NULL,
		[SolutionVersion] varchar(50) NULL,
		[UpgradeDate] [datetime] NOT NULL DEFAULT GETDATE(),
		[UpgradeUser] varchar(150) NOT NULL default SYSTEM_USER,		
	)

    CREATE TABLE [User]
    (
	   [Id] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_User(Id)] PRIMARY KEY CLUSTERED,
	   [FirstName] [varchar] (50) NOT NULL,
	   [LastName] [varchar] (50) NOT NULL,
	   [Email] [varchar] (255) NOT NULL,
	   [AuthenticationProvider] [tinyint] NOT NULL,
	   [AuthenticationProviderToken] [varchar] (255) NULL,
	   [CommunicationToken] [varchar] (255) NULL,
	   [CommunicationTokenCreateTime] [datetime] NULL,    
	   [PasswordHash] [varchar] (255) NULL,
	   [Salt] [varchar] (50)  NULL,	   
	   [CreateTime] [datetime] NOT NULL,
	   [AvatarUrl] [varchar] (255) NULL,
	   CONSTRAINT [Uniq_Email_AuthProvider] UNIQUE ([Email],[AuthenticationProvider])    
    )



    CREATE TABLE [Author]
    (
	   [Id] [int] IDENTITY(1,1) CONSTRAINT [PK_Author(Id)] PRIMARY KEY CLUSTERED
    )

    CREATE TABLE [AuthorInfo]
    (
	   [Id] [int] IDENTITY(1,1) CONSTRAINT [PK_AuthorInfo(Id)] PRIMARY KEY CLUSTERED,
	   [Author] int NOT NULL CONSTRAINT [FK_AuthorInfo(Author)_Author(Id)] FOREIGN KEY REFERENCES [Author] (Id),
	   [Text] varchar(50) NOT NULL,
	   [TextType] int NOT NULL,
    )


    CREATE TABLE [Category]
    (
       [Id] [int] IDENTITY(1,1) CONSTRAINT [PK_Category(Id)] PRIMARY KEY CLUSTERED,
	  [Name] varchar(150) NOT NULL,
	  [ParrentCategory] int NULL CONSTRAINT [FK_Category(ParrentCategory)_Category(Id)] FOREIGN KEY REFERENCES [Category] (Id)
    )


    CREATE TABLE [Book]
    (
	   [Id] [bigint] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_Book(Id)] PRIMARY KEY CLUSTERED,
	   [Guid] [varchar] (50) NOT NULL,
	   [Author] int NULL CONSTRAINT [FK_Book(Author)_Author(Id)] FOREIGN KEY REFERENCES [Author] (Id),
	   [Name] varchar (MAX) NULL,
	   [Category] int NULL CONSTRAINT [FK_Book(Category)_Category(Id)] FOREIGN KEY REFERENCES [dbo].[Category](Id)
    )

    CREATE TABLE [Bookmark]
    (
	   [Id] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_Bookmark(Id)] PRIMARY KEY CLUSTERED,
	   [Book] bigint NOT NULL CONSTRAINT [FK_Bookmark(Book)_Book(Id)] FOREIGN KEY REFERENCES [dbo].[Book](Id),
	   [Page] varchar(50) NOT NULL
    )




    INSERT INTO [dbo].[DatabaseVersion]
		(DatabaseVersion)
	VALUES
		('000' )
		-- DatabaseVersion - varchar


	--ROLLBACK
COMMIT