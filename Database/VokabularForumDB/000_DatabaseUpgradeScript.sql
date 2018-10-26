SET XACT_ABORT ON;
--USE VokabularForumDB

ALTER DATABASE CURRENT
SET ALLOW_SNAPSHOT_ISOLATION ON

ALTER DATABASE CURRENT
SET READ_COMMITTED_SNAPSHOT ON WITH ROLLBACK IMMEDIATE

SET XACT_ABORT ON
BEGIN TRAN


-- Create table for storing database version
	CREATE TABLE [dbo].[DatabaseVersion]
	(
	   [Id] int IDENTITY(1,1) NOT NULL CONSTRAINT [PK_DatabaseVersion(Id)] PRIMARY KEY CLUSTERED,
	   [DatabaseVersion] varchar(50) NOT NULL,
	   [SolutionVersion] varchar(50) NULL,
	   [UpgradeDate] datetime NOT NULL DEFAULT GETDATE(),
	   [UpgradeUser] varchar(150) NOT NULL default SYSTEM_USER,
	)

-- Add column for mapping Category (Forum) - BookType (Vokabular)
	ALTER TABLE [dbo].[yaf_Category] ADD ExternalID SMALLINT NULL

-- Add column for mapping Forum (Forum) - Category (Vokabular)
	ALTER TABLE [dbo].[yaf_Forum] ADD ExternalID INT NULL

-- Add column for mapping Forum (Forum) - Project (Vokabular)
	ALTER TABLE [dbo].[yaf_Forum] ADD ExternalProjectID BIGINT NULL

	ALTER TABLE [dbo].[yaf_UserProfile] ADD VokabularID NVARCHAR(255) NULL

-- Insert version number to DatabaseVersion table

    INSERT INTO [dbo].[DatabaseVersion]
		(DatabaseVersion)
	VALUES
		('000')
		-- DatabaseVersion - varchar

	--ROLLBACK
COMMIT