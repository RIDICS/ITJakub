USE [ITJakubMobileAppsDB]
SET XACT_ABORT ON
BEGIN TRAN

ALTER TABLE [dbo].[Group] DROP COLUMN [IsActive]

ALTER TABLE [dbo].[Group] ADD [State] smallint NOT NULL DEFAULT 0

INSERT INTO [dbo].[DatabaseVersion]
		(DatabaseVersion)
	VALUES
		('002' )
		-- DatabaseVersion - varchar

--ROLLBACK
COMMIT