SET XACT_ABORT ON;

BEGIN TRAN

	INSERT INTO [dbo].[ExternalResourceType]
           ([Name])
     VALUES
           ('OaiPmh')

	INSERT INTO [dbo].[ParserType]
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