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

	INSERT INTO [dbo].[DatabaseVersion]
		(DatabaseVersion)
    VALUES
		( '000' )
		-- DatabaseVersion - varchar

--ROLLBACK
COMMIT 