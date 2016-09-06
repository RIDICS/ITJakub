SET XACT_ABORT ON;

BEGIN TRAN

    CREATE TABLE [dbo].[FavoriteLabel]
	(
		[Id] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_FavoriteLabel(Id)] PRIMARY KEY CLUSTERED,
		[Name] varchar(150) NOT NULL,
		[Color] char(7) NOT NULL,
		[User] int NOT NULL CONSTRAINT [FK_FavoriteLabel(User)_User(Id)] FOREIGN KEY REFERENCES [dbo].[User](Id),
		[ParentLabel] bigint NULL CONSTRAINT [FK_FavoriteLabel(ParentLabel)_FavoriteLabel(Id)] FOREIGN KEY REFERENCES [dbo].[FavoriteLabel](Id),
		[IsDefault] bit NOT NULL,
		[LastUseTime] datetime NULL
	)

	ALTER TABLE [dbo].[Favorites] ADD
	   [CreateTime] datetime NULL,
	   [FavoriteLabel] bigint NULL FOREIGN KEY REFERENCES [dbo].[FavoriteLabel](Id),
	   [BookVersion] bigint NULL FOREIGN KEY REFERENCES [dbo].[BookVersion](Id),
	   [BookType] int NULL FOREIGN KEY REFERENCES [dbo].[BookType](Id),
	   [Query] varchar(max) NULL,
	   [QueryType] smallint NULL,
	   [CardFileId] varchar(255) NULL


	INSERT INTO [dbo].[FavoriteLabel]
	(
		Name,
		Color,
		[User]
	)
	SELECT 'Výchozí', '#EEB711', u.Id FROM [dbo].[User] u
	

    INSERT INTO [dbo].[DatabaseVersion]
		(DatabaseVersion)
    VALUES
		( '029' )
		-- DatabaseVersion - varchar

--ROLLBACK
COMMIT 