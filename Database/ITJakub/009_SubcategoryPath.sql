SET XACT_ABORT ON;

BEGIN TRAN

    ALTER TABLE [dbo].[Category] ADD
	   [Path] varchar(255) NOT NULL

    INSERT INTO [dbo].[DatabaseVersion]
		(DatabaseVersion)
    VALUES
		('009' )
		-- DatabaseVersion - varchar

--ROLLBACK
COMMIT 