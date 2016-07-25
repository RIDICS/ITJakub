SET XACT_ABORT ON;

BEGIN TRAN;
    ALTER TABLE [dbo].[SpecialPermission] ADD CanEditionPrintText bit NULL;

    INSERT INTO [dbo].[DatabaseVersion]
		 ( DatabaseVersion )
	VALUES
		 ( '024' );
COMMIT;

