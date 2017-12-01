SET XACT_ABORT ON;

BEGIN TRAN

	INSERT INTO [dbo].[DatabaseVersion]
		(DatabaseVersion)
    VALUES
		( '002' )
		-- DatabaseVersion - varchar

	CREATE TABLE [dbo].[EditionNoteResource]
	(
	   [ResourceVersionId] bigint NOT NULL CONSTRAINT [PK_EditionNoteResource(ResourceVersionId)] PRIMARY KEY CLUSTERED FOREIGN KEY REFERENCES [dbo].[ResourceVersion] (Id),
	   [ExternalId] varchar(100) NULL,
	   [BookVersion] bigint NULL CONSTRAINT [FK_EditionNoteResource(BookVersion)_BookVersionResource(ResourceVersionId)] FOREIGN KEY REFERENCES [dbo].[BookVersionResource](ResourceVersionId)
	)

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