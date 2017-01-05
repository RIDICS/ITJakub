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
	   [AvatarUrl] varchar(255) NULL,
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
	   [BookType] int NOT NULL CONSTRAINT [FK_Category(BookType)_BookType(Id)] FOREIGN KEY REFERENCES [dbo].[BookType](Id),
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

	CREATE TABLE [dbo].[TermCategory]
    (
	   [Id] int IDENTITY(1,1) NOT NULL CONSTRAINT [PK_TermCategory(Id)] PRIMARY KEY CLUSTERED,
	   [Name] varchar(255) NOT NULL CONSTRAINT [UQ_TermCategory(Name)] UNIQUE
    )

	CREATE TABLE [dbo].[Term]
    (
	   [Id] int IDENTITY(1,1) NOT NULL CONSTRAINT [PK_Term(Id)] PRIMARY KEY CLUSTERED,
	   [ExternalId] varchar(255) NULL,
	   [Text] varchar(255) NOT NULL CONSTRAINT [UQ_Term(Text)] UNIQUE,
	   [Position] bigint NOT NULL,
	   [TermCategory] int NOT NULL CONSTRAINT [FK_Term(TermCategory)_TermCategory(Id)] FOREIGN KEY REFERENCES [dbo].[TermCategory](Id)
    )

	CREATE TABLE [dbo].[Keyword]
    (
	   [Id] int IDENTITY(1,1) NOT NULL CONSTRAINT [PK_Keyword(Id)] PRIMARY KEY CLUSTERED,
	   [Text] varchar(255) NOT NULL CONSTRAINT [UQ_Keyword(Text)] UNIQUE
    )

    
