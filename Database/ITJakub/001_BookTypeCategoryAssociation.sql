
USE ITJakubDB

BEGIN TRAN

     ALTER TABLE [dbo].[Category] ADD [BookType] int NULL CONSTRAINT [FK_Category(BookType)_BookType(Id)] FOREIGN KEY REFERENCES [dbo].[BookType](Id)

    INSERT INTO [dbo].[DatabaseVersion]
		(DatabaseVersion)
	VALUES
		('001' )
		-- DatabaseVersion - varchar


	--ROLLBACK
COMMIT