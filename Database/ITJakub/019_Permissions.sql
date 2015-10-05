SET XACT_ABORT ON;
USE ITJakubDB;

BEGIN TRAN;

	CREATE TABLE [dbo].[Group](
		[Id] int IDENTITY(1,1) NOT NULL CONSTRAINT [PK_Group(Id)] PRIMARY KEY,
		[Name] varchar(255) NOT NULL UNIQUE,
		[Description] varchar(500) NULL,
		[CreateTime] datetime NOT NULL,
		[CreatedBy] int NULL CONSTRAINT [FK_Group(CreatedBy)_User(Id)] FOREIGN KEY REFERENCES [dbo].[User](Id)
	);

	CREATE TABLE [dbo].[User_Group](
		[User] int NOT NULL CONSTRAINT [FK_User_Group(User)_User(Id)] FOREIGN KEY REFERENCES [dbo].[User] (Id),
		[Group] int NOT NULL CONSTRAINT [FK_User_Group(Group)_Group(Id)] FOREIGN KEY REFERENCES [dbo].[Group](Id),
		CONSTRAINT [PK_User_Group(User)_User_Group(Group)] PRIMARY KEY ([User], [Group])
	);

	CREATE TABLE [dbo].[Permission](
		[Id] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_Permission(Id)] PRIMARY KEY,
		[Group] int NOT NULL CONSTRAINT [FK_Permission(Group)_Group(Id)] FOREIGN KEY REFERENCES [dbo].[Group](Id),
		[Book] bigint NOT NULL CONSTRAINT [FK_Permission(Book)_Book(Id)] FOREIGN KEY REFERENCES [dbo].[Book](Id),
		CONSTRAINT [Uniq_Permission(Group_Book)] UNIQUE ([Group],[Book])    
	);
	
	CREATE TABLE [dbo].[SpecialPermission](
		[Id] int IDENTITY(1,1) NOT NULL CONSTRAINT [PK_SpecialPermission(Id)] PRIMARY KEY,
		[PermissionType] varchar(20) NOT NULL,
		[CanUploadBook] bit NULL,
		[CanManagePermissions] bit NULL,
		[CanAddNews] bit NULL,
		[CanManageFeedbacks] bit NULL,
		CONSTRAINT [Uniq_SpecialPermission(PermissionType_CanUploadBook_CanManagePermissions_CanAddNews_CanManageFeedbacks)] UNIQUE ([PermissionType],[CanUploadBook],[CanManagePermissions],[CanAddNews],[CanManageFeedbacks])    
	);

	CREATE TABLE [dbo].[SpecialPermission_Group](
		[SpecialPermission] int NOT NULL CONSTRAINT [FK_SpecialPermission_Group(SpecialPermission)_SpecialPermission(Id)] FOREIGN KEY REFERENCES [dbo].[SpecialPermission] (Id),
		[Group] int NOT NULL CONSTRAINT [FK_SpecialPermission_Group(Group)_Group(Id)] FOREIGN KEY REFERENCES [dbo].[Group](Id),
		CONSTRAINT [PK_SpecialPermission_Group(SpecialPermission)_SpecialPermission_Group(Group)] PRIMARY KEY ([SpecialPermission], [Group])
	);




	INSERT INTO dbo.SpecialPermission
	(
	    --Id - this column value is auto-generated
	    PermissionType,
	    CanUploadBook,
	    CanManagePermissions,
	    CanAddNews,
	    CanManageFeedbacks
	)
	VALUES
	(
	    -- Id - int
	    'ManagePermissions', -- PermissionType - varchar
	    NULL, -- CanUploadBook - bit
	    1, -- CanManagePermissions - bit
	    NULL, -- CanAddNews - bit
	    NULL -- CanManageFeedbacks - bit
	),(
		-- Id - int
	    'UploadBook', -- PermissionType - varchar
	    1, -- CanUploadBook - bit
	    NULL, -- CanManagePermissions - bit
	    NULL, -- CanAddNews - bit
	    NULL -- CanManageFeedbacks - bit
	),(
		-- Id - int
	    'News', -- PermissionType - varchar
	    NULL, -- CanUploadBook - bit
	    NULL, -- CanManagePermissions - bit
	    1, -- CanAddNews - bit
	    NULL -- CanManageFeedbacks - bit
	),(
		-- Id - int
	    'Feedback', -- PermissionType - varchar
	    NULL, -- CanUploadBook - bit
	    NULL, -- CanManagePermissions - bit
	    NULL, -- CanAddNews - bit
	    1 -- CanManageFeedbacks - bit
	)



	INSERT INTO dbo.[User]
	(
	    --Id - this column value is auto-generated
	    FirstName,
	    LastName,
	    Email,
	    AuthenticationProvider,
	    CommunicationToken,
	    CommunicationTokenCreateTime,
	    PasswordHash,
	    Salt,
	    CreateTime,
	    AvatarUrl,
	    UserName
	)
	VALUES
	(
	    -- Id - int
	    'Admin', -- FirstName - varchar
	    'Admin', -- LastName - varchar
	    'Admin', -- Email - varchar
	    0, -- AuthenticationProvider - tinyint
	    'e61edc70-9c8f-4ef4-84ff-f57b3758b88f', -- CommunicationToken - varchar
	    '2015-10-01 10:50:36', -- CommunicationTokenCreateTime - datetime
	    'ANx75Iw7AnQgKChYKghJVcXKE8vwofGlP3tRctamVrqTLOvhyXM0Qko27aui6mhTlg==', -- PasswordHash - varchar -- password is 'Administrator'
	    '', -- Salt - varchar
	    '2015-10-01 10:50:36', -- CreateTime - datetime
	    NULL, -- AvatarUrl - varchar
	    'Admin' -- UserName - varchar
	)

	DECLARE @AdminUserId INT

	SELECT @AdminUserId = [Id] FROM [dbo].[User] WHERE [dbo].[User].[UserName]= 'Admin'

	INSERT INTO dbo.[Group]
	(
	    --Id - this column value is auto-generated
	    Name,
	    Description,
	    CreateTime,
	    CreatedBy
	)
	VALUES
	(
	    -- Id - int
	    'AdminGroup', -- Name - varchar
	    'Group for administrators', -- Description - varchar
	    '2015-10-01 10:52:35', -- CreateTime - datetime
	     @AdminUserId -- CreatedBy - int
	)

	DECLARE @AdminGroupId INT

	SELECT @AdminGroupId = [Id] FROM [dbo].[Group] WHERE [dbo].[Group].[Name]= 'AdminGroup'

	INSERT INTO dbo.User_Group
	(
	    [User],
	    [Group]
	)
	VALUES
	(
	    @AdminUserId, -- User - int
	    @AdminGroupId -- Group - int
	)

	DECLARE @ManagePermissionId INT

	SELECT @ManagePermissionId = [Id] FROM [dbo].[SpecialPermission] WHERE [dbo].[SpecialPermission].[PermissionType] = 'ManagePermissions'

	INSERT INTO dbo.SpecialPermission_Group
	(
	    SpecialPermission,
	    [Group]
	)
	VALUES
	(
	    @ManagePermissionId, -- SpecialPermission - int
	    @AdminGroupId -- Group - int
	)


	ALTER TABLE dbo.[User] DROP COLUMN [Salt]

    INSERT INTO [dbo].[DatabaseVersion]
		 ( DatabaseVersion )
    VALUES
		 ( '019' );
	-- DatabaseVersion - varchar
--ROLLBACK
COMMIT;