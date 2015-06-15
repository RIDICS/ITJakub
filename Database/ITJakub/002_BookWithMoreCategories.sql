SET XACT_ABORT ON;
USE ITJakubDB

BEGIN TRAN

    ALTER TABLE [dbo].[Book] DROP CONSTRAINT [FK_Book(Category)_Category(Id)]
    ALTER TABLE [dbo].[Book] DROP COLUMN [Category]
    ALTER TABLE [dbo].[Book] DROP CONSTRAINT [FK_Book(BookType)_BookType(Id)]
    ALTER TABLE [dbo].[Book] DROP COLUMN [BookType]

    ALTER TABLE [dbo].[BookVersion] ADD [DefaultBookType] int NULL CONSTRAINT [FK_BookVersion(BookType)_BookType(Id)] FOREIGN KEY REFERENCES [dbo].[BookType](Id)

    CREATE TABLE [dbo].[BookVersion_Category]
    (
	   [Category] int NOT NULL CONSTRAINT [FK_BookVersion_Category(Category)_Category(Id)] FOREIGN KEY REFERENCES [dbo].[Category] (Id),
	   [BookVersion] bigint NOT NULL CONSTRAINT [FK_BookVersion_Category(Book)_Book(Id)] FOREIGN KEY REFERENCES [dbo].[BookVersion](Id),
	   CONSTRAINT [PK_BookVersion_Category(Category)_BookVersion_Category(Book)] PRIMARY KEY ([Category], [BookVersion])
    )

    INSERT INTO [dbo].[DatabaseVersion]
		(DatabaseVersion)
	VALUES
		('002' )
		-- DatabaseVersion - varchar


	--ROLLBACK
COMMIT