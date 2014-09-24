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

    CREATE TABLE [dbo].[User]
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



    CREATE TABLE [dbo].[Author]
    (
	   [Id] [int] IDENTITY(1,1) CONSTRAINT [PK_Author(Id)] PRIMARY KEY CLUSTERED
    )

    CREATE TABLE [dbo].[AuthorInfo]
    (
	   [Id] [int] IDENTITY(1,1) CONSTRAINT [PK_AuthorInfo(Id)] PRIMARY KEY CLUSTERED,
	   [Author] int NOT NULL CONSTRAINT [FK_AuthorInfo(Author)_Author(Id)] FOREIGN KEY REFERENCES [dbo].[Author] (Id),
	   [Text] varchar(50) NOT NULL,
	   [TextType] int NOT NULL,
    )


    CREATE TABLE [dbo].[Category]
    (
        [Id] [int] IDENTITY(1,1) CONSTRAINT [PK_Category(Id)] PRIMARY KEY CLUSTERED,
	   [Name] varchar(150) NOT NULL,
	   [ParentCategory] int NULL CONSTRAINT [FK_Category(ParentCategory)_Category(Id)] FOREIGN KEY REFERENCES [dbo].[Category](Id)
    )
    
    CREATE TABLE [dbo].[BookType]
    (
	   [Id] int IDENTITY(1,1) NOT NULL CONSTRAINT [PK_BookType(Id)] PRIMARY KEY CLUSTERED,
	   [Type] varchar(50) NOT NULL
    )

    CREATE TABLE [dbo].[Book]
    (
	   [Id] [bigint] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_Book(Id)] PRIMARY KEY CLUSTERED,
	   [Guid] [varchar] (50) NOT NULL,
	   [Category] int NULL CONSTRAINT [FK_Book(Category)_Category(Id)] FOREIGN KEY REFERENCES [dbo].[Category](Id),
	   [BookType] int NULL CONSTRAINT [FK_Book(BookType)_BookType(Id)] FOREIGN KEY REFERENCES [dbo].[BookType](Id)
    )

    CREATE TABLE [dbo].[Bookmark]
    (
	   [Id] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_Bookmark(Id)] PRIMARY KEY CLUSTERED,
	   [User] int NOT NULL CONSTRAINT [FK_Bookmark(Book)_User(Id)] FOREIGN KEY REFERENCES [dbo].[User](Id),
	   [Book] bigint NOT NULL CONSTRAINT [FK_Bookmark(Book)_Book(Id)] FOREIGN KEY REFERENCES [dbo].[Book](Id),
	   [Page] varchar(50) NOT NULL
    )

    CREATE TABLE [dbo].[Transformation]
    (
	   [Id] int IDENTITY(1,1) NOT NULL CONSTRAINT [PK_Transformation(Id)] PRIMARY KEY CLUSTERED,
	   [ResultType] smallint NOT NULL,	    
	   [BookType] int  NULL CONSTRAINT [FK_Transformation(BookType)_BookType(Id)] FOREIGN KEY REFERENCES [dbo].[BookType](Id),
	   [IsDefault] bit NOT NULL
    )

    CREATE TABLE [dbo].[BookVersion]
    (
	   [Id] [bigint] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_BookVersion(Id)] PRIMARY KEY CLUSTERED,
	   [VersionId] [varchar] (50) NOT NULL,
	   [Name] varchar (MAX) NULL,
	   [CreateTime][datetime] NOT NULL,
	   [Description] varchar(MAX) NULL,
	   [Book] bigint NOT NULL CONSTRAINT [FK_BookVersion(Book)_Book(Id)] FOREIGN KEY REFERENCES [dbo].[Book] (Id),
	   [Transformation] int NULL CONSTRAINT [FK_BookVersion(Book)_Transformation(Id)] FOREIGN KEY REFERENCES [dbo].[Transformation] (Id)
    )
    
    CREATE TABLE [dbo].[BookVersion_Author]
    (
	   [Author] int NOT NULL CONSTRAINT [FK_BookVersion_Author(Author)_Author(Id)] FOREIGN KEY REFERENCES [dbo].[Author] (Id),
	   [BookVersion] bigint NOT NULL CONSTRAINT [FK_BookVersion_Author(Book)_Book(Id)] FOREIGN KEY REFERENCES [dbo].[BookVersion](Id),
	   CONSTRAINT [PK_BookVersion_Author(Author)_BookVersion_Author(Book)] PRIMARY KEY ([Author], [BookVersion])
    )

    CREATE TABLE [dbo].[Image]
    (
	   [Id] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_Image(Id)] PRIMARY KEY CLUSTERED,
	   [FileName] varchar(255) NOT NULL,
	   [ImageType] smallint NOT NULL,
	   [Book] bigint NOT NULL CONSTRAINT [FK_Image(Book)_Book(Id)] FOREIGN KEY REFERENCES [dbo].[Book](Id)
    )







    INSERT INTO [dbo].[DatabaseVersion]
		(DatabaseVersion)
	VALUES
		('000' )
		-- DatabaseVersion - varchar


	--ROLLBACK
COMMIT