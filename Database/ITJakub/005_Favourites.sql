SET XACT_ABORT ON;
USE ITJakubDB

BEGIN TRAN



    DROP TABLE dbo.Bookmark
    
    CREATE TABLE dbo.Favorites(
	   [Id] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_BookContentItem(Id)] PRIMARY KEY CLUSTERED,

    )
















    

    INSERT INTO [dbo].[DatabaseVersion]
		(DatabaseVersion)
	VALUES
		('005' )
		-- DatabaseVersion - varchar

ROLLBACK
--COMMIT