SET XACT_ABORT ON;
USE ITJakubDB;

BEGIN TRAN;
	INSERT INTO [dbo].[SpecialPermission]
		 ( PermissionType, PermissionCategorization, CanEditionPrintText )
	VALUES
		 ( 'EditionPrintText', 0, 1 );



	DECLARE @AdminGroupId INT

	SELECT @AdminGroupId = [Id] FROM [dbo].[Group] WHERE [dbo].[Group].[Name]= 'AdminGroup'

	INSERT INTO [dbo].[SpecialPermission_Group]
	(
	    SpecialPermission,
	    [Group]
	)
	SELECT sp.Id, @AdminGroupId FROM dbo.SpecialPermission sp WHERE sp.PermissionType = 'EditionPrintText'



	INSERT INTO [dbo].[DatabaseVersion]
		 ( DatabaseVersion )
	VALUES
		 ( '025' );
	-- DatabaseVersion - varchar
--ROLLBACK
COMMIT;
