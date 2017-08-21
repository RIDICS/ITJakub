SET XACT_ABORT ON;

BEGIN TRAN

    CREATE TABLE [dbo].[DiscussionPost]
	(
		[Id] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_DiscussionPost(Id)] PRIMARY KEY CLUSTERED,
		[Text] nvarchar(2000) NOT NULL,
		[CreateTime] datetime NOT NULL,
		[User] int NOT NULL CONSTRAINT [FK_DiscussionPost(User)_User(Id)] FOREIGN KEY REFERENCES [dbo].[User](Id),
		[ParentPost] bigint NULL CONSTRAINT [FK_DiscussionPost(ParentPost)_DiscussionPost(Id)] FOREIGN KEY REFERENCES [dbo].[DiscussionPost](Id),
		[Resource] bigint NOT NULL CONSTRAINT [FK_DiscussionPost(Resource)_Resource(Id)] FOREIGN KEY REFERENCES [dbo].[Resource](Id),
		[EditCount] int NULL,
		[LastEditTime] datetime NULL
	)

	CREATE TABLE [dbo].[TextComment]
	(
		[Id] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_TextComment(Id)] PRIMARY KEY CLUSTERED,
		[Text] nvarchar(2000) NOT NULL,
		[CreateTime] datetime NOT NULL,
		[User] int NOT NULL CONSTRAINT [FK_TextComment(User)_User(Id)] FOREIGN KEY REFERENCES [dbo].[User](Id),
		[ParentComment] bigint NULL CONSTRAINT [FK_TextComment(ParentComment)_TextComment(Id)] FOREIGN KEY REFERENCES [dbo].[TextComment](Id),
		[PageResource] bigint NOT NULL CONSTRAINT [FK_TextComment(PageResource)_Resource(Id)] FOREIGN KEY REFERENCES [dbo].[Resource](Id)
	)

	CREATE TABLE [dbo].[Snapshot]
	(
		[Id] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_Snapshot(Id)] PRIMARY KEY CLUSTERED,
		[VersionNumber] int NOT NULL,
		[CreateTime] datetime NOT NULL,
		[Project] bigint NOT NULL CONSTRAINT [FK_Snapshot(Project)_Project(Id)] FOREIGN KEY REFERENCES [dbo].[Project](Id),
		[User] int NOT NULL CONSTRAINT [FK_Snapshot(User)_User(Id)] FOREIGN KEY REFERENCES [dbo].[User](Id),
		[Comment] nvarchar(2000)
	)

	CREATE TABLE [dbo].[HistoryLog]
	(
		[Id] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_HistoryLog(Id)] PRIMARY KEY CLUSTERED,
		[Project] bigint NOT NULL CONSTRAINT [FK_HistoryLog(Project)_Project(Id)] FOREIGN KEY REFERENCES [dbo].[Project](Id),
		[User] int NOT NULL CONSTRAINT [FK_HistoryLog(User)_User(Id)] FOREIGN KEY REFERENCES [dbo].[User](Id),
		[CreateTime] datetime NOT NULL,
		[LogType] smallint NOT NULL,
		[Text] nvarchar(2000),
		[DiscussionPost] bigint NOT NULL CONSTRAINT [FK_HistoryLog(DiscussionPost)_DiscussionPost(Id)] FOREIGN KEY REFERENCES [dbo].[DiscussionPost](Id),
		[Snapshot] bigint NOT NULL CONSTRAINT [FK_HistoryLog(Snapshot)_Snapshot(Id)] FOREIGN KEY REFERENCES [dbo].[Snapshot](Id),
		[ResourceVersion] bigint NOT NULL CONSTRAINT [FK_HistoryLog(ResourceVersion)_ResourceVersion(Id)] FOREIGN KEY REFERENCES [dbo].[ResourceVersion](Id)
	)


	CREATE TABLE [dbo].[Snapshot_ResourceVersion]
    (
		[Snapshot] bigint NOT NULL CONSTRAINT [FK_Snapshot_ResourceVersion(Snapshot)_Snapshot(Id)] FOREIGN KEY REFERENCES [dbo].[Snapshot](Id),
		[ResourceVersion] bigint NOT NULL CONSTRAINT [FK_Snapshot_ResourceVersion(ResourceVersion)_ResourceVersion(Id)] FOREIGN KEY REFERENCES [dbo].[ResourceVersion](Id),
		CONSTRAINT [PK_Snapshot_ResourceVersion(Snapshot)_Snapshot_ResourceVersion(ResourceVersion)] PRIMARY KEY ([Snapshot], [ResourceVersion])
    )

	
    INSERT INTO [dbo].[DatabaseVersion]
		(DatabaseVersion)
    VALUES
		( '001' )
		-- DatabaseVersion - varchar

--TODO remove this user insert (used only for development)
INSERT INTO [dbo].[User]
           ([FirstName]
           ,[LastName]
           ,[Email]
           ,[UserName]
           ,[AuthenticationProvider]
           ,[CommunicationToken]
           ,[CommunicationTokenCreateTime]
           ,[PasswordHash]
           ,[CreateTime]
           ,[AvatarUrl])
     VALUES
           ('Josef'
           ,'Novák'
           ,'test@example.com'
           ,'test'
           ,1
           ,'not'
           ,NULL
           ,'not'
           ,'2017-08-21 00:00:00.000'
           ,NULL)

--ROLLBACK
COMMIT 