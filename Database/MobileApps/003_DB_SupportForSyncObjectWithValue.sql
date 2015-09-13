USE [ITJakubMobileAppsDB]
SET XACT_ABORT ON
BEGIN TRAN

ALTER TABLE [dbo].[SynchronizedObject]
ADD [ObjectValue] varchar(MAX) NULL,
	[SynchronizedObjectType] tinyint NOT NULL

-- Require testing
ALTER TABLE [dbo].[SynchronizedObject]
	ALTER COLUMN [RowKey] varchar(50) NULL

INSERT INTO [dbo].[DatabaseVersion]
		(DatabaseVersion)
	VALUES
		('003' )
		-- DatabaseVersion - varchar

--ROLLBACK
COMMIT