SET XACT_ABORT ON;
USE ITJakubDB;

BEGIN TRAN;

	ALTER TABLE [dbo].[SpecialPermission] ADD [CanDerivateLemmatization] bit NULL;
	ALTER TABLE [dbo].[SpecialPermission] ALTER COLUMN [PermissionType] varchar(25) NOT NULL;
	
	ALTER TABLE [dbo].[SpecialPermission] DROP CONSTRAINT [UQ_SpecialPermission(All)];

	ALTER TABLE [dbo].[SpecialPermission] ADD CONSTRAINT [UQ_SpecialPermission(All)] UNIQUE ([PermissionType],[CanUploadBook],[CanManagePermissions],[CanAddNews],[CanManageFeedbacks],[CanReadCardFile],[CardFileId],[CardFileName],[AutoImportAllowed],[AutoimportCategory],[CanEditLemmatization],[CanReadLemmatization],[CanDerivateLemmatization]);
	

    INSERT INTO [dbo].[DatabaseVersion]
		 ( DatabaseVersion )
    VALUES
		 ( '020' );
	-- DatabaseVersion - varchar
--ROLLBACK
COMMIT;