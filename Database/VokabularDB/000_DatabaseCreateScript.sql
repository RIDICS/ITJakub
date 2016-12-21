SET XACT_ABORT ON;
--USE VokabularDB

ALTER DATABASE CURRENT
SET ALLOW_SNAPSHOT_ISOLATION ON

ALTER DATABASE CURRENT
SET READ_COMMITTED_SNAPSHOT ON WITH ROLLBACK IMMEDIATE



SET XACT_ABORT ON
BEGIN TRAN


-- Create table for storing database version
	CREATE TABLE [dbo].[DatabaseVersion] 
	(
	   [Id] int IDENTITY(1,1) NOT NULL CONSTRAINT [PK_DatabaseVersion(Id)] PRIMARY KEY CLUSTERED,
	   [DatabaseVersion] varchar(50) NOT NULL,
	   [SolutionVersion] varchar(50) NULL,
	   [UpgradeDate] datetime NOT NULL DEFAULT GETDATE(),
	   [UpgradeUser] varchar(150) NOT NULL default SYSTEM_USER,
	)

-- Create generic tables
    CREATE TABLE [dbo].[User]
    (
	   [Id] int IDENTITY(1,1) NOT NULL CONSTRAINT [PK_User(Id)] PRIMARY KEY CLUSTERED,
	   [FirstName] varchar(50) NOT NULL,
	   [LastName] varchar(50) NOT NULL,
	   [Email] varchar(255) NOT NULL,
	   [UserName] varchar(50) NOT NULL CONSTRAINT [UQ_User(UserName)] UNIQUE,
	   [AuthenticationProvider] tinyint NOT NULL,
	   [CommunicationToken] varchar(255) CONSTRAINT [UQ_User(CommunicationToken)] NOT NULL UNIQUE,
	   [CommunicationTokenCreateTime] datetime NULL,    
	   [PasswordHash] varchar(255) NULL,
	   [CreateTime] datetime NOT NULL,
	   [AvatarUrl] varchar (255) NULL,
	   CONSTRAINT [UQ_User(Email)(AuthProvider)] UNIQUE ([Email],[AuthenticationProvider])
    )
	
    CREATE TABLE [dbo].[OriginalAuthor]
    (
	   [Id] int IDENTITY(1,1) CONSTRAINT [PK_OriginalAuthor(Id)] PRIMARY KEY CLUSTERED,
	   [FirstName] varchar(50) NOT NULL,
	   [LastName] varchar(100) NOT NULL,
	   CONSTRAINT [UQ_OriginalAuthor(FirstName)(LastName)] UNIQUE ([FirstName],[LastName])
    )
	
    CREATE TABLE [dbo].[ResponsibleType] 
    (
	   [Id] int IDENTITY(1,1) CONSTRAINT [PK_ResponsibleType(Id)] PRIMARY KEY CLUSTERED,
	   [Text] varchar(100) NOT NULL CONSTRAINT [UQ_ResponsibleType(Text)] UNIQUE,
	   [Type] smallint NOT NULL
    )

    CREATE TABLE [dbo].[ResponsiblePerson]
    (
	   [Id] int IDENTITY(1,1) CONSTRAINT [PK_ResponsiblePerson(Id)] PRIMARY KEY CLUSTERED,
	   [FirstName] varchar(50) NOT NULL,
	   [LastName] varchar(50) NOT NULL,
	   CONSTRAINT [UQ_ResponsiblePerson(FirstName)(LastName)] UNIQUE ([FirstName],[LastName])
    )

	CREATE TABLE [dbo].[Publisher] 
    (
	   [Id] int IDENTITY(1,1) CONSTRAINT [PK_Publisher(Id)] PRIMARY KEY CLUSTERED,
	   [Text] nvarchar(255) NOT NULL CONSTRAINT [UQ_Publisher(Text)] UNIQUE,
	   [Email] varchar(255) NULL
    )
	
    CREATE TABLE [dbo].[BookType]
    (
	   [Id] int IDENTITY(1,1) NOT NULL CONSTRAINT [PK_BookType(Id)] PRIMARY KEY CLUSTERED,
	   [Type] smallint NOT NULL CONSTRAINT [UQ_BookType(Type)] UNIQUE
    )

	CREATE TABLE [dbo].[Category]
    (
       [Id] int IDENTITY(1,1) CONSTRAINT [PK_Category(Id)] PRIMARY KEY CLUSTERED,
	   [ExternalId] varchar(150) NOT NULL CONSTRAINT [UQ_Category(ExternalId)] UNIQUE,
	   [Description] varchar(150) NULL,
	   [Path] varchar(MAX) NOT NULL,
	   [BookType] int NULL CONSTRAINT [FK_Category(BookType)_BookType(Id)] FOREIGN KEY REFERENCES [dbo].[BookType](Id),
	   [ParentCategory] int NULL CONSTRAINT [FK_Category(ParentCategory)_Category(Id)] FOREIGN KEY REFERENCES [dbo].[Category](Id)
    )
	
	CREATE TABLE [dbo].[LiteraryGenre]
    (
	   [Id] int IDENTITY(1,1) NOT NULL CONSTRAINT [PK_LiteraryGenre(Id)] PRIMARY KEY CLUSTERED,
	   [Name] varchar(255) NOT NULL CONSTRAINT [UQ_LiteraryGenre(Name)] UNIQUE
    )

    CREATE TABLE [dbo].[LiteraryKind]
    (
	   [Id] int IDENTITY(1,1) NOT NULL CONSTRAINT [PK_LiteraryKind(Id)] PRIMARY KEY CLUSTERED,
	   [Name] varchar(255) NOT NULL CONSTRAINT [UQ_LiteraryKind(Name)] UNIQUE
    )

