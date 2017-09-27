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
	   [FirstName] nvarchar(50) NOT NULL,
	   [LastName] nvarchar(50) NOT NULL,
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
	   [FirstName] nvarchar(50) NOT NULL,
	   [LastName] nvarchar(100) NOT NULL,
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
	   [FirstName] nvarchar(50) NOT NULL,
	   [LastName] nvarchar(50) NOT NULL,
	   CONSTRAINT [UQ_ResponsiblePerson(FirstName)(LastName)] UNIQUE ([FirstName],[LastName])
    )
	
    CREATE TABLE [dbo].[BookType]
    (
	   [Id] int IDENTITY(1,1) NOT NULL CONSTRAINT [PK_BookType(Id)] PRIMARY KEY CLUSTERED,
	   [Type] smallint NOT NULL CONSTRAINT [UQ_BookType(Type)] UNIQUE
    )

	CREATE TABLE [dbo].[Category]
    (
       [Id] int IDENTITY(1,1) CONSTRAINT [PK_Category(Id)] PRIMARY KEY CLUSTERED,
	   [ExternalId] varchar(150) NULL,
	   [Description] nvarchar(150) NULL,
	   [Path] varchar(MAX) NOT NULL,
	   [ParentCategory] int NULL CONSTRAINT [FK_Category(ParentCategory)_Category(Id)] FOREIGN KEY REFERENCES [dbo].[Category](Id)
    )
	
	CREATE TABLE [dbo].[LiteraryGenre]
    (
	   [Id] int IDENTITY(1,1) NOT NULL CONSTRAINT [PK_LiteraryGenre(Id)] PRIMARY KEY CLUSTERED,
	   [Name] nvarchar(255) NOT NULL CONSTRAINT [UQ_LiteraryGenre(Name)] UNIQUE
    )

    CREATE TABLE [dbo].[LiteraryKind]
    (
	   [Id] int IDENTITY(1,1) NOT NULL CONSTRAINT [PK_LiteraryKind(Id)] PRIMARY KEY CLUSTERED,
	   [Name] nvarchar(255) NOT NULL CONSTRAINT [UQ_LiteraryKind(Name)] UNIQUE
    )

	CREATE TABLE [dbo].[LiteraryOriginal]
    (
	   [Id] int IDENTITY(1,1) NOT NULL CONSTRAINT [PK_LiteraryOriginal(Id)] PRIMARY KEY CLUSTERED,
	   [Name] nvarchar(255) NOT NULL CONSTRAINT [UQ_LiteraryOriginal(Name)] UNIQUE
    )

	CREATE TABLE [dbo].[TermCategory]
    (
	   [Id] int IDENTITY(1,1) NOT NULL CONSTRAINT [PK_TermCategory(Id)] PRIMARY KEY CLUSTERED,
	   [Name] nvarchar(255) NOT NULL CONSTRAINT [UQ_TermCategory(Name)] UNIQUE
    )

	CREATE TABLE [dbo].[Term]
    (
	   [Id] int IDENTITY(1,1) NOT NULL CONSTRAINT [PK_Term(Id)] PRIMARY KEY CLUSTERED,
	   [ExternalId] varchar(255) NULL,
	   [Text] nvarchar(255) NOT NULL CONSTRAINT [UQ_Term(Text)] UNIQUE,
	   [Position] bigint NOT NULL,
	   [TermCategory] int NOT NULL CONSTRAINT [FK_Term(TermCategory)_TermCategory(Id)] FOREIGN KEY REFERENCES [dbo].[TermCategory](Id)
    )

	CREATE TABLE [dbo].[Keyword]
    (
	   [Id] int IDENTITY(1,1) NOT NULL CONSTRAINT [PK_Keyword(Id)] PRIMARY KEY CLUSTERED,
	   [Text] nvarchar(255) NOT NULL CONSTRAINT [UQ_Keyword(Text)] UNIQUE
    )

    
