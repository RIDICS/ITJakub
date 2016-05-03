SET XACT_ABORT ON;
USE ITJakubDB;

BEGIN TRAN;
	ALTER TABLE [dbo].[ManuscriptDescription]
	   ALTER COLUMN Idno VARCHAR(100) NULL;
	ALTER TABLE [dbo].[ManuscriptDescription]
	   ALTER COLUMN Repository VARCHAR(200) NULL;

	INSERT INTO [dbo].[DatabaseVersion]
		 ( DatabaseVersion )
	VALUES
		 ( '026' );
	-- DatabaseVersion - varchar
--ROLLBACK
COMMIT;
