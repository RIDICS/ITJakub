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
		CanDerivateLemmatization,
		CanEditionPrintText,
		CanEditStaticText,
		CanManageRepositoryImport
	)
	VALUES
	(
	    -- Id - int
	    'ManageRepositoryImport', -- PermissionType - varchar
		0, -- PermissionCategorization - tinyint
	    NULL, -- CanUploadBook - bit
	    NULL, -- CanManagePermissions - bit
	    NULL, -- CanAddNews - bit
	    NULL, -- CanManageFeedbacks - bit
		NULL, -- CanEditLemmatization - bit
		NULL, -- CanReadLemmatization - bit
		NULL, -- CanDerivateLemmatization - bit
		NULL, -- CanEditionPrintText - bit
		NULL, -- CanEditStaticText - bit
		1 -- CanManageRepositoryImport - bit
	)

	DECLARE @AdminGroupId INT

	SELECT @AdminGroupId = [Id] FROM [dbo].[UserGroup] WHERE [dbo].[UserGroup].[Name]= 'AdminGroup'

	INSERT INTO dbo.SpecialPermission_UserGroup
	(
	    SpecialPermission,
	    [UserGroup]
	)
	SELECT sp.Id, @AdminGroupId FROM dbo.SpecialPermission sp WHERE sp.PermissionType = 'ManageRepositoryImport'


    INSERT INTO [dbo].[DatabaseVersion]
		(DatabaseVersion)
    VALUES
		('005')
		-- DatabaseVersion - varchar

--ROLLBACK
COMMIT 