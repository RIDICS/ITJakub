SET XACT_ABORT ON;
USE ITJakubDB

BEGIN TRAN

    ALTER TABLE [dbo].[Favorites] ADD
	   [Category] int NULL FOREIGN KEY REFERENCES [dbo].[Category] (Id),
	   [XmlEntryId] varchar(max) NULL

    ALTER TABLE [dbo].[BookVersion] ADD
	   [Acronym] varchar(255) NULL

    INSERT INTO [dbo].[DatabaseVersion]
		(DatabaseVersion)
    VALUES
		('008' )
		-- DatabaseVersion - varchar

--ROLLBACK
COMMIT 