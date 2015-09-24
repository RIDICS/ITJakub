USE [ITJakubMobileAppsDB]
SET XACT_ABORT ON
BEGIN TRAN

    ALTER TABLE [dbo].[Task]
    ADD [Description] varchar(MAX) NOT NULL


    INSERT INTO [dbo].[DatabaseVersion]
		    (DatabaseVersion)
	    VALUES
		    ('004' )
		    -- DatabaseVersion - varchar

--ROLLBACK
COMMIT