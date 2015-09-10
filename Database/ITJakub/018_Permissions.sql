SET XACT_ABORT ON;
USE ITJakubDB;

BEGIN TRAN;

	CREATE TABLE [dbo].[Group](
		[Id] int IDENTITY(1,1) NOT NULL CONSTRAINT [PK_Group(Id)] PRIMARY KEY,
		[Name] varchar(255) NOT NULL,
		[Description] varchar(500) NULL
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
	);

    INSERT INTO [dbo].[DatabaseVersion]
		 ( DatabaseVersion )
    VALUES( '018' );
	-- DatabaseVersion - varchar
--ROLLBACK
COMMIT;