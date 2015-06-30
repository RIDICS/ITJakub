SET XACT_ABORT ON;
USE ITJakubDB

BEGIN TRAN

    CREATE TABLE [dbo].[BookHeadword]
    (
	   [Id] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_BookHeadword(Id)] PRIMARY KEY CLUSTERED,
	   [BookVersion] bigint NOT NULL FOREIGN KEY REFERENCES [dbo].[BookVersion] (Id),
	   [XmlEntryId] varchar(max) NOT NULL,
	   [DefaultHeadword] varchar(255) NOT NULL,
	   [Headword] varchar(255) NOT NULL,
	   [Visibility] smallint NOT NULL,
    )

    INSERT INTO [dbo].[DatabaseVersion]
		(DatabaseVersion)
    VALUES
		('007' )
		-- DatabaseVersion - varchar

--ROLLBACK
COMMIT 