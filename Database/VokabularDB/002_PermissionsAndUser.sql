SET XACT_ABORT ON;

BEGIN TRAN;

	CREATE TABLE [dbo].[UserGroup](
		[Id] int IDENTITY(1,1) NOT NULL CONSTRAINT [PK_UserGroup(Id)] PRIMARY KEY,
		[Name] varchar(255) NOT NULL UNIQUE,
		[Description] varchar(500) NULL,
		[CreateTime] datetime NOT NULL,
		[CreatedBy] int NULL CONSTRAINT [FK_UserGroup(CreatedBy)_User(Id)] FOREIGN KEY REFERENCES [dbo].[User](Id),
		[ExternalId] int NULL,
	);

	CREATE TABLE [dbo].[User_UserGroup](
		[User] int NOT NULL CONSTRAINT [FK_User_UserGroup(User)_User(Id)] FOREIGN KEY REFERENCES [dbo].[User] (Id),
		[UserGroup] int NOT NULL CONSTRAINT [FK_User_UserGroup(UserGroup)_UserGroup(Id)] FOREIGN KEY REFERENCES [dbo].[UserGroup](Id),
		CONSTRAINT [PK_User_UserGroup(User)_User_UserGroup(UserGroup)] PRIMARY KEY ([User], [UserGroup])
	);

	CREATE TABLE [dbo].[Permission](
		[Id] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_Permission(Id)] PRIMARY KEY,
		[UserGroup] int NOT NULL CONSTRAINT [FK_Permission(UserGroup)_UserGroup(Id)] FOREIGN KEY REFERENCES [dbo].[UserGroup](Id),
		[Project] bigint NOT NULL CONSTRAINT [FK_Permission(Project)_Project(Id)] FOREIGN KEY REFERENCES [dbo].[Project](Id),
		CONSTRAINT [UQ_Permission(UserGroup_Project)] UNIQUE ([UserGroup],[Project])
	);
	
	DECLARE @AdminUserId INT;

	INSERT INTO [dbo].[User] ([CreateTime]
           ,[AvatarUrl])
     VALUES
           ('2017-08-21 00:00:00.000' -- CreateTime
           ,NULL) -- AvatarUrl
		  
	SET @AdminUserId = SCOPE_IDENTITY();

	INSERT INTO dbo.[UserGroup]
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

	SELECT @AdminGroupId = [Id] FROM [dbo].[UserGroup] WHERE [dbo].[UserGroup].[Name]= 'AdminGroup'

	INSERT INTO dbo.User_UserGroup
	(
	    [User],
	    [UserGroup]
	)
	VALUES
	(
	    @AdminUserId, -- User - int
	    @AdminGroupId -- Group - int
	)


    INSERT INTO [dbo].[DatabaseVersion]
		 ( DatabaseVersion )
    VALUES
		 ( '002' );
	-- DatabaseVersion - varchar
--ROLLBACK
COMMIT;