SET XACT_ABORT ON;

BEGIN TRAN

	ALTER TABLE [dbo].[Book] ADD [LastVersion] bigint NULL CONSTRAINT [FK_Book(LastVersion)_BookVersion(Id)] FOREIGN KEY REFERENCES [dbo].[BookVersion](Id)

     ALTER TABLE [dbo].[Category] ADD [BookType] int NULL CONSTRAINT [FK_Category(BookType)_BookType(Id)] FOREIGN KEY REFERENCES [dbo].[BookType](Id)

    INSERT INTO [dbo].[DatabaseVersion]
		(DatabaseVersion)
	VALUES
		('001' )
		-- DatabaseVersion - varchar


	--ROLLBACK
COMMIT