USE [ITJakubMobileAppsDB]
SET XACT_ABORT ON
BEGIN TRAN

    INSERT INTO [dbo].[Application]
			([Name])
	    VALUES
			('Fillwords2')


    INSERT INTO [dbo].[DatabaseVersion]
		    (DatabaseVersion)
	    VALUES
		    ('006' )
		    -- DatabaseVersion - varchar

--ROLLBACK
COMMIT