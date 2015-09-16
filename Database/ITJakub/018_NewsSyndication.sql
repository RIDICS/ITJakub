SET XACT_ABORT ON;
USE ITJakubDB;

BEGIN TRAN;
  

    CREATE TABLE [dbo].[NewsSyndicationItem]( [Id] BIGINT IDENTITY(1, 1) NOT NULL CONSTRAINT [PK_NewsSyndicationItem(Id)] PRIMARY KEY CLUSTERED,							 
							 [Title] VARCHAR(255) NOT NULL,
							 [CreateDate] DATETIME NOT NULL,
							 [Text] varchar(2000) NOT NULL,
							 [Url] varchar(max) NOT NULL,
							 [User] INT NULL CONSTRAINT [FK_NewsSyndicationItem(User)_User(Id)] FOREIGN KEY REFERENCES [dbo].[User]( Id )
							 );
    INSERT INTO [dbo].[DatabaseVersion]
		 ( DatabaseVersion )
    VALUES( '018' );
    -- DatabaseVersion - varchar
--ROLLBACK
COMMIT;