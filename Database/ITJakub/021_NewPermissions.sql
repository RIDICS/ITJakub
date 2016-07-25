SET XACT_ABORT ON;

BEGIN TRAN;

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
		 ( '021' );
	-- DatabaseVersion - varchar
--ROLLBACK
COMMIT;