SET XACT_ABORT ON;
--USE ITJakubWebDB

ALTER DATABASE CURRENT
SET ALLOW_SNAPSHOT_ISOLATION ON

ALTER DATABASE CURRENT
SET READ_COMMITTED_SNAPSHOT ON WITH ROLLBACK IMMEDIATE



SET XACT_ABORT ON
BEGIN TRAN


	CREATE TABLE [dbo].[DatabaseVersion] 
	(
	   [Id] int IDENTITY(1,1) NOT NULL CONSTRAINT [PK_DatabaseVersion(Id)] PRIMARY KEY CLUSTERED,
	   [DatabaseVersion] varchar(50) NOT NULL,
	   [SolutionVersion] varchar(50) NULL,
	   [UpgradeDate] datetime NOT NULL DEFAULT GETDATE(),
	   [UpgradeUser] varchar(150) NOT NULL default SYSTEM_USER,		
	)

    CREATE TABLE dbo.StaticText
	(
	   [Id] int IDENTITY(1,1) NOT NULL CONSTRAINT [PK_StaticText(Id)] PRIMARY KEY CLUSTERED,
	   [Name] varchar(255) CONSTRAINT [UQ_StaticText(Name)] NOT NULL UNIQUE,
	   [Text] nvarchar(max) NOT NULL,
	   [Format] smallint NOT NULL,
	   [ModificationTime] datetime NULL,
	   [ModificationUser] varchar(255) NULL
    )
        

    INSERT INTO [dbo].[DatabaseVersion]
		(DatabaseVersion)
	VALUES
		('000' )
		-- DatabaseVersion - varchar

--ROLLBACK
COMMIT