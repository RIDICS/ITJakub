SET XACT_ABORT ON;

BEGIN TRAN

    ALTER TABLE [dbo].[Project] ADD ForumID INT NULL

    INSERT INTO [dbo].[DatabaseVersion]
		(DatabaseVersion)
    VALUES
		('004')
		-- DatabaseVersion - varchar

--ROLLBACK
COMMIT