-- Create project and resource tables

    CREATE TABLE [dbo].[Project]
    (
	   [Id] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_Project(Id)] PRIMARY KEY CLUSTERED,
	   [Name] varchar(MAX) NOT NULL
	   -- TODO possible reference to latest metadata
	   -- TODO Unique?
    )

	CREATE TABLE [dbo].[Resource]
    (
	   [Id] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_Resource(Id)] PRIMARY KEY CLUSTERED,
	   [Project] bigint NOT NULL CONSTRAINT [FK_Resource(Project)_Project(Id)] FOREIGN KEY REFERENCES [dbo].[Project] (Id),
	   [Name] varchar(255) NOT NULL,
	   [ResourceType] varchar(50) NOT NULL,
	   [ContentType] smallint NOT NULL
    )

	CREATE TABLE [dbo].[ResourceVersion]
	(
	   [Id] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_ResourceVersion(Id)] PRIMARY KEY CLUSTERED,
	   [Resource] bigint NOT NULL CONSTRAINT [FK_ResourceVersion(Resource)_Resource(Id)] FOREIGN KEY REFERENCES [dbo].[Resource] (Id),
	   [ParentResource] bigint NULL CONSTRAINT [FK_ResourceVersion(ParentResource)_Resource(Id)] FOREIGN KEY REFERENCES [dbo].[Resource] (Id),
	   [VersionNumber] int NOT NULL,
	   [CreateTime] datetime NOT NULL,
	   [CreatedByUser] int NOT NULL CONSTRAINT [FK_ResourceVersion(CreatedByUser)_User(Id)] FOREIGN KEY REFERENCES [dbo].[User] (Id),
	   [Comment] varchar(MAX) NULL,
	   CONSTRAINT [UQ_ResourceVersion(Id)(VersionNumber)] UNIQUE ([Id],[VersionNumber])  
	)

	ALTER TABLE [dbo].[Resource] ADD [LatestVersion] bigint NULL CONSTRAINT [FK_Resource(LatestVersion)_ResourceVersion(Id)] FOREIGN KEY REFERENCES [dbo].[ResourceVersion](Id)

	CREATE TABLE [dbo].[MetadataResource]
    (
	   [Id] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_MetadataResource(Id)] PRIMARY KEY CLUSTERED,
	   [ResourceVersion] bigint NOT NULL CONSTRAINT [FK_MetadataResource(ResourceVersion)_ResourceVersion(Id)] FOREIGN KEY REFERENCES [dbo].[ResourceVersion] (Id),
	   [Title] varchar(MAX) NULL, -- TODO is versioned?
	   [SubTitle] varchar(MAX) NULL, -- TODO will be used?
	   [RelicAbbreviation] varchar(100) NULL,
	   [SourceAbbreviation] nvarchar(255) NULL,
	   [Publisher] int NULL CONSTRAINT [FK_BookVersion(Publisher)_Publisher(Id)] FOREIGN KEY REFERENCES [dbo].[Publisher](Id),
	   [PublishPlace] varchar(100) NULL,
	   [PublishDate] varchar(50) NULL,
	   [Copyright] varchar(MAX) NULL,
	   [BiblText] varchar(MAX) NULL,
	   [OriginDate] varchar(50) NULL,
	   [NotBefore] date NULL,
	   [NotAfter] date NULL,
	   [ManuscriptIdno] varchar (50) NULL,
	   [ManuscriptSettlement] varchar (100) NULL,
	   [ManuscriptCountry] varchar (100) NULL,
	   [ManuscriptRepository] varchar (100) NULL,
	   [ManuscriptExtent] varchar(50) NULL, -- TODO unkown value type
	   [ManuscriptTitle] varchar(MAX) NULL -- TODO is different from Title?
	   
	   -- TODO !!! Is possible have multiple different manuscripts?
	   -- TODO handling M:N LiteraryGenre, LiteraryKind
    )

    CREATE TABLE [dbo].[PageResource] 
    (
	   [Id] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_PageResource(Id)] PRIMARY KEY CLUSTERED,
	   [ResourceVersion] bigint NOT NULL CONSTRAINT [FK_PageResource(ResourceVersion)_ResourceVersion(Id)] FOREIGN KEY REFERENCES [dbo].[ResourceVersion] (Id),
	   [Name] varchar(50) NOT NULL,
	   [Position] int NOT NULL
    )

	CREATE TABLE [dbo].[TextResource]
	(
	   [Id] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_TextResource(Id)] PRIMARY KEY CLUSTERED,
	   [ResourceVersion] bigint NOT NULL CONSTRAINT [FK_TextResource(ResourceVersion)_ResourceVersion(Id)] FOREIGN KEY REFERENCES [dbo].[ResourceVersion] (Id),
	   [ExternalId] varchar(100) NULL
	)

	CREATE TABLE [dbo].[ImageResource]
	(
	   [Id] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_ImageResource(Id)] PRIMARY KEY CLUSTERED,
	   [ResourceVersion] bigint NOT NULL CONSTRAINT [FK_ImageResource(ResourceVersion)_ResourceVersion(Id)] FOREIGN KEY REFERENCES [dbo].[ResourceVersion] (Id),
	   [FileName] varchar(255),
	   [MimeType] varchar(255),
	   [Size] bigint NOT NULL
	)

	
