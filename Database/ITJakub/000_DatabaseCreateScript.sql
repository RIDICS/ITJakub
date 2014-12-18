
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
	   [Id] int IDENTITY(1,1) NOT NULL CONSTRAINT [PK_DatabaseVersion(Id)] PRIMARY KEY CLUSTERED,
	   [DatabaseVersion] varchar(50) NOT NULL,
	   [SolutionVersion] varchar(50) NULL,
	   [UpgradeDate] datetime NOT NULL DEFAULT GETDATE(),
	   [UpgradeUser] varchar(150) NOT NULL default SYSTEM_USER,		
	)

    CREATE TABLE [dbo].[User]
    (
	   [Id] int IDENTITY(1,1) NOT NULL CONSTRAINT [PK_User(Id)] PRIMARY KEY CLUSTERED,
	   [FirstName] varchar(50) NOT NULL,
	   [LastName] varchar(50) NOT NULL,
	   [Email] varchar(255) NOT NULL,
	   [AuthenticationProvider] tinyint NOT NULL,
	   [CommunicationToken] varchar(255) NOT NULL UNIQUE,
	   [CommunicationTokenCreateTime] datetime NULL,    
	   [PasswordHash] varchar(255) NULL,
	   [Salt] varchar(50)  NULL,	   
	   [CreateTime] datetime NOT NULL,
	   [AvatarUrl] varchar (255) NULL,
	   CONSTRAINT [Uniq_Email_AuthProvider] UNIQUE ([Email],[AuthenticationProvider])    
    )



    CREATE TABLE [dbo].[Author]
    (
	   [Id] int IDENTITY(1,1) CONSTRAINT [PK_Author(Id)] PRIMARY KEY CLUSTERED,
	   [Name] varchar(100) NOT NULL
    )


    CREATE TABLE [dbo].[ResponsibleType] 
    (
	   [Id] int IDENTITY(1,1) CONSTRAINT [PK_ResponsibleType(Id)] PRIMARY KEY CLUSTERED,
	   [Text] varchar(100) NOT NULL,
	   [Type] smallint NULL
    )

     CREATE TABLE [dbo].[Responsible]
    (
	   [Id] int IDENTITY(1,1) CONSTRAINT [PK_Responsible(Id)] PRIMARY KEY CLUSTERED,
	   [Text] varchar(100) NOT NULL,
	   [ResponsibleType] int NULL CONSTRAINT [FK_Responsible(ResponsibleType)_ResponsibleType(Id)] FOREIGN KEY REFERENCES [dbo].[ResponsibleType](Id)
	   
    )

    CREATE TABLE [dbo].[Publisher] 
    (
	   [Id] int IDENTITY(1,1) CONSTRAINT [PK_Publisher(Id)] PRIMARY KEY CLUSTERED,
	   [Text] varchar(100) NULL,
	   [Email] varchar(100) NULL
    )



    CREATE TABLE [dbo].[Category]
    (
        [Id] int IDENTITY(1,1) CONSTRAINT [PK_Category(Id)] PRIMARY KEY CLUSTERED,
	   [XmlId] varchar(150) NOT NULL UNIQUE,
	   [Description] varchar(150) NULL,
	   [ParentCategory] int NULL CONSTRAINT [FK_Category(ParentCategory)_Category(Id)] FOREIGN KEY REFERENCES [dbo].[Category](Id)
    )
    
    CREATE TABLE [dbo].[BookType]
    (
	   [Id] int IDENTITY(1,1) NOT NULL CONSTRAINT [PK_BookType(Id)] PRIMARY KEY CLUSTERED,
	   [Type] smallint NOT NULL
    )

    CREATE TABLE [dbo].[Book]
    (
	   [Id] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_Book(Id)] PRIMARY KEY CLUSTERED,
	   [Guid] varchar(50) NOT NULL,
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
	   [Name] varchar (100) NOT NULL,
	   [Description] varchar (MAX) NULL,	    
	   [OutputFormat] smallint NOT NULL,	    
	   [BookType] int  NULL CONSTRAINT [FK_Transformation(BookType)_BookType(Id)] FOREIGN KEY REFERENCES [dbo].[BookType](Id),
	   [IsDefaultForBookType] bit NOT NULL,
	   [ResourceLevel] smallint NOT NULL
    )



    CREATE TABLE [dbo].[BookVersion]
    (
	   [Id] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_BookVersion(Id)] PRIMARY KEY CLUSTERED,
	   [VersionId] varchar(50) NOT NULL,
	   [Title] varchar (MAX) NULL,
	   [SubTitle] varchar (MAX) NULL,
	   [CreateTime] datetime NOT NULL,
	   [Description] varchar(MAX) NULL,
	   [Book] bigint NOT NULL CONSTRAINT [FK_BookVersion(Book)_Book(Id)] FOREIGN KEY REFERENCES [dbo].[Book] (Id),
	   [Publisher] int NULL CONSTRAINT [FK_BookVersion(Publisher)_Publisher(Id)] FOREIGN KEY REFERENCES [dbo].[Publisher](Id),
	   [PublishPlace] varchar(100) NULL,
	   [PublishDate] varchar(50) NULL,
	   [Copyright] varchar(MAX) NULL,
	   [AvailabilityStatus] smallint NULL,
	   [BiblText] varchar (MAX) NULL,
	   CONSTRAINT [Uniq_VersionId_Book] UNIQUE ([VersionId],[Book])  

    )

    CREATE TABLE [dbo].[BookVersion_Transformation]
    (
	   [Transformation] int NOT NULL CONSTRAINT [FK_BookVersion_Transformation(Transformation)_Transformation(Id)] FOREIGN KEY REFERENCES [dbo].[Transformation] (Id),
	   [BookVersion] bigint NOT NULL CONSTRAINT [FK_BookVersion_Transformation(Book)_Book(Id)] FOREIGN KEY REFERENCES [dbo].[BookVersion](Id),
	   CONSTRAINT [PK_BookVersion_Transformation(Transformation)_BookVersion_Transformation(Book)] PRIMARY KEY ([Transformation], [BookVersion])
    )

    CREATE TABLE [dbo].[BookPage] 
    (
	   [Id] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_BookPage(Id)] PRIMARY KEY CLUSTERED,
	   [Text] varchar(50) NULL,
	   [XmlId] varchar(100) NULL,
	   [XmlResource] varchar(100) NULL,
	   [Image] varchar(100) NULL,	   
	   [Position] int NOT NULL,
	   [BookVersion] bigint NOT NULL CONSTRAINT [FK_BookPage(BookVersion)_BookVersion(Id)] FOREIGN KEY REFERENCES [dbo].[BookVersion](Id)

    )

    CREATE TABLE [dbo].[ManuscriptDescription]
    (
	   [Id] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_ManuscriptDescription(Id)] PRIMARY KEY CLUSTERED,
	   [BookVersion] bigint NULL CONSTRAINT [FK_ManuscriptDescription(BookVersion)_ManuscriptDescription(Id)] FOREIGN KEY REFERENCES [dbo].[BookVersion] (Id),
	   [Title] varchar (MAX) NULL,
	   [Idno] varchar (50) NULL,
	   [Settlement] varchar (100) NULL,
	   [Country] varchar (100) NULL,
	   [Repository] varchar (50) NULL,
	   [OriginDate] varchar (50) NULL   

    )

    CREATE TABLE [dbo].[BookBibl] 
    (
	   [Id] int IDENTITY(1,1) NOT NULL CONSTRAINT [PK_BookBibl(Id)] PRIMARY KEY CLUSTERED,
	   [Text] varchar(50) NULL,
	   [Type] varchar(50) NULL,
	   [Subtype] varchar(50) NULL,
	   [BiblType] smallint NULL,
	   [BookVersion] bigint NULL CONSTRAINT [FK_BookBibl(BookVersion)_BookVersion(Id)] FOREIGN KEY REFERENCES [dbo].[BookVersion](Id)

    )

	CREATE TABLE [dbo].[Keyword] 
    (
	   [Id] [bigint] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_Keyword(Id)] PRIMARY KEY CLUSTERED,
	   [Text] varchar(50) NULL,
	   [BookVersion] bigint NULL CONSTRAINT [FK_Keyword(BookVersion)_BookVersion(Id)] FOREIGN KEY REFERENCES [dbo].[BookVersion](Id)

    )

    CREATE TABLE [dbo].[BookVersion_Responsible]
    (
	   [Responsible] int NOT NULL CONSTRAINT [FK_BookVersion_Responsible(Responsible)_Responsible(Id)] FOREIGN KEY REFERENCES [dbo].[Responsible] (Id),
	   [BookVersion] bigint NOT NULL CONSTRAINT [FK_BookVersion_Responsible(Book)_Book(Id)] FOREIGN KEY REFERENCES [dbo].[BookVersion](Id),
	   CONSTRAINT [PK_BookVersion_Responsible(Responsible)_BookVersion_Responsible(Book)] PRIMARY KEY ([Responsible], [BookVersion])
    )
    
    CREATE TABLE [dbo].[BookVersion_Author]
    (
	   [Author] int NOT NULL CONSTRAINT [FK_BookVersion_Author(Author)_Author(Id)] FOREIGN KEY REFERENCES [dbo].[Author] (Id),
	   [BookVersion] bigint NOT NULL CONSTRAINT [FK_BookVersion_Author(Book)_Book(Id)] FOREIGN KEY REFERENCES [dbo].[BookVersion](Id),
	   CONSTRAINT [PK_BookVersion_Author(Author)_BookVersion_Author(Book)] PRIMARY KEY ([Author], [BookVersion])
    )

    INSERT INTO [dbo].[DatabaseVersion]
		(DatabaseVersion)
	VALUES
		('000' )
		-- DatabaseVersion - varchar


	--ROLLBACK
COMMIT