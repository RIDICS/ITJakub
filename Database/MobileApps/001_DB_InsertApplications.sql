USE [ITJakubMobileAppsDB]
SET XACT_ABORT ON
BEGIN TRAN

INSERT INTO [dbo].[Application]
           ([Name])
     VALUES
           ('Chat'), ('Hangman'), ('Crosswords'), ('SynchronizedReading'), ('Fillwords')


INSERT INTO [dbo].[DatabaseVersion]
		(DatabaseVersion)
	VALUES
		('001' )
		-- DatabaseVersion - varchar

--ROLLBACK
COMMIT