-- Create project and resource tables

    CREATE TABLE [dbo].[Project]
    (
	   [Id] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_Project(Id)] PRIMARY KEY CLUSTERED,
	   [Name] nvarchar(2000) NOT NULL,
	   [CreateTime] datetime NOT NULL,
	   [ExternalId] varchar(255) NULL,
	   [CreatedByUser] int NOT NULL CONSTRAINT [FK_Project(CreatedByUser)_User(Id)] FOREIGN KEY REFERENCES [dbo].[User] (Id)
	   -- TODO last modification time?
    )
		
	CREATE TABLE [dbo].[NamedResourceGroup]
	(
	   [Id] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_NamedResourceGroup(Id)] PRIMARY KEY CLUSTERED,
	   [Project] bigint NOT NULL CONSTRAINT [FK_NamedResourceGroup(Project)_Project(Id)] FOREIGN KEY REFERENCES [dbo].[Project] (Id),
	   [Name] nvarchar(255) NOT NULL,
	   [TextType] smallint NOT NULL
	)

	CREATE TABLE [dbo].[Resource]
    (
	   [Id] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_Resource(Id)] PRIMARY KEY CLUSTERED,
	   [Project] bigint NOT NULL CONSTRAINT [FK_Resource(Project)_Project(Id)] FOREIGN KEY REFERENCES [dbo].[Project] (Id),
	   [Name] varchar(255) NOT NULL,
	   [ResourceType] smallint NOT NULL,
	   [ContentType] smallint NOT NULL,
	   [IsRemoved] bit NOT NULL,
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
	   [Comment] nvarchar(2000) NULL,
	   CONSTRAINT [UQ_ResourceVersion(Id)(VersionNumber)] UNIQUE ([Id],[VersionNumber])
	)

	ALTER TABLE [dbo].[Resource] ADD [LatestVersion] bigint NULL CONSTRAINT [FK_Resource(LatestVersion)_ResourceVersion(Id)] FOREIGN KEY REFERENCES [dbo].[ResourceVersion](Id)

	CREATE TABLE [dbo].[BookVersionResource]
	(
	   [ResourceVersionId] bigint NOT NULL CONSTRAINT [PK_BookVersionResource(ResourceVersionId)] PRIMARY KEY CLUSTERED FOREIGN KEY REFERENCES [dbo].[ResourceVersion] (Id),
	   [ExternalId] varchar(255) NULL
	)

	CREATE TABLE [dbo].[MetadataResource]
    (
	   [ResourceVersionId] bigint NOT NULL CONSTRAINT [PK_MetadataResource(ResourceVersionId)] PRIMARY KEY CLUSTERED FOREIGN KEY REFERENCES [dbo].[ResourceVersion] (Id),
	   [AuthorsLabel] nvarchar(2000) NULL,
	   [Title] nvarchar(2000) NULL,
	   [SubTitle] nvarchar(2000) NULL,
	   [RelicAbbreviation] varchar(100) NULL,
	   [SourceAbbreviation] varchar(255) NULL,
	   [PublishPlace] nvarchar(100) NULL,
	   [PublishDate] varchar(50) NULL,
	   [PublisherText] nvarchar(2000) NULL,
	   [PublisherEmail] varchar(255) NULL,
	   [Copyright] nvarchar(MAX) NULL,
	   [BiblText] nvarchar(MAX) NULL,
	   [OriginDate] varchar(50) NULL,
	   [NotBefore] date NULL,
	   [NotAfter] date NULL,
	   [ManuscriptIdno] nvarchar (50) NULL,
	   [ManuscriptSettlement] nvarchar (100) NULL,
	   [ManuscriptCountry] nvarchar (100) NULL,
	   [ManuscriptRepository] nvarchar (100) NULL,
	   [ManuscriptExtent] nvarchar(2000) NULL,
	   [ManuscriptTitle] nvarchar(2000) NULL
    )

    CREATE TABLE [dbo].[PageResource]
    (
	   [ResourceVersionId] bigint NOT NULL CONSTRAINT [PK_PageResource(ResourceVersionId)] PRIMARY KEY CLUSTERED FOREIGN KEY REFERENCES [dbo].[ResourceVersion] (Id),
	   [Name] nvarchar(50) NOT NULL,
	   [Position] int NOT NULL
    )

	CREATE TABLE [dbo].[TextResource]
	(
	   [ResourceVersionId] bigint NOT NULL CONSTRAINT [PK_TextResource(ResourceVersionId)] PRIMARY KEY CLUSTERED FOREIGN KEY REFERENCES [dbo].[ResourceVersion] (Id),
	   [ExternalId] varchar(100) NULL,
	   [BookVersion] bigint NULL CONSTRAINT [FK_TextResource(BookVersion)_BookVersionResource(ResourceVersionId)] FOREIGN KEY REFERENCES [dbo].[BookVersionResource](ResourceVersionId)
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

	CREATE TABLE [dbo].[TrackResource]
	(
	   [ResourceVersionId] bigint NOT NULL CONSTRAINT [PK_TrackResource(ResourceVersionId)] PRIMARY KEY CLUSTERED FOREIGN KEY REFERENCES [dbo].[ResourceVersion] (Id),
	   [Name] nvarchar(255) NOT NULL,
	   [Text] nvarchar(MAX) NULL,
	   [Position] SMALLINT NOT NULL,
	   [ResourceChapter] bigint NULL CONSTRAINT [FK_TrackResource(ResourceChapter)_Resource(Id)] FOREIGN KEY REFERENCES [dbo].[Resource](Id),
	   [ResourceBeginningPage] bigint NULL CONSTRAINT [FK_TrackResource(ResourceBeginningPage)_Resource(Id)] FOREIGN KEY REFERENCES [dbo].[Resource](Id)
	)

	CREATE TABLE [dbo].[ChapterResource]
	(
	   [ResourceVersionId] bigint NOT NULL CONSTRAINT [PK_ChapterResource(ResourceVersionId)] PRIMARY KEY CLUSTERED FOREIGN KEY REFERENCES [dbo].[ResourceVersion] (Id),
	   [Name] nvarchar(1000) NOT NULL,
	   [Position] int NOT NULL,
	   [ResourceBeginningPage] bigint NULL CONSTRAINT [FK_ChapterResource(ResourceBeginningPage)_Resource(Id)] FOREIGN KEY REFERENCES [dbo].[Resource](Id)
	)

	CREATE TABLE [dbo].[HeadwordResource]
	(
	   [ResourceVersionId] bigint NOT NULL CONSTRAINT [PK_HeadwordResource(ResourceVersionId)] PRIMARY KEY CLUSTERED FOREIGN KEY REFERENCES [dbo].[ResourceVersion] (Id),
	   [ExternalId] varchar(100) NOT NULL,
	   [DefaultHeadword] nvarchar(255) NOT NULL,
	   [Sorting] nvarchar(255) NOT NULL,
	   [BookVersion] bigint NULL CONSTRAINT [FK_HeadwordResource(BookVersion)_BookVersionResource(ResourceVersionId)] FOREIGN KEY REFERENCES [dbo].[BookVersionResource](ResourceVersionId)
	)

	CREATE TABLE [dbo].[HeadwordItem]
	(
	   [Id] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_HeadwordItem(Id)] PRIMARY KEY CLUSTERED,
	   [HeadwordResource] bigint NOT NULL CONSTRAINT [FK_HeadwordItem(HeadwordResource)_HeadwordResource(ResourceVersionId)] FOREIGN KEY REFERENCES [dbo].[HeadwordResource](ResourceVersionId),
	   [Headword] nvarchar(255) NOT NULL,
	   [HeadwordOriginal] nvarchar(255) NULL,
	   [ResourcePage] bigint NULL CONSTRAINT [FK_HeadwordItem(ResourcePage)_Resource(Id)] FOREIGN KEY REFERENCES [dbo].[Resource](Id)
	)

	CREATE TABLE [dbo].[BinaryResource]
	(
	   [ResourceVersionId] bigint NOT NULL CONSTRAINT [PK_BinaryResource(ResourceVersionId)] PRIMARY KEY CLUSTERED FOREIGN KEY REFERENCES [dbo].[ResourceVersion] (Id),
	   [Name] nvarchar(255) NOT NULL,
	   [FileName] varchar(255) NOT NULL
	)

	
