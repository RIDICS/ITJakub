SET XACT_ABORT ON;
USE ITJakubDB

BEGIN TRAN

    CREATE TABLE [dbo].[TokenHyperCharacteristics]
    (
	   [Id] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_TokenHyperCharacteristics(Id)] PRIMARY KEY CLUSTERED,
	   [Value] varchar(255) NOT NULL,
	   [Type] smallint NOT NULL,
    )
    

    CREATE TABLE [dbo].[TokenCharacteristics]
    (
       [Id] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_Characteristics(Id)] PRIMARY KEY CLUSTERED,
	  [Value] varchar(255) NOT NULL,
	  [Type] smallint NOT NULL,
	  [TokenHyperCharacteristics] bigint NULL FOREIGN KEY REFERENCES dbo.TokenHyperCharacteristics(Id)
    )

    

    CREATE TABLE [dbo].[Token]
    (
	   [Id] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_Token(Id)] PRIMARY KEY CLUSTERED,
	   [Token] varchar(255) NOT NULL UNIQUE,  	   	   
	   [TokenCharacteristics] bigint NULL FOREIGN KEY REFERENCES [dbo].[TokenCharacteristics](Id)
    )


    INSERT INTO [dbo].[DatabaseVersion]
		(DatabaseVersion)
    VALUES
		('010' )
		-- DatabaseVersion - varchar

ROLLBACK
--COMMIT 