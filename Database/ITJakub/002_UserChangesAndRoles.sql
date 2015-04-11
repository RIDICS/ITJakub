
USE ITJakubDB

BEGIN TRAN

	ALTER TABLE [dbo].[User] ADD [UserName] varchar(50) NOT NULL UNIQUE

    INSERT INTO [dbo].[DatabaseVersion]
		(DatabaseVersion)
	VALUES
		('002' )
		-- DatabaseVersion - varchar


	--ROLLBACK
COMMIT