-- Other tables
	
	CREATE TABLE [dbo].[FavoriteLabel]
	(
	   [Id] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_FavoriteLabel(Id)] PRIMARY KEY CLUSTERED,
	   [Name] nvarchar(150) NOT NULL,
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
	   [Title] nvarchar(255) NULL,
	   [Description] nvarchar(2000) NULL, --TODO is required?
	   [CreateTime] datetime NULL,
	   [Project] bigint NULL FOREIGN KEY REFERENCES [dbo].[Project] (Id),
	   [Category] int NULL FOREIGN KEY REFERENCES [dbo].[Category] (Id),
	   [Resource] bigint NULL FOREIGN KEY REFERENCES [dbo].[Resource](Id),
	   [ResourceVersion] bigint NULL FOREIGN KEY REFERENCES [dbo].[ResourceVersion](Id),
	   --TODO FavoriteSnapshot
	   [BookType] int NULL FOREIGN KEY REFERENCES [dbo].[BookType](Id),
	   [Query] nvarchar(max) NULL,
	   [QueryType] smallint NULL
    )

	CREATE TABLE [dbo].[Feedback]
	(
	   [Id] bigint IDENTITY(1, 1) NOT NULL CONSTRAINT [PK_Feedback(Id)] PRIMARY KEY CLUSTERED,
	   [FeedbackType] varchar(255) NOT NULL,
	   [Text] nvarchar(2000) NOT NULL,
	   [CreateTime] datetime NOT NULL,
	   [AuthorName] nvarchar(255) NULL,
	   [AuthorEmail] varchar(255) NULL,
	   [AuthorUser] int NULL CONSTRAINT [FK_Feedback(AuthorUser)_User(Id)] FOREIGN KEY REFERENCES [dbo].[User](Id),
	   [FeedbackCategory] smallint NOT NULL,
	   [Project] bigint NULL CONSTRAINT [FK_Feedback(Project)_Project(Id)] FOREIGN KEY REFERENCES [dbo].[Project](Id),
	   [Resource] bigint NULL CONSTRAINT [FK_Feedback(Resource)_Resource(Id)] FOREIGN KEY REFERENCES [dbo].[Resource](Id)
	)

	CREATE TABLE [dbo].[NewsSyndicationItem]
	(
	   [Id] bigint IDENTITY(1, 1) NOT NULL CONSTRAINT [PK_NewsSyndicationItem(Id)] PRIMARY KEY CLUSTERED,
	   [Title] nvarchar(255) NOT NULL,
	   [CreateTime] datetime NOT NULL,
	   [Text] nvarchar(2000) NOT NULL,
	   [Url] varchar(max) NOT NULL,
	   [ItemType] smallint NOT NULL,
	   [User] int NULL CONSTRAINT [FK_NewsSyndicationItem(User)_User(Id)] FOREIGN KEY REFERENCES [dbo].[User](Id)
	)

	CREATE TABLE [dbo].[Transformation]
    (
	   [Id] int IDENTITY(1,1) NOT NULL CONSTRAINT [PK_Transformation(Id)] PRIMARY KEY CLUSTERED,
	   [Name] varchar (100) NOT NULL,
	   [Description] nvarchar (2000) NULL,
	   [OutputFormat] smallint NOT NULL,
	   [BookType] int NULL CONSTRAINT [FK_Transformation(BookType)_BookType(Id)] FOREIGN KEY REFERENCES [dbo].[BookType](Id),
	   [IsDefaultForBookType] bit NOT NULL,
	   [ResourceLevel] smallint NOT NULL
    )

	CREATE TABLE [dbo].[Snapshot]
	(
		[Id] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_Snapshot(Id)] PRIMARY KEY CLUSTERED,
		[VersionNumber] int NOT NULL,
		[CreateTime] datetime NOT NULL,
		[PublishTime] datetime NULL,
		[Project] bigint NOT NULL CONSTRAINT [FK_Snapshot(Project)_Project(Id)] FOREIGN KEY REFERENCES [dbo].[Project](Id),
		[CreatedByUser] int NOT NULL CONSTRAINT [FK_Snapshot(CreatedByUser)_User(Id)] FOREIGN KEY REFERENCES [dbo].[User](Id),
		[Comment] nvarchar(2000),
		[DefaultBookType] int NOT NULL CONSTRAINT [FK_Snapshot(DefaultBookType)_BookType(Id)] FOREIGN KEY REFERENCES [dbo].[BookType](Id)
	)

	ALTER TABLE [dbo].[Project] ADD [LatestPublishedSnapshot] bigint NULL CONSTRAINT [FK_Project(LatestPublishedSnapshot)_Snapshot(Id)] FOREIGN KEY REFERENCES [dbo].[Snapshot](Id)

	CREATE TABLE [dbo].[HistoryLog]
	(
		[Id] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_HistoryLog(Id)] PRIMARY KEY CLUSTERED,
		[Project] bigint NULL CONSTRAINT [FK_HistoryLog(Project)_Project(Id)] FOREIGN KEY REFERENCES [dbo].[Project](Id),
		[User] int NULL CONSTRAINT [FK_HistoryLog(User)_User(Id)] FOREIGN KEY REFERENCES [dbo].[User](Id),
		[CreateTime] datetime NOT NULL,
		[LogType] varchar(255) NOT NULL,
		[Text] nvarchar(2000) NOT NULL,
		[AdditionalDescription] nvarchar(2000) NULL,
		[ExternalId] varchar(255) NULL,
		--[DiscussionPost] bigint NULL CONSTRAINT [FK_HistoryLog(DiscussionPost)_DiscussionPost(Id)] FOREIGN KEY REFERENCES [dbo].[DiscussionPost](Id),
		[Snapshot] bigint NULL CONSTRAINT [FK_HistoryLog(Snapshot)_Snapshot(Id)] FOREIGN KEY REFERENCES [dbo].[Snapshot](Id),
		[ResourceVersion] bigint NULL CONSTRAINT [FK_HistoryLog(ResourceVersion)_ResourceVersion(Id)] FOREIGN KEY REFERENCES [dbo].[ResourceVersion](Id)
	)

		
