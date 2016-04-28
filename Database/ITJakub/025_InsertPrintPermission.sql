SET XACT_ABORT ON;
USE ITJakubDB;

BEGIN TRAN;
	INSERT INTO [dbo].[SpecialPermission]
		 ( PermissionType, PermissionCategorization, CanEditionPrintText )
	VALUES
		 ( 'EditionPrintText', 0, 1 );

	INSERT INTO [dbo].[DatabaseVersion]
		 ( DatabaseVersion )
	VALUES
		 ( '025' );
	-- DatabaseVersion - varchar
--ROLLBACK
COMMIT;