-- Create M:N tables
	
	CREATE TABLE [dbo].[Project_OriginalAuthor]
    (
	   [Author] int NOT NULL CONSTRAINT [FK_Project_OriginalAuthor(Author)_OriginalAuthor(Id)] FOREIGN KEY REFERENCES [dbo].[OriginalAuthor] (Id),
	   [Project] bigint NOT NULL CONSTRAINT [FK_Project_OriginalAuthor(Project)_Project(Id)] FOREIGN KEY REFERENCES [dbo].[Project] (Id),
	   CONSTRAINT [PK_Project_OriginalAuthor(Author)_Project_OriginalAuthor(Project)] PRIMARY KEY ([Author], [Project])
    )
	
	CREATE TABLE [dbo].[Project_ResponsiblePerson]
    (
	   [Responsible] int NOT NULL CONSTRAINT [FK_Project_ResponsiblePerson(Responsible)_ResponsiblePerson(Id)] FOREIGN KEY REFERENCES [dbo].[ResponsiblePerson] (Id),
	   [Project] bigint NOT NULL CONSTRAINT [FK_Project_ResponsiblePerson(Project)_Project(Id)] FOREIGN KEY REFERENCES [dbo].[Project] (Id),
	   CONSTRAINT [PK_Project_ResponsiblePerson(Responsible)_Project_ResponsiblePerson(Project)] PRIMARY KEY ([Responsible], [Project])
    )
    
	CREATE TABLE [dbo].[Project_Category]
    (
	   [Category] int NOT NULL CONSTRAINT [FK_Project_Category(Category)_Category(Id)] FOREIGN KEY REFERENCES [dbo].[Category] (Id),
	   [Project] bigint NOT NULL CONSTRAINT [FK_Project_Category(Project)_Project(Id)] FOREIGN KEY REFERENCES [dbo].[Project](Id),
	   CONSTRAINT [PK_Project_Category(Category)_Project_Category(Project)] PRIMARY KEY ([Category], [Project])
    )
	
	CREATE TABLE [dbo].[ResponsiblePerson_ResponsibleType]
    (
	   [Person] int NOT NULL CONSTRAINT [FK_ResponsiblePerson_ResponsibleType(Person)_ResponsiblePerson(Id)] FOREIGN KEY REFERENCES [dbo].[ResponsiblePerson] (Id),
	   [Type] int NOT NULL CONSTRAINT [FK_ResponsiblePerson_ResponsibleType(Type)_ResponsibleType(Id)] FOREIGN KEY REFERENCES [dbo].[ResponsibleType](Id),
	   CONSTRAINT [PK_ResponsiblePerson_ResponsibleType(Person)_ResponsiblePerson_ResponsibleType(Type)] PRIMARY KEY ([Person], [Type])
    )

	CREATE TABLE [dbo].[Project_LiteraryGenre]
    (
	   [LiteraryGenre] int NOT NULL CONSTRAINT [FK_Project_LiteraryGenre(LiteraryGenre)_LiteraryGenre(Id)] FOREIGN KEY REFERENCES [dbo].[LiteraryGenre] (Id),
	   [Project] bigint NOT NULL CONSTRAINT [FK_Project_LiteraryGenre(Project)_Project(Id)] FOREIGN KEY REFERENCES [dbo].[Project](Id),
	   CONSTRAINT [PK_Project_LiteraryGenre(LiteraryGenre)_Project_LiteraryGenre(Project)] PRIMARY KEY ([LiteraryGenre], [Project])
    )

	CREATE TABLE [dbo].[Project_LiteraryKind]
    (
	   [LiteraryKind] int NOT NULL CONSTRAINT [FK_Project_LiteraryKind(LiteraryKind)_LiteraryKind(Id)] FOREIGN KEY REFERENCES [dbo].[LiteraryKind] (Id),
	   [Project] bigint NOT NULL CONSTRAINT [FK_Project_LiteraryKind(Project)_Project(Id)] FOREIGN KEY REFERENCES [dbo].[Project](Id),
	   CONSTRAINT [PK_Project_LiteraryKind(LiteraryKind)_Project_LiteraryKind(Project)] PRIMARY KEY ([LiteraryKind], [Project])
    )

	
	-- TODO check varchar vs nvarchar


-- Insert version number to DatabaseVersion table

    INSERT INTO [dbo].[DatabaseVersion]
		(DatabaseVersion)
	VALUES
		('000')
		-- DatabaseVersion - varchar


	ROLLBACK
--COMMIT