-- Create M:N tables
	
	CREATE TABLE [dbo].[Project_OriginalAuthor]
    (
	   [Project] bigint NOT NULL CONSTRAINT [FK_Project_OriginalAuthor(Project)_Project(Id)] FOREIGN KEY REFERENCES [dbo].[Project] (Id),
	   [Author] int NOT NULL CONSTRAINT [FK_Project_OriginalAuthor(Author)_OriginalAuthor(Id)] FOREIGN KEY REFERENCES [dbo].[OriginalAuthor] (Id),
	   [Sequence] int NOT NULL,
	   CONSTRAINT [PK_Project_OriginalAuthor(Project)_Project_OriginalAuthor(Author)] PRIMARY KEY ([Project], [Author])
    )
	
	CREATE TABLE [dbo].[Project_ResponsiblePerson]
    (
	   [Project] bigint NOT NULL CONSTRAINT [FK_Project_ResponsiblePerson(Project)_Project(Id)] FOREIGN KEY REFERENCES [dbo].[Project] (Id),
	   [Responsible] int NOT NULL CONSTRAINT [FK_Project_ResponsiblePerson(Responsible)_ResponsiblePerson(Id)] FOREIGN KEY REFERENCES [dbo].[ResponsiblePerson] (Id),
	   [ResponsibleType] int NOT NULL CONSTRAINT [FK_Project_ResponsiblePerson(ResponsibleType)_ResponsibleType(Id)] FOREIGN KEY REFERENCES [dbo].[ResponsibleType] (Id),
	   CONSTRAINT [PK_Project_ResponsiblePerson(Project)_Project_ResponsiblePerson(Responsible)_Project_ResponsiblePerson(ResponsibleType)] PRIMARY KEY ([Project], [Responsible], [ResponsibleType])
    )
    
	CREATE TABLE [dbo].[Project_Category]
    (
	   [Project] bigint NOT NULL CONSTRAINT [FK_Project_Category(Project)_Project(Id)] FOREIGN KEY REFERENCES [dbo].[Project] (Id),
	   [Category] int NOT NULL CONSTRAINT [FK_Project_Category(Category)_Category(Id)] FOREIGN KEY REFERENCES [dbo].[Category] (Id),
	   CONSTRAINT [PK_Project_Category(Project)_Project_Category(Category)] PRIMARY KEY ([Project], [Category])
    )
	
	CREATE TABLE [dbo].[Project_LiteraryGenre]
    (
	   [Project] bigint NOT NULL CONSTRAINT [FK_Project_LiteraryGenre(Project)_Project(Id)] FOREIGN KEY REFERENCES [dbo].[Project](Id),
	   [LiteraryGenre] int NOT NULL CONSTRAINT [FK_Project_LiteraryGenre(LiteraryGenre)_LiteraryGenre(Id)] FOREIGN KEY REFERENCES [dbo].[LiteraryGenre] (Id),
	   CONSTRAINT [PK_Project_LiteraryGenre(Project)_Project_LiteraryGenre(LiteraryGenre)] PRIMARY KEY ([Project], [LiteraryGenre])
    )

	CREATE TABLE [dbo].[Project_LiteraryKind]
    (
	   [Project] bigint NOT NULL CONSTRAINT [FK_Project_LiteraryKind(Project)_Project(Id)] FOREIGN KEY REFERENCES [dbo].[Project](Id),
	   [LiteraryKind] int NOT NULL CONSTRAINT [FK_Project_LiteraryKind(LiteraryKind)_LiteraryKind(Id)] FOREIGN KEY REFERENCES [dbo].[LiteraryKind] (Id),
	   CONSTRAINT [PK_Project_LiteraryKind(Project)_Project_LiteraryKind(LiteraryKind)] PRIMARY KEY ([Project], [LiteraryKind])
    )

	CREATE TABLE [dbo].[Project_LiteraryOriginal]
    (
	   [Project] bigint NOT NULL CONSTRAINT [FK_Project_LiteraryOriginal(Project)_Project(Id)] FOREIGN KEY REFERENCES [dbo].[Project](Id),
	   [LiteraryOriginal] int NOT NULL CONSTRAINT [FK_Project_LiteraryOriginal(LiteraryOriginal)_LiteraryOriginal(Id)] FOREIGN KEY REFERENCES [dbo].[LiteraryOriginal] (Id),
	   CONSTRAINT [PK_Project_LiteraryOriginal(Project)_Project_LiteraryOriginal(LiteraryOriginal)] PRIMARY KEY ([Project], [LiteraryOriginal])
    )

	CREATE TABLE [dbo].[Project_Keyword]
    (
	   [Project] bigint NOT NULL CONSTRAINT [FK_Project_Keyword(Project)_Project(Id)] FOREIGN KEY REFERENCES [dbo].[Project](Id),
	   [Keyword] int NOT NULL CONSTRAINT [FK_Project_Keyword(Keyword)_Keyword(Id)] FOREIGN KEY REFERENCES [dbo].[Keyword](Id),
	   CONSTRAINT [PK_Project_Keyword(Project)_Project_Keyword(Keyword)] PRIMARY KEY ([Project], [Keyword])
    )

	CREATE TABLE [dbo].[PageResource_Term]
	(
	   [PageResource] bigint NOT NULL CONSTRAINT [FK_PageResource_Term(PageResource)_PageResource(Id)] FOREIGN KEY REFERENCES [dbo].[PageResource](ResourceVersionId),
	   [Term] int NOT NULL CONSTRAINT [FK_PageResource_Term(Term)_Term(Id)] FOREIGN KEY REFERENCES [dbo].[Term](Id),
	   CONSTRAINT [PK_PageResource_Term(PageResource)_PageResource_Term(Term)] PRIMARY KEY ([PageResource], [Term])
    )

	CREATE TABLE [dbo].[Snapshot_ResourceVersion]
    (
		[Snapshot] bigint NOT NULL CONSTRAINT [FK_Snapshot_ResourceVersion(Snapshot)_Snapshot(Id)] FOREIGN KEY REFERENCES [dbo].[Snapshot](Id),
		[ResourceVersion] bigint NOT NULL CONSTRAINT [FK_Snapshot_ResourceVersion(ResourceVersion)_ResourceVersion(Id)] FOREIGN KEY REFERENCES [dbo].[ResourceVersion](Id),
		CONSTRAINT [PK_Snapshot_ResourceVersion(Snapshot)_Snapshot_ResourceVersion(ResourceVersion)] PRIMARY KEY ([Snapshot], [ResourceVersion])
    )

	CREATE TABLE [dbo].[Snapshot_BookType]
    (
		[Snapshot] bigint NOT NULL CONSTRAINT [FK_Snapshot_BookType(Snapshot)_Snapshot(Id)] FOREIGN KEY REFERENCES [dbo].[Snapshot](Id),
		[BookType] int NOT NULL CONSTRAINT [FK_Snapshot_BookType(BookType)_BookType(Id)] FOREIGN KEY REFERENCES [dbo].[BookType](Id),
		CONSTRAINT [PK_Snapshot_BookType(Snapshot)_Snapshot_BookType(BookType)] PRIMARY KEY ([Snapshot], [BookType])
    )


-- Insert version number to DatabaseVersion table

    INSERT INTO [dbo].[DatabaseVersion]
		(DatabaseVersion)
	VALUES
		('000')
		-- DatabaseVersion - varchar


	--ROLLBACK
COMMIT