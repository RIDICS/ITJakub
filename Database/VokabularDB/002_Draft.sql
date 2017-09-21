SET XACT_ABORT ON;

BEGIN TRAN

    CREATE TABLE [dbo].[DiscussionPost]
	(
		[Id] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_DiscussionPost(Id)] PRIMARY KEY CLUSTERED,
		[Text] nvarchar(2000) NOT NULL,
		[CreateTime] datetime NOT NULL,
		[CreatedByUser] int NOT NULL CONSTRAINT [FK_DiscussionPost(CreatedByUser)_User(Id)] FOREIGN KEY REFERENCES [dbo].[User](Id),
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
		[CreatedByUser] int NOT NULL CONSTRAINT [FK_TextComment(CreatedByUser)_User(Id)] FOREIGN KEY REFERENCES [dbo].[User](Id),
		[ParentComment] bigint NULL CONSTRAINT [FK_TextComment(ParentComment)_TextComment(Id)] FOREIGN KEY REFERENCES [dbo].[TextComment](Id),
		[PageResource] bigint NOT NULL CONSTRAINT [FK_TextComment(PageResource)_Resource(Id)] FOREIGN KEY REFERENCES [dbo].[Resource](Id)
	)

	
    INSERT INTO [dbo].[DatabaseVersion]
		(DatabaseVersion)
    VALUES
		( '002' )
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