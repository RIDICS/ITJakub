
BEGIN TRAN

    CREATE TABLE [dbo].[BookContentItem] 
    (
	   [Id] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_BookContentItem(Id)] PRIMARY KEY CLUSTERED,
	   [Text] varchar(MAX) NOT NULL,
	   [ItemOrder] int NOT NULL,
	   [BookPage] bigint NULL CONSTRAINT [FK_BookContentItem(BookPage)_BookPage(Id)] FOREIGN KEY REFERENCES [dbo].[BookPage](Id),
	   [BookVersion] bigint NOT NULL CONSTRAINT [FK_BookContentItem(BookVersion)_BookVersion(Id)] FOREIGN KEY REFERENCES [dbo].[BookVersion](Id),
	   [ParentBookContentItem] bigint NULL CONSTRAINT [FK_BookContentItem(ParentBookContentItem)_BookContentItem(Id)] FOREIGN KEY REFERENCES [dbo].[BookContentItem](Id)
    )

    INSERT INTO [dbo].[DatabaseVersion]
		(DatabaseVersion)
	VALUES
		('004' )
		-- DatabaseVersion - varchar


	--ROLLBACK
COMMIT