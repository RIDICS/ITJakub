SET XACT_ABORT ON;
USE ITJakubDB;

BEGIN TRAN;

	ALTER TABLE [dbo].[SpecialPermission] ADD [CanDerivateLemmatization] bit NULL;
	ALTER TABLE [dbo].[SpecialPermission] ALTER COLUMN [PermissionType] varchar(25) NOT NULL;
	
	ALTER TABLE [dbo].[SpecialPermission] DROP CONSTRAINT [UQ_SpecialPermission(All)];

	ALTER TABLE [dbo].[SpecialPermission] ADD CONSTRAINT [UQ_SpecialPermission(All)] UNIQUE ([PermissionType],[CanUploadBook],[CanManagePermissions],[CanAddNews],[CanManageFeedbacks],[CanReadCardFile],[CardFileId],[CardFileName],[AutoImportAllowed],[AutoimportCategory],[CanEditLemmatization],[CanReadLemmatization],[CanDerivateLemmatization]);


	--action permissions
	INSERT INTO [dbo].[SpecialPermission]
	(
	     --Id - this column value is auto-generated
	     PermissionType,
	     PermissionCategorization,
	     CanUploadBook,
	     CanManagePermissions,
	     CanAddNews,
	     CanManageFeedbacks,
		CanEditLemmatization,
		CanReadLemmatization,
		CanDerivateLemmatization
	)
	VALUES
	(
		-- Id - int
	    'DerivateLemmatization', -- PermissionType - varchar
		0, -- PermissionCategorization - tinyint
	     NULL, -- CanUploadBook - bit
	     NULL, -- CanManagePermissions - bit
	     NULL, -- CanAddNews - bit
	     NULL, -- CanManageFeedbacks - bit
		NULL, -- CanEditLemmatization - bit
		NULL, -- CanReadLemmatization - bit
		1 -- CanDerivateLemmatization - bit
	)

	DECLARE @AdminGroupId INT

	SELECT @AdminGroupId = [Id] FROM [dbo].[Group] WHERE [dbo].[Group].[Name]= 'AdminGroup'

	INSERT INTO [dbo].[SpecialPermission_Group]
	(
	    SpecialPermission,
	    [Group]
	)
	SELECT sp.Id, @AdminGroupId FROM dbo.SpecialPermission sp WHERE sp.PermissionType = 'DerivateLemmatization'


    INSERT INTO [dbo].[DatabaseVersion]
		 ( DatabaseVersion )
    VALUES
		 ( '020' );
	-- DatabaseVersion - varchar
--ROLLBACK
COMMIT;