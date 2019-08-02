SET XACT_ABORT ON;

BEGIN TRAN;

	CREATE TABLE [dbo].[UserGroup](
		[Id] int IDENTITY(1,1) NOT NULL CONSTRAINT [PK_UserGroup(Id)] PRIMARY KEY,
		[Name] varchar(255) NULL,
		[CreateTime] datetime NOT NULL,
		[LastChange] datetime NOT NULL,
		[ExternalId] int NOT NULL CONSTRAINT [UQ_UserGroup(ExternalId)] UNIQUE,
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


    INSERT INTO [dbo].[DatabaseVersion]
		 ( DatabaseVersion )
    VALUES
		 ( '002' );
	-- DatabaseVersion - varchar
--ROLLBACK
COMMIT;