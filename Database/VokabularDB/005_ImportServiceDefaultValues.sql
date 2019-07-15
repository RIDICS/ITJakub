SET XACT_ABORT ON;

BEGIN TRAN

	INSERT INTO [dbo].[ExternalRepositoryType]
           ([Name])
     VALUES
           ('OaiPmh')

	INSERT INTO [dbo].[BibliographicFormat]
           ([Name])
     VALUES
           ('Marc21')



    INSERT INTO [dbo].[DatabaseVersion]
		(DatabaseVersion)
    VALUES
		('005')
		-- DatabaseVersion - varchar

--ROLLBACK
COMMIT 