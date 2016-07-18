SET XACT_ABORT ON;
USE ITJakubDB;

BEGIN TRAN;

	ALTER TABLE [dbo].[SpecialPermission] ADD [CanEditStaticText] bit NULL;
		
	ALTER TABLE [dbo].[SpecialPermission] DROP CONSTRAINT [UQ_SpecialPermission(All)];

	ALTER TABLE [dbo].[SpecialPermission] ADD CONSTRAINT [UQ_SpecialPermission(All)] UNIQUE ([PermissionType],[CanUploadBook],[CanManagePermissions],[CanAddNews],[CanManageFeedbacks],[CanReadCardFile],[CardFileId],[CardFileName],[AutoImportAllowed],[AutoimportCategory],[CanEditLemmatization],[CanReadLemmatization],[CanDerivateLemmatization],[CanEditionPrintText],[CanEditStaticText]);
	
	
    INSERT INTO [dbo].[DatabaseVersion]
		 ( DatabaseVersion )
    VALUES
		 ( '027' );
	-- DatabaseVersion - varchar
--ROLLBACK
COMMIT;