SET XACT_ABORT ON;
USE ITJakubDB

BEGIN TRAN



    DROP TABLE dbo.Bookmark
    
    CREATE TABLE dbo.Favorites(
	   [Id] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_Favorites(Id)] PRIMARY KEY CLUSTERED,
	   [User] int NOT NULL FOREIGN KEY REFERENCES dbo.[User] (Id),
	   [FavoriteType] [varchar](255) NOT NULL,	   
	   [PageXmlId] varchar(255) NULL,

    )
    


    

    INSERT INTO [dbo].[DatabaseVersion]
		(DatabaseVersion)
	VALUES
		('005' )
		-- DatabaseVersion - varchar

--ROLLBACK
COMMIT