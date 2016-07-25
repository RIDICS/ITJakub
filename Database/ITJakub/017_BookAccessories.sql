SET XACT_ABORT ON;

BEGIN TRAN;

    CREATE TABLE [dbo].[Accessory]
    (
	   [Id] bigint NOT NULL IDENTITY(1,1) PRIMARY KEY,
	   [FileName] varchar(255) NOT NULL,
	   [Type] smallint NOT NULL,
	   [BookVersion] bigint NULL CONSTRAINT[FK_Accessory(BookVersion)_BookVersion(Id)] FOREIGN KEY REFERENCES [dbo].[BookVersion](Id)
    )    
    

    CREATE TABLE [dbo].[TermCategory]
    (
	   [Id] int NOT NULL IDENTITY(1,1) PRIMARY KEY,
	   [Name] varchar(255) NOT NULL  CONSTRAINT [UQ_TermCategory] UNIQUE
    )

    ALTER TABLE [dbo].[Term] ADD
    [TermCategory] int NULL CONSTRAINT [FK_Term(TermCategory)_TermCategory(Id)] FOREIGN KEY REFERENCES [dbo].[TermCategory](Id)

    INSERT INTO [dbo].[DatabaseVersion]
		 ( DatabaseVersion )
    VALUES( '017' );
    -- DatabaseVersion - varchar
--ROLLBACK
COMMIT;