-- Create project and resource tables

    CREATE TABLE [dbo].[Project]
    (
	   [Id] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_Project(Id)] PRIMARY KEY CLUSTERED,
	   [Name] varchar(MAX) NOT NULL,
	   [CreateTime] datetime NOT NULL,
	   [CreatedByUser] int NOT NULL CONSTRAINT [FK_Project(CreatedByUser)_User(Id)] FOREIGN KEY REFERENCES [dbo].[User] (Id)
	   -- TODO possible reference to latest metadata
	   -- TODO Unique?
	   -- TODO last modification time?
    )
		
	CREATE TABLE [dbo].[NamedResourceGroup]
	(
	   [Id] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_NamedResourceGroup(Id)] PRIMARY KEY CLUSTERED,
	   [Project] bigint NOT NULL CONSTRAINT [FK_NamedResourceGroup(Project)_Project(Id)] FOREIGN KEY REFERENCES [dbo].[Project] (Id),
	   [Name] varchar(255) NOT NULL,
	   [TextType] smallint NOT NULL
	)

	CREATE TABLE [dbo].[Resource]
    (
	   [Id] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_Resource(Id)] PRIMARY KEY CLUSTERED,
	   [Project] bigint NOT NULL CONSTRAINT [FK_Resource(Project)_Project(Id)] FOREIGN KEY REFERENCES [dbo].[Project] (Id),
	   [Name] varchar(255) NOT NULL,
	   [ResourceType] smallint NOT NULL,
	   [ContentType] smallint NOT NULL,
	   [NamedResourceGroup] bigint NULL CONSTRAINT [FK_Resource(NamedResourceGroup)_NamedResourceGroup(Id)] FOREIGN KEY REFERENCES [dbo].[NamedResourceGroup] (Id)
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
	   [ResourceVersionId] bigint NOT NULL CONSTRAINT [PK_MetadataResource(ResourceVersionId)] PRIMARY KEY CLUSTERED FOREIGN KEY REFERENCES [dbo].[ResourceVersion] (Id),
	   [Title] varchar(MAX) NULL, -- TODO is versioned?
	   [SubTitle] varchar(MAX) NULL, -- TODO will be used?
	   [RelicAbbreviation] varchar(100) NULL,
	   [SourceAbbreviation] nvarchar(255) NULL,
	   [Publisher] int NULL CONSTRAINT [FK_MetadataResource(Publisher)_Publisher(Id)] FOREIGN KEY REFERENCES [dbo].[Publisher](Id),
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
	   [ResourceVersionId] bigint NOT NULL CONSTRAINT [PK_PageResource(ResourceVersionId)] PRIMARY KEY CLUSTERED FOREIGN KEY REFERENCES [dbo].[ResourceVersion] (Id),
	   [Name] varchar(50) NOT NULL,
	   [Position] int NOT NULL
    )

	CREATE TABLE [dbo].[TextResource]
	(
	   [ResourceVersionId] bigint NOT NULL CONSTRAINT [PK_TextResource(ResourceVersionId)] PRIMARY KEY CLUSTERED FOREIGN KEY REFERENCES [dbo].[ResourceVersion] (Id),
	   [ExternalId] varchar(100) NULL
	)

	CREATE TABLE [dbo].[ImageResource]
	(
	   [ResourceVersionId] bigint NOT NULL CONSTRAINT [PK_ImageResource(ResourceVersionId)] PRIMARY KEY CLUSTERED FOREIGN KEY REFERENCES [dbo].[ResourceVersion] (Id),
	   [FileName] varchar(255) NOT NULL,
	   [MimeType] varchar(255) NOT NULL,
	   [Size] bigint NOT NULL
	)

	CREATE TABLE [dbo].[AudioResource]
	(
	   [ResourceVersionId] bigint NOT NULL CONSTRAINT [PK_AudioResource(ResourceVersionId)] PRIMARY KEY CLUSTERED FOREIGN KEY REFERENCES [dbo].[ResourceVersion] (Id),
	   [Duration] bigint NULL,
	   [FileName] varchar(255) NOT NULL,
	   [AudioType] tinyint NOT NULL,
	   [MimeType] varchar(255) NOT NULL
	)

	CREATE TABLE [dbo].[ChapterResource]
	(
	   [ResourceVersionId] bigint NOT NULL CONSTRAINT [PK_ChapterResource(ResourceVersionId)] PRIMARY KEY CLUSTERED FOREIGN KEY REFERENCES [dbo].[ResourceVersion] (Id),
	   [Name] varchar(MAX) NOT NULL,
	   [Position] int NOT NULL,
	   [BeginningPageResource] bigint NULL CONSTRAINT [FK_ChapterResource(BeginningPageResource)_Resource(Id)] FOREIGN KEY REFERENCES [dbo].[Resource](Id)
	)

	CREATE TABLE [dbo].[DefaultHeadwordResource]
	(
	   [ResourceVersionId] bigint NOT NULL CONSTRAINT [PK_DefaultHeadwordResource(ResourceVersionId)] PRIMARY KEY CLUSTERED FOREIGN KEY REFERENCES [dbo].[ResourceVersion] (Id),
	   [ExternalId] varchar(100) NOT NULL,
	   [DefaultHeadword] nvarchar(255) NOT NULL,
	   [Sorting] nvarchar(255) NOT NULL
	)

	CREATE TABLE [dbo].[HeadwordResource]
	(
	   [ResourceVersionId] bigint NOT NULL CONSTRAINT [PK_HeadwordResource(ResourceVersionId)] PRIMARY KEY CLUSTERED FOREIGN KEY REFERENCES [dbo].[ResourceVersion] (Id),
	   [Headword] nvarchar(255) NOT NULL,
	   [HeadwordOriginal] nvarchar(255) NULL,
	   [PageResource] bigint NULL CONSTRAINT [FK_HeadwordResource(PageResource)_Resource(Id)] FOREIGN KEY REFERENCES [dbo].[Resource](Id)
	)

	CREATE TABLE [dbo].[TermResource]
	(
	   [ResourceVersionId] bigint NOT NULL CONSTRAINT [PK_TermResource(ResourceVersionId)] PRIMARY KEY CLUSTERED FOREIGN KEY REFERENCES [dbo].[ResourceVersion] (Id),
	   [Term] int NOT NULL CONSTRAINT [FK_TermResource(Term)_Term(Id)] FOREIGN KEY REFERENCES [dbo].[Term](Id)
	   --ParentResource is PageResource
    )

	CREATE TABLE [dbo].[KeywordResource]
    (
	   [ResourceVersionId] bigint NOT NULL CONSTRAINT [PK_KeywordResource(ResourceVersionId)] PRIMARY KEY CLUSTERED FOREIGN KEY REFERENCES [dbo].[ResourceVersion] (Id),
	   [Keyword] int NOT NULL CONSTRAINT [FK_KeywordResource(Keyword)_Keyword(Id)] FOREIGN KEY REFERENCES [dbo].[Keyword](Id)
    )


