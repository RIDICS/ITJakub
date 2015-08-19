SET XACT_ABORT ON;
USE ITJakubDB;

BEGIN TRAN;
    
	 CREATE TABLE [dbo].[Track]([Id] BIGINT IDENTITY(1, 1)	NOT NULL  CONSTRAINT [PK_Track(Id)] PRIMARY KEY CLUSTERED,
								[Name] varchar(255) NOT NULL,
								[Position] SMALLINT NOT NULL,
								[BookVersion] bigint NOT NULL CONSTRAINT [FK_Track(BookVersion)_BookVersion(Id)] FOREIGN KEY REFERENCES [dbo].[BookVersion](Id)
								);


	CREATE TABLE [dbo].[Recording](
		[Id] bigint IDENTITY(1,1) NOT null CONSTRAINT [PK_Recording(Id)] PRIMARY KEY CLUSTERED,
		[Length] bigint NOT NULL,		
		[FileName] varchar(255) NOT NULL,
		[AudioType] tinyint NOT NULL,
		[MimeType] varchar(255) NULL,
		[Track] bigint NOT NULL CONSTRAINT [FK_Recording(Track)_Track(Id)] FOREIGN KEY REFERENCES [dbo].[Track](Id)
	);
	

    INSERT INTO [dbo].[DatabaseVersion]
		 ( DatabaseVersion
		 )
    VALUES( '014' );
    -- DatabaseVersion - varchar
--ROLLBACK
COMMIT;