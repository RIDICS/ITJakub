USE [ITJakubMobileAppsDB]
SET XACT_ABORT ON
BEGIN TRAN


    ALTER TABLE [dbo].SynchronizedObject DROP COLUMN [RowKey]

    ALTER TABLE [dbo].SynchronizedObject
    ADD [ObjectExternalId] varchar (50) NULL


    CREATE TABLE [dbo].[SynchronizedObjectData]
    (
	   [Id] bigint NOT NULL IDENTITY(1,1) PRIMARY KEY,
	   [Group] bigint NOT NULL CONSTRAINT [FK_SynchronizedObjectData(Group)_Group(Id)] FOREIGN KEY REFERENCES dbo.[Group](Id),
	   [Data] nvarchar(MAX) NOT NULL
    )

    
    CREATE TABLE [dbo].[TaskData]
    (
	   [Id] bigint NOT NULL IDENTITY(1,1) PRIMARY KEY,
	   [Task] bigint NOT NULL CONSTRAINT [FK_TaskData(Task)_Task(Id)] FOREIGN KEY REFERENCES dbo.[Task](Id),
	   [Application] bigint NOT NULL CONSTRAINT [FK_TaskData(Application)_Application(Id)] FOREIGN KEY REFERENCES dbo.[Application](Id),
	   [Data] nvarchar(MAX) NOT NULL
    )




INSERT INTO [dbo].[DatabaseVersion]
		(DatabaseVersion)
	VALUES
		('005' )
		-- DatabaseVersion - varchar

--ROLLBACK
COMMIT