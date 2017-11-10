SET XACT_ABORT ON;

BEGIN TRAN

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