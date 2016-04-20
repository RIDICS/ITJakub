SET XACT_ABORT ON;
USE ITJakubDB;

BEGIN TRAN;

	CREATE TABLE [dbo].[LiteraryGenre]
    (
	   [Id] int IDENTITY(1,1) NOT NULL CONSTRAINT [PK_LiteraryGenre(Id)] PRIMARY KEY CLUSTERED,
	   [Name] varchar(255) CONSTRAINT [UQ_LiteraryGenre(Name)] NOT NULL UNIQUE
    )

    CREATE TABLE [dbo].[BookVersion_LiteraryGenre]
    (
	   [LiteraryGenre] int NOT NULL CONSTRAINT [FK_BookVersion_LiteraryGenre(LiteraryGenre)_LiteraryGenre(Id)] FOREIGN KEY REFERENCES [dbo].[LiteraryGenre] (Id),
	   [BookVersion] bigint NOT NULL CONSTRAINT [FK_BookVersion_LiteraryGenre(Book)_Book(Id)] FOREIGN KEY REFERENCES [dbo].[BookVersion](Id),
	   CONSTRAINT [PK_BookVersion_LiteraryGenre(LiteraryGenre)_BookVersion_LiteraryGenre(Book)] PRIMARY KEY ([LiteraryGenre], [BookVersion])
    )

    CREATE TABLE [dbo].[LiteraryKind]
    (
	   [Id] int IDENTITY(1,1) NOT NULL CONSTRAINT [PK_LiteraryKind(Id)] PRIMARY KEY CLUSTERED,
	   [Name] varchar(255) CONSTRAINT [UQ_LiteraryKind(Name)] NOT NULL UNIQUE
    )

    CREATE TABLE [dbo].[BookVersion_LiteraryKind]
    (
	   [LiteraryKind] int NOT NULL CONSTRAINT [FK_BookVersion_LiteraryKind(LiteraryKind)_LiteraryKind(Id)] FOREIGN KEY REFERENCES [dbo].[LiteraryKind] (Id),
	   [BookVersion] bigint NOT NULL CONSTRAINT [FK_BookVersion_LiteraryKind(Book)_Book(Id)] FOREIGN KEY REFERENCES [dbo].[BookVersion](Id),
	   CONSTRAINT [PK_BookVersion_LiteraryKind(LiteraryKind)_BookVersion_LiteraryKind(Book)] PRIMARY KEY ([LiteraryKind], [BookVersion])
    )

    CREATE TABLE [dbo].[LiteraryOriginal]
    (
	   [Id] int IDENTITY(1,1) NOT NULL CONSTRAINT [PK_LiteraryOriginal(Id)] PRIMARY KEY CLUSTERED,
	   [Name] varchar(255) CONSTRAINT [UQ_LiteraryOriginal(Name)] NOT NULL UNIQUE
    )

    CREATE TABLE [dbo].[BookVersion_LiteraryOriginal]
    (
	   [LiteraryOriginal] int NOT NULL CONSTRAINT [FK_BookVersion_LiteraryKind(LiteraryOriginal)_LiteraryOriginal(Id)] FOREIGN KEY REFERENCES [dbo].[LiteraryOriginal] (Id),
	   [BookVersion] bigint NOT NULL CONSTRAINT [FK_BookVersion_LiteraryOriginal(Book)_Book(Id)] FOREIGN KEY REFERENCES [dbo].[BookVersion](Id),
	   CONSTRAINT [PK_BookVersion_LiteraryOriginal(LiteraryOriginal)_BookVersion_LiteraryOriginal(Book)] PRIMARY KEY ([LiteraryOriginal], [BookVersion])
    )

    INSERT INTO [dbo].[DatabaseVersion]
		 ( DatabaseVersion )
    VALUES
		 ( '022' );
	-- DatabaseVersion - varchar
--ROLLBACK
COMMIT;