SET XACT_ABORT ON;

BEGIN TRAN

    CREATE TABLE [dbo].[BookHeadword]
    (
	   [Id] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_BookHeadword(Id)] PRIMARY KEY CLUSTERED,
	   [BookVersion] bigint NOT NULL FOREIGN KEY REFERENCES [dbo].[BookVersion] (Id),
	   [XmlEntryId] varchar(255) NOT NULL,
	   [DefaultHeadword] nvarchar(255) NOT NULL,
	   [Headword] nvarchar(255) NOT NULL,
	   [Visibility] smallint NOT NULL,
    )

    INSERT INTO [dbo].[DatabaseVersion]
		(DatabaseVersion)
    VALUES
		('007' )
		-- DatabaseVersion - varchar

--ROLLBACK
COMMIT 