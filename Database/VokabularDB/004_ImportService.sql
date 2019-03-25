SET XACT_ABORT ON;

BEGIN TRAN

	CREATE TABLE[dbo].[ExternalRepositoryType]
	(
		[Id][int] IDENTITY(1,1) NOT NULL CONSTRAINT[PK_ExternalRepositoryType(Id)] PRIMARY KEY CLUSTERED,
		[Name] [varchar] (50) NOT NULL CONSTRAINT[UQ_ExternalRepositoryType(Name)] UNIQUE
	)

	CREATE TABLE [dbo].[BibliographicFormat]
	(
		[Id] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_BibliographicFormat(Id)] PRIMARY KEY CLUSTERED,
		[Name] [varchar](50) NOT NULL CONSTRAINT [UQ_BibliographicFormat(Name)] UNIQUE
	)	

	CREATE TABLE [dbo].[ExternalRepository]
	(
		[Id] int IDENTITY(1,1) NOT NULL CONSTRAINT [PK_ExternalRepository(Id)] PRIMARY KEY CLUSTERED,
		[Name] nvarchar(255) NOT NULL,
		[Description] nvarchar(MAX) NULL,
		[Url] varchar(255) NOT NULL,
		[License] nvarchar(MAX) NULL,
		[Configuration] nvarchar(MAX) NULL,
		[CreatedByUser] int NOT NULL CONSTRAINT [FK_ExternalRepository(CreatedByUser)_User(Id)] FOREIGN KEY REFERENCES [dbo].[User] (Id),
		[BibliographicFormat] int NOT NULL CONSTRAINT [FK_ExternalRepository(BibliographicFormat)_BibliographicFormat(Id)] FOREIGN KEY REFERENCES [dbo].[BibliographicFormat] (Id),
		[ExternalRepositoryType] int NOT NULL CONSTRAINT [FK_ExternalRepository(ExternalResourceType)_ExternalRepositoryType(Id)] FOREIGN KEY REFERENCES [dbo].[ExternalRepositoryType] (Id)
	)
    
	CREATE TABLE [dbo].[ImportHistory]
	(
		[Id] int IDENTITY(1,1) NOT NULL CONSTRAINT [PK_ImportHistory(Id)] PRIMARY KEY CLUSTERED,
		[Date] datetime NOT NULL,
		[Status] tinyint NOT NULL,
		[Message] nvarchar(255) NULL,
		[ExternalRepository] int NOT NULL CONSTRAINT [FK_ImportHistory(ExternalRepository)_ExternalRepository(Id)] FOREIGN KEY REFERENCES [dbo].[ExternalRepository] (Id),
		[CreatedByUser] int NOT NULL CONSTRAINT [FK_ImportHistory(CreatedByUser)_User(Id)] FOREIGN KEY REFERENCES [dbo].[User] (Id)
	)
	
	CREATE TABLE [dbo].[ImportMetadata]
	(
		[Id] int IDENTITY(1,1) NOT NULL CONSTRAINT [PK_ImportMetadata(Id)] PRIMARY KEY CLUSTERED,
		[ExternalId] varchar(50) NOT NULL,
		[LastUpdateMessage] nvarchar(255) NULL,
		[LastUpdate] int NOT NULL CONSTRAINT [FK_ImportMetadata(LastUpdate)_ImportHistory(Id)] FOREIGN KEY REFERENCES [dbo].[ImportHistory] (Id),
		[Snapshot] bigint NULL CONSTRAINT [FK_ImportMetadata(Snapshot)_Snapshot(Id)] FOREIGN KEY REFERENCES [dbo].[Snapshot] (Id),
	)

	CREATE TABLE [dbo].[FilteringExpressionSet]
	(
		[Id] int IDENTITY(1,1) NOT NULL CONSTRAINT [PK_FilteringExpressionSet(Id)] PRIMARY KEY CLUSTERED,
		[Name] nvarchar(255) NOT NULL,
		[BibliographicFormat] int NOT NULL CONSTRAINT [FK_FilteringExpressionSet(BibliographicFormat)_BibliographicFormat(Id)] FOREIGN KEY REFERENCES [dbo].[BibliographicFormat] (Id),
		[CreatedByUser] int NOT NULL CONSTRAINT [FK_FilteringExpressionSet(CreatedByUser)_User(Id)] FOREIGN KEY REFERENCES [dbo].[User] (Id)
	)

	CREATE TABLE [dbo].[ExternalRepository_FilteringExpressionSet](
		[ExternalRepository] int NOT NULL CONSTRAINT [FK_ExternalRepository_FilteringExpressionSet(ExternalRepository)_ExternalRepository(Id)] FOREIGN KEY REFERENCES [dbo].[ExternalRepository] (Id),
		[FilteringExpressionSet] int NOT NULL CONSTRAINT [FK_ExternalRepository_FilteringExpressionSet(FilteringExpressionSet)_FilteringExpressionSet(Id)] FOREIGN KEY REFERENCES [dbo].[FilteringExpressionSet](Id),
		CONSTRAINT [PK_ExternalRepository_FilteringExpressionSet(ExternalRepository)_ExternalRepo_FilteringExpressionSet(FilteringExpressionSet)] PRIMARY KEY ([ExternalRepository], [FilteringExpressionSet])
	);

	CREATE TABLE [dbo].[FilteringExpression]
	(
		[Id] int IDENTITY(1,1) NOT NULL CONSTRAINT [PK_FilteringExpressiony(Id)] PRIMARY KEY CLUSTERED,
		[Key] nvarchar(255) NOT NULL,
		[Field] nvarchar(255) NOT NULL,
		[FilteringExpressionSet] int NOT NULL CONSTRAINT [FK_FilteringExpression(FilteringExpressionSet)_FilteringExpressionSet(Id)] FOREIGN KEY REFERENCES [dbo].[FilteringExpression] (Id)
	)

	ALTER TABLE [dbo].[SpecialPermission] ADD [CanManageRepositoryImport] bit NULL;  

    INSERT INTO [dbo].[DatabaseVersion]
		(DatabaseVersion)
    VALUES
		('004')
		-- DatabaseVersion - varchar

--ROLLBACK
COMMIT 