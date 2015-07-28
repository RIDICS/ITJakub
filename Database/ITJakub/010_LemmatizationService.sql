SET XACT_ABORT ON;
USE ITJakubDB

BEGIN TRAN

    CREATE TABLE [dbo].[HyperCanonicalForm]
    (
	   [Id] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_HyperCanonicalForm(Id)] PRIMARY KEY CLUSTERED,
	   [Value] varchar(255) NOT NULL,
	   [Type] smallint NOT NULL,
    )
        

    CREATE TABLE [dbo].[Token]
    (
	   [Id] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_Token(Id)] PRIMARY KEY CLUSTERED,
	   [Text] varchar(255) NOT NULL UNIQUE,
	   [Description] varchar(255) NOT NULL
    )    

    CREATE TABLE [dbo].[TokenCharacteristic]
    (
       [Id] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_TokenCharacteristic(Id)] PRIMARY KEY CLUSTERED,
	  [Token] bigint NULL FOREIGN KEY REFERENCES [dbo].[Token](Id),
	  [MorphologicalCharakteristic] nvarchar(16) NOT NULL,
	  [Description] varchar(255) NOT NULL
    )

    

    CREATE TABLE [dbo].[CanonicalForm]
    (
	   [Id] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_CanonicalForm(Id)] PRIMARY KEY CLUSTERED,
	   [Text] varchar(255) NOT NULL,
	   [Type] smallint NOT NULL,	   
	   [Description] varchar(255) NOT NULL,
	   [HyperCanonicalForm] BIGINT NULL FOREIGN KEY REFERENCES [dbo].HyperCanonicalForm(Id)
    )


    CREATE TABLE [dbo].[CanonicalForm_TokenCharacteristic]
    (
	   [Id] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_CanonicalForm_TokenCharacteristic(Id)] PRIMARY KEY CLUSTERED,
	   [TokenCharacteristic] BIGINT NOT NULL FOREIGN KEY REFERENCES [dbo].[TokenCharacteristic](Id),
	   [CanonicalForm] bigint NOT NULL FOREIGN KEY REFERENCES [dbo].CanonicalForm(Id)
    )
    

    INSERT INTO [dbo].[DatabaseVersion]
		(DatabaseVersion)
    VALUES
		('010' )
		-- DatabaseVersion - varchar

--ROLLBACK
COMMIT 