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
		CONSTRAINT [UQ_Permission(Group_Book)] UNIQUE ([Group],[Book])    
	);
	
	CREATE TABLE [dbo].[SpecialPermission](
		[Id] int IDENTITY(1,1) NOT NULL CONSTRAINT [PK_SpecialPermission(Id)] PRIMARY KEY,
		[PermissionType] varchar(25) NOT NULL,
		[PermissionCategorization] tinyint NOT NULL,
		[CanUploadBook] bit NULL,
		[CanManagePermissions] bit NULL,
		[CanAddNews] bit NULL,
		[CanManageFeedbacks] bit NULL,
		[CanReadCardFile] bit NULL,
		[CardFileId] varchar(100) NULL,
		[CardFileName] varchar(100) NULL,
		[AutoimportAllowed] bit NULL,
		[AutoimportCategory] int NULL CONSTRAINT [FK_SpecialPermission(AutoimportCategory)_Category(Id)] FOREIGN KEY REFERENCES [dbo].[Category] (Id),
		[CanEditLemmatization] bit NULL,
		[CanReadLemmatization] bit NULL,
		[CanDerivateLemmatization] bit NULL,
		CONSTRAINT [UQ_SpecialPermission(All)] UNIQUE ([PermissionType],[CanUploadBook],[CanManagePermissions],[CanAddNews],[CanManageFeedbacks],[CanReadCardFile],[CardFileId],[CardFileName],[AutoImportAllowed],[AutoimportCategory],[CanEditLemmatization],[CanReadLemmatization],[CanDerivateLemmatization]) 
	);

	CREATE TABLE [dbo].[SpecialPermission_Group](
		[SpecialPermission] int NOT NULL CONSTRAINT [FK_SpecialPermission_Group(SpecialPermission)_SpecialPermission(Id)] FOREIGN KEY REFERENCES [dbo].[SpecialPermission] (Id),
		[Group] int NOT NULL CONSTRAINT [FK_SpecialPermission_Group(Group)_Group(Id)] FOREIGN KEY REFERENCES [dbo].[Group](Id),
		CONSTRAINT [PK_SpecialPermission_Group(SpecialPermission)_SpecialPermission_Group(Group)] PRIMARY KEY ([SpecialPermission], [Group])
	);



	--action permissions
	INSERT INTO dbo.SpecialPermission
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
	    'ManagePermissions', -- PermissionType - varchar
		0, -- PermissionCategorization - tinyint
	    NULL, -- CanUploadBook - bit
	    1, -- CanManagePermissions - bit
	    NULL, -- CanAddNews - bit
	    NULL, -- CanManageFeedbacks - bit
		NULL, -- CanEditLemmatization - bit
		NULL, -- CanReadLemmatization - bit
		NULL -- CanDerivateLemmatization - bit
	),(
		-- Id - int
	    'UploadBook', -- PermissionType - varchar
		0, -- PermissionCategorization - tinyint
	    1, -- CanUploadBook - bit
	    NULL, -- CanManagePermissions - bit
	    NULL, -- CanAddNews - bit
	    NULL, -- CanManageFeedbacks - bit
		NULL, -- CanEditLemmatization - bit
		NULL, -- CanReadLemmatization - bit
		NULL -- CanDerivateLemmatization - bit
	),(
		-- Id - int
	    'News', -- PermissionType - varchar
		0, -- PermissionCategorization - tinyint
	    NULL, -- CanUploadBook - bit
	    NULL, -- CanManagePermissions - bit
	    1, -- CanAddNews - bit
	    NULL, -- CanManageFeedbacks - bit
		NULL, -- CanEditLemmatization - bit
		NULL, -- CanReadLemmatization - bit
		NULL -- CanDerivateLemmatization - bit
	),(
		-- Id - int
	    'Feedback', -- PermissionType - varchar
		0, -- PermissionCategorization - tinyint
	    NULL, -- CanUploadBook - bit
	    NULL, -- CanManagePermissions - bit
	    NULL, -- CanAddNews - bit
	    1, -- CanManageFeedbacks - bit
		NULL, -- CanEditLemmatization - bit
		NULL, -- CanReadLemmatization - bit
		NULL -- CanDerivateLemmatization - bit
	),(
		-- Id - int
	    'EditLemmatization', -- PermissionType - varchar
		0, -- PermissionCategorization - tinyint
	    NULL, -- CanUploadBook - bit
	    NULL, -- CanManagePermissions - bit
	    NULL, -- CanAddNews - bit
	    NULL, -- CanManageFeedbacks - bit
		1, -- CanEditLemmatization - bit
		NULL, -- CanReadLemmatization - bit
		NULL -- CanDerivateLemmatization - bit
	),(
		-- Id - int
	    'ReadLemmatization', -- PermissionType - varchar
		0, -- PermissionCategorization - tinyint
	    NULL, -- CanUploadBook - bit
	    NULL, -- CanManagePermissions - bit
	    NULL, -- CanAddNews - bit
	    NULL, -- CanManageFeedbacks - bit
		NULL, -- CanEditLemmatization - bit
		1, -- CanReadLemmatization - bit
		NULL -- CanDerivateLemmatization - bit
	),(
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

	INSERT INTO dbo.SpecialPermission
	(
	    --Id - this column value is auto-generated
	    PermissionType,
	    PermissionCategorization,
	    AutoimportAllowed,
		AutoimportCategory
	)
	SELECT 'Autoimport', 1, 1, c.Id FROM dbo.Category c

	--cardfiles permissions
	INSERT INTO dbo.SpecialPermission
	(
	    --Id - this column value is auto-generated
	    PermissionType,
	    PermissionCategorization,
	    CanReadCardFile,
	    CardFileId,
	    CardFileName
	)
	VALUES
	(
	    -- Id - int
	    'CardFile', -- PermissionType - varchar
	    2, -- PermissionCategorization - tinyint	    
	    1, -- CanReadCardFile - bit
	    '1', -- CardFileId - varchar
	    'NLA – excerpce' -- CardFileName - varchar
	),(
	    -- Id - int
	    'CardFile', -- PermissionType - varchar
	    2, -- PermissionCategorization - tinyint	    
	    1, -- CanReadCardFile - bit
	    '2', -- CardFileId - varchar
	    'Gebauer – excerpce' -- CardFileName - varchar
	),(
	    -- Id - int
	    'CardFile', -- PermissionType - varchar
	    2, -- PermissionCategorization - tinyint	    
	    1, -- CanReadCardFile - bit
	    '3', -- CardFileId - varchar
	    'Gebauer – prameny' -- CardFileName - varchar
	),(
	    -- Id - int
	    'CardFile', -- PermissionType - varchar
	    2, -- PermissionCategorization - tinyint	    
	    1, -- CanReadCardFile - bit
	    '4', -- CardFileId - varchar
	    'Staročeská excerpce' -- CardFileName - varchar
	),(
	    -- Id - int
	    'CardFile', -- PermissionType - varchar
	    2, -- PermissionCategorization - tinyint	    
	    1, -- CanReadCardFile - bit
	    '5', -- CardFileId - varchar
	    'Zubatý – excerpce' -- CardFileName - varchar
	),(
	    -- Id - int
	    'CardFile', -- PermissionType - varchar
	    2, -- PermissionCategorization - tinyint	    
	    1, -- CanReadCardFile - bit
	    '6', -- CardFileId - varchar
	    'Archiv lidového jazyka' -- CardFileName - varchar
	),(
	    -- Id - int
	    'CardFile', -- PermissionType - varchar
	    2, -- PermissionCategorization - tinyint	    
	    1, -- CanReadCardFile - bit
	    '7', -- CardFileId - varchar
	    'Excerpce textů z 16. století' -- CardFileName - varchar
	),(
	    -- Id - int
	    'CardFile', -- PermissionType - varchar
	    2, -- PermissionCategorization - tinyint	    
	    1, -- CanReadCardFile - bit
	    '8', -- CardFileId - varchar
	    'Latinsko-česká kartotéka' -- CardFileName - varchar
	),(
	    -- Id - int
	    'CardFile', -- PermissionType - varchar
	    2, -- PermissionCategorization - tinyint	    
	    1, -- CanReadCardFile - bit
	    '9', -- CardFileId - varchar
	    'Rukopisy' -- CardFileName - varchar
	),(
	    -- Id - int
	    'CardFile', -- PermissionType - varchar
	    2, -- PermissionCategorization - tinyint	    
	    1, -- CanReadCardFile - bit
	    '10', -- CardFileId - varchar
	    'Justitia' -- CardFileName - varchar
	),(
	    -- Id - int
	    'CardFile', -- PermissionType - varchar
	    2, -- PermissionCategorization - tinyint	    
	    1, -- CanReadCardFile - bit
	    '11', -- CardFileId - varchar
	    'Tyl – excerpce' -- CardFileName - varchar
	),(
	    -- Id - int
	    'CardFile', -- PermissionType - varchar
	    2, -- PermissionCategorization - tinyint	    
	    1, -- CanReadCardFile - bit
	    '12', -- CardFileId - varchar
	    'NLA − prameny' -- CardFileName - varchar
	),(
	    -- Id - int
	    'CardFile', -- PermissionType - varchar
	    2, -- PermissionCategorization - tinyint	    
	    1, -- CanReadCardFile - bit
	    '14', -- CardFileId - varchar
	    'Svoboda' -- CardFileName - varchar
	),(
	    -- Id - int
	    'CardFile', -- PermissionType - varchar
	    2, -- PermissionCategorization - tinyint	    
	    1, -- CanReadCardFile - bit
	    '15', -- CardFileId - varchar
	    'Excerpce pomístních jmen' -- CardFileName - varchar
	),(
	    -- Id - int
	    'CardFile', -- PermissionType - varchar
	    2, -- PermissionCategorization - tinyint	    
	    1, -- CanReadCardFile - bit
	    '16', -- CardFileId - varchar
	    'Tereziánský katastr' -- CardFileName - varchar
	),(
	    -- Id - int
	    'CardFile', -- PermissionType - varchar
	    2, -- PermissionCategorization - tinyint	    
	    1, -- CanReadCardFile - bit
	    '17', -- CardFileId - varchar
	    'Archivy, muzea' -- CardFileName - varchar
	),(
	    -- Id - int
	    'CardFile', -- PermissionType - varchar
	    2, -- PermissionCategorization - tinyint	    
	    1, -- CanReadCardFile - bit
	    '18', -- CardFileId - varchar
	    'Stabilní katastr' -- CardFileName - varchar
	),(
	    -- Id - int
	    'CardFile', -- PermissionType - varchar
	    2, -- PermissionCategorization - tinyint	    
	    1, -- CanReadCardFile - bit
	    '19', -- CardFileId - varchar
	    'Sajtl' -- CardFileName - varchar
	),(
	    -- Id - int
	    'CardFile', -- PermissionType - varchar
	    2, -- PermissionCategorization - tinyint	    
	    1, -- CanReadCardFile - bit
	    '20', -- CardFileId - varchar
	    'Dodatky PSJČ' -- CardFileName - varchar
	),(
	    -- Id - int
	    'CardFile', -- PermissionType - varchar
	    2, -- PermissionCategorization - tinyint	    
	    1, -- CanReadCardFile - bit
	    '21', -- CardFileId - varchar
	    'Grepl - archiv' -- CardFileName - varchar
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
	    'CT:ca22d7b7-e1d6-46b0-a77f-29296fe9f7f0', -- CommunicationToken - varchar
	    '2015-10-01 10:50:36', -- CommunicationTokenCreateTime - datetime
	    'PW:sha1:1000:FhLySoxcL/5CA0RqlRBZMiqblj4sZ0zV:Vocj0I6bhs9bF4p9Nh+Rk7vbCoToulg9', -- PasswordHash - varchar -- password is 'Administrator'
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

	INSERT INTO dbo.SpecialPermission_Group
	(
	    SpecialPermission,
	    [Group]
	)
	SELECT sp.Id, @AdminGroupId FROM dbo.SpecialPermission sp


	ALTER TABLE dbo.[User] DROP COLUMN [Salt]

    INSERT INTO [dbo].[DatabaseVersion]
		 ( DatabaseVersion )
    VALUES
		 ( '019' );
	-- DatabaseVersion - varchar
--ROLLBACK
COMMIT;