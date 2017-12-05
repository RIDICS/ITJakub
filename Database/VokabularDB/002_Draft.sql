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
           ('Admin' -- FirstName
           ,'Admin' -- LastName
           ,'admin@example.com' -- Email
           ,'Admin' -- UserName
           ,0 -- AuthenticationProvider
           ,'' -- CommunicationToken
           ,NULL -- CommunicationTokenCreateTime
           ,'PW:sha1:1000:FhLySoxcL/5CA0RqlRBZMiqblj4sZ0zV:Vocj0I6bhs9bF4p9Nh+Rk7vbCoToulg9' -- PasswordHash (password is 'Administrator')
           ,'2017-08-21 00:00:00.000' -- CreateTime
           ,NULL) -- AvatarUrl

--ROLLBACK
COMMIT 