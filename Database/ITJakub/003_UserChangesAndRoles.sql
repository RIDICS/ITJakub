
BEGIN TRAN

	ALTER TABLE [dbo].[User] ADD [UserName] varchar(50) NOT NULL UNIQUE

	ALTER TABLE [dbo].[User] ALTER COLUMN [FirstName] varchar(50) NULL

	ALTER TABLE [dbo].[User] ALTER COLUMN [LastName] varchar(50) NULL

	ALTER TABLE [dbo].[User] ADD [ExternalId] bigint NOT NUll
	

    INSERT INTO [dbo].[DatabaseVersion]
		(DatabaseVersion)
	VALUES
		('003' )
		-- DatabaseVersion - varchar


	--ROLLBACK
COMMIT