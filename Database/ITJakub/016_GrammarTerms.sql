SET XACT_ABORT ON;
USE ITJakubDB;

BEGIN TRAN;

    CREATE TABLE [dbo].[Term]
    (
	   [Id] int NOT NULL IDENTITY(1,1) PRIMARY KEY,
	   [XmlId] varchar(255) NOT NULL CONSTRAINT[UQ_Term(XmlId)] UNIQUE,
	   [Text] varchar(255) NOT NULL,
	   [Position] bigint NOT NULL	   
    )

    CREATE TABLE [dbo].[BookPage_Term](
	   [Id] bigint NOT NULL IDENTITY(1,1) PRIMARY KEY,
	   [BookPage] bigint NOT NULL CONSTRAINT [PK_BookPage_Term(BookPage)_BookPage(Id)] FOREIGN KEY REFERENCES [dbo].[BookPage](Id),
	   [Term] int NOT NULL CONSTRAINT [PK_BookPage_Term(Term)_Term(Id)] FOREIGN KEY REFERENCES [dbo].[Term](Id)
    )
    

    INSERT INTO [dbo].[DatabaseVersion]
		 ( DatabaseVersion )
    VALUES( '016' );
    -- DatabaseVersion - varchar
--ROLLBACK
COMMIT;