-- Other tables
	
	CREATE TABLE [dbo].[FavoriteLabel]
	(
	   [Id] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_FavoriteLabel(Id)] PRIMARY KEY CLUSTERED,
	   [Name] varchar(150) NOT NULL,
	   [Color] char(7) NOT NULL,
	   [User] int NOT NULL CONSTRAINT [FK_FavoriteLabel(User)_User(Id)] FOREIGN KEY REFERENCES [dbo].[User](Id),
	   --[ParentLabel] bigint NULL CONSTRAINT [FK_FavoriteLabel(ParentLabel)_FavoriteLabel(Id)] FOREIGN KEY REFERENCES [dbo].[FavoriteLabel](Id),
	   [IsDefault] bit NOT NULL,
	   [LastUseTime] datetime NULL
	)
	
	CREATE TABLE [dbo].[Favorite]
	(
	   [Id] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_Favorite(Id)] PRIMARY KEY CLUSTERED,
	   [FavoriteType] varchar(255) NOT NULL,
	   [FavoriteLabel] bigint NOT NULL FOREIGN KEY REFERENCES [dbo].[FavoriteLabel](Id),
	   [Title] varchar(255) NULL,
	   [Description] varchar(max) NULL, --TODO is required?
	   [CreateTime] datetime NULL,
	   [Project] bigint NULL FOREIGN KEY REFERENCES [dbo].[Project] (Id),
	   [Category] int NULL FOREIGN KEY REFERENCES [dbo].[Category] (Id),
	   [Resource] bigint NULL FOREIGN KEY REFERENCES [dbo].[Resource](Id),
	   [ResourceVersion] bigint NULL FOREIGN KEY REFERENCES [dbo].[ResourceVersion](Id),
	   --TODO FavoriteSnapshot
	   [BookType] int NULL FOREIGN KEY REFERENCES [dbo].[BookType](Id),
	   [Query] varchar(max) NULL,
	   [QueryType] smallint NULL
    )

	CREATE TABLE [dbo].[Feedback]
	(
	   [Id] bigint IDENTITY(1, 1) NOT NULL CONSTRAINT [PK_Feedback(Id)] PRIMARY KEY CLUSTERED,
	   [FeedbackType] varchar(255) NOT NULL,
	   [Text] nvarchar(2000) NOT NULL,
	   [CreateTime] datetime NOT NULL,
	   [AuthorName] varchar(255) NULL,
	   [AuthorEmail] varchar(255) NULL,
	   [AuthorUser] int NULL CONSTRAINT [FK_Feedback(AuthorUser)_User(Id)] FOREIGN KEY REFERENCES [dbo].[User](Id),
	   [FeedbackCategory] smallint NOT NULL,
	   [Project] bigint NULL CONSTRAINT [FK_Feedback(Project)_Project(Id)] FOREIGN KEY REFERENCES [dbo].[Project](Id),
	   [Resource] bigint NULL CONSTRAINT [FK_Feedback(Resource)_Resource(Id)] FOREIGN KEY REFERENCES [dbo].[Resource](Id)
	)

	CREATE TABLE [dbo].[NewsSyndicationItem]
	(
	   [Id] bigint IDENTITY(1, 1) NOT NULL CONSTRAINT [PK_NewsSyndicationItem(Id)] PRIMARY KEY CLUSTERED,
	   [Title] varchar(255) NOT NULL,
	   [CreateTime] datetime NOT NULL,
	   [Text] varchar(2000) NOT NULL,
	   [Url] varchar(max) NOT NULL,
	   [ItemType] smallint NOT NULL,
	   [User] int NULL CONSTRAINT [FK_NewsSyndicationItem(User)_User(Id)] FOREIGN KEY REFERENCES [dbo].[User](Id)
	)

	CREATE TABLE [dbo].[Transformation]
    (
	   [Id] int IDENTITY(1,1) NOT NULL CONSTRAINT [PK_Transformation(Id)] PRIMARY KEY CLUSTERED,
	   [Name] varchar (100) NOT NULL,
	   [Description] varchar (MAX) NULL,
	   [OutputFormat] smallint NOT NULL,
	   [BookType] int NULL CONSTRAINT [FK_Transformation(BookType)_BookType(Id)] FOREIGN KEY REFERENCES [dbo].[BookType](Id),
	   [IsDefaultForBookType] bit NOT NULL,
	   [ResourceLevel] smallint NOT NULL
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