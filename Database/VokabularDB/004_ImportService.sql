SET XACT_ABORT ON;

BEGIN TRAN

	CREATE TABLE[dbo].[ExternalResourceType]
	(
		[Id][int] IDENTITY(1,1) NOT NULL CONSTRAINT[PK_ExternalResourceType(Id)] PRIMARY KEY CLUSTERED,
		[Name] [varchar] (50) NOT NULL CONSTRAINT[UQ_ExternalResourceType(Name)] UNIQUE
	)

	CREATE TABLE [dbo].[ParserType]
	(
		[Id] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_ParserType(Id)] PRIMARY KEY CLUSTERED,
		[Name] [varchar](50) NOT NULL CONSTRAINT [UQ_ParserType(Name)] UNIQUE
	)	

	CREATE TABLE [dbo].[ExternalResource]
	(
		[Id] int IDENTITY(1,1) NOT NULL CONSTRAINT [PK_ExternalResource(Id)] PRIMARY KEY CLUSTERED,
		[Name] nvarchar(255) NOT NULL,
		[Description] nvarchar(MAX) NULL,
		[Url] varchar(255) NOT NULL,
		[License] nvarchar(MAX) NULL,
		[Configuration] nvarchar(MAX) NULL,
		[CreatedByUser] int NOT NULL CONSTRAINT [FK_ExternalResource(CreatedByUser)_User(Id)] FOREIGN KEY REFERENCES [dbo].[User] (Id),
		[ParserType] int NOT NULL CONSTRAINT [FK_ExternalResource(ParserType)_ParserType(Id)] FOREIGN KEY REFERENCES [dbo].[ParserType] (Id),
		[ExternalResourceType] int NOT NULL CONSTRAINT [FK_ExternalResource(ExternalResourceType)_ExternalResourceType(Id)] FOREIGN KEY REFERENCES [dbo].[ExternalResourceType] (Id)
	)
    
	CREATE TABLE [dbo].[ImportHistory]
	(
		[Id] int IDENTITY(1,1) NOT NULL CONSTRAINT [PK_ImportHistory(Id)] PRIMARY KEY CLUSTERED,
		[Date] datetime NOT NULL,
		[Status] tinyint NOT NULL,
		[Message] nvarchar(255) NULL,
		[UpdatedItems] int NOT NULL,
		[NewItems] int NOT NULL,
		[FailedItems] int NOT NULL,
		[ExternalResource] int NOT NULL CONSTRAINT [FK_ImportHistory(ExternalResource)_ExternalResource(Id)] FOREIGN KEY REFERENCES [dbo].[ExternalResource] (Id),
		[UpdatedByUser] int NOT NULL CONSTRAINT [FK_ImportHistory(UpdatedByUser)_User(Id)] FOREIGN KEY REFERENCES [dbo].[User] (Id)
	)
	
	CREATE TABLE [dbo].[ProjectImportMetadata]
	(
		[Id] int IDENTITY(1,1) NOT NULL CONSTRAINT [PK_ProjectImportMetadata(Id)] PRIMARY KEY CLUSTERED,
		[ExternalId] varchar(50) NOT NULL,
		[LastUpdateMessage] nvarchar(255) NULL,
		[LastUpdate] int NOT NULL CONSTRAINT [FK_ProjectImportMetadata(LastUpdate)_ImportHistory(Id)] FOREIGN KEY REFERENCES [dbo].[ImportHistory] (Id),
		[LastSuccessfulUpdate] int NULL CONSTRAINT [FK_ProjectImportMetadata(LastSuccessfulUpdate)_ImportHistory(Id)] FOREIGN KEY REFERENCES [dbo].[ImportHistory] (Id),
		[Project] bigint NOT NULL CONSTRAINT [FK_ProjectImportMetadatay(Project)_Project(Id)] FOREIGN KEY REFERENCES [dbo].[Project] (Id),
	)

    INSERT INTO [dbo].[DatabaseVersion]
		(DatabaseVersion)
    VALUES
		('004')
		-- DatabaseVersion - varchar

--ROLLBACK
COMMIT 