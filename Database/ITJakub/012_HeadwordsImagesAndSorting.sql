SET XACT_ABORT ON;
USE ITJakubDB

BEGIN TRAN

    ALTER TABLE [dbo].[BookHeadword] ADD
	   [Image] varchar(100) NULL,
	   [SortOrder] nvarchar(255) NOT NULL

    INSERT INTO [dbo].[DatabaseVersion]
		(DatabaseVersion)
    VALUES
		('012')
		-- DatabaseVersion - varchar

--ROLLBACK
COMMIT 