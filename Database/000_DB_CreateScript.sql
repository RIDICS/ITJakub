USE ITJakubMobileAppsDB
SET XACT_ABORT ON
BEGIN TRAN


--create table for storing database version
	CREATE TABLE [dbo].[DatabaseVersion] (
		[Id] [int] IDENTITY(1,1) NOT NULL,
		[DatabaseVersion] varchar(50) NOT NULL,
		[SolutionVersion] varchar(50) NULL,
		[UpgradeDate] [datetime] NOT NULL DEFAULT GETDATE(),
		[UpgradeUser] varchar(150) NOT NULL default SYSTEM_USER,
		CONSTRAINT [PK_DatabaseVersion] PRIMARY KEY CLUSTERED 
		(
			[Id] ASC
		) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	
	)

	INSERT INTO [dbo].[DatabaseVersion]
		(DatabaseVersion)
	VALUES
		('000' )
		-- DatabaseVersion - varchar



/********************* TABLES *******************/

/* Application */
CREATE TABLE [Application](
    [Id] [bigint] IDENTITY(1,1) NOT NULL,
    [Name] [varchar] (100) NOT NULL,
    CONSTRAINT [PK_Application] PRIMARY KEY CLUSTERED ([Id] ASC)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) 
)

/* Institution */
CREATE TABLE [Institution](
    [Id] [bigint] IDENTITY(1,1) NOT NULL,
    [Name] [varchar] (100) NOT NULL,
    [CreateTime] [datetime] NOT NULL,    
    [EnterCode] [varchar] (100) NOT NULL UNIQUE,
    CONSTRAINT [PK_Institution] PRIMARY KEY CLUSTERED ([Id] ASC)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) 
)

/* User */
CREATE TABLE [User](
    [Id] [bigint] IDENTITY(1,1) NOT NULL,
    [FirstName] [varchar] (50) NOT NULL,
    [LastName] [varchar] (50) NOT NULL,
    [Email] [varchar] (255) NOT NULL,
    [AuthenticationProvider] [tinyint] NOT NULL,
    [AuthenticationProviderToken] [varchar] (255) NOT NULL,
    [CommunicationToken] [varchar] (255),
    [InstitutionId] [bigint],
    [CreateTime] [datetime] NOT NULL,
    CONSTRAINT [Uniq_User] UNIQUE ([Email],[AuthenticationProvider]),
    CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED ([Id] ASC)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) 
)

/* Group*/
CREATE TABLE [Group](
    [Id] [bigint] IDENTITY(1,1) NOT NULL,    
    [AuthorId] [bigint] NOT NULL,
    [TaskId] [bigint],
    [CreateTime] [datetime] NOT NULL,
    [EnterCode] [varchar] (100) NOT NULL UNIQUE,
    CONSTRAINT [PK_Group] PRIMARY KEY CLUSTERED ([Id] ASC)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) 
)


/* Synchronized object in group */
CREATE TABLE [SynchronizedObject](
    [Id] [bigint] IDENTITY(1,1) NOT NULL,    
    [AuthorId] [bigint] NOT NULL,
    [GroupId] [bigint] NOT NULL,
    [ApplicationId] [bigint] NOT NULL,
    [CreateTime] [datetime] NOT NULL,
    [ObjectType] [varchar] (50),
    CONSTRAINT [PK_SynchronizedObject] PRIMARY KEY CLUSTERED ([Id] ASC)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)      
)


/* Predefined task for application */
CREATE TABLE [Task](
    [Id] [bigint] IDENTITY(1,1) NOT NULL,    
    [AuthorId] [bigint] NOT NULL,
    [ApplicationId] [bigint] NOT NULL,    
    [Name] [varchar] (100) NOT NULL,
    [CreateTime] [datetime] NOT NULL,
    CONSTRAINT [PK_Task] PRIMARY KEY CLUSTERED ([Id] ASC)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) 
)


/* Relationship User <--> Group */
CREATE TABLE [UserToGroup](
    [UserId] [bigint]  NOT NULL,
    [GroupId] [bigint]  NOT NULL,
    CONSTRAINT [PK_UserToGroup] PRIMARY KEY CLUSTERED ([UserId],[GroupId])WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) 
)



/********************* CONSTRAINTS *******************/

/* FK User -> Institution */
ALTER TABLE [dbo].[User]  WITH CHECK ADD  CONSTRAINT [FK_User_Institution] FOREIGN KEY([InstitutionId])
REFERENCES [dbo].[Institution] ([Id])
GO
ALTER TABLE [dbo].[User] CHECK CONSTRAINT [FK_User_Institution]
GO

/* FK Group -> Task */
ALTER TABLE [dbo].[Group]  WITH CHECK ADD  CONSTRAINT [FK_Group_Task] FOREIGN KEY([TaskId])
REFERENCES [dbo].[Task] ([Id])
GO
ALTER TABLE [dbo].[Group] CHECK CONSTRAINT [FK_Group_Task]
GO

/* FK Group -> User */
ALTER TABLE [dbo].[Group]  WITH CHECK ADD  CONSTRAINT [FK_Group_User] FOREIGN KEY([AuthorId])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[Group] CHECK CONSTRAINT [FK_Group_User]
GO

/* FK SynchronizedObject -> User */
ALTER TABLE [dbo].[SynchronizedObject]  WITH CHECK ADD  CONSTRAINT [FK_SynchronizedObject_User] FOREIGN KEY([AuthorId])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[SynchronizedObject] CHECK CONSTRAINT [FK_SynchronizedObject_User]
GO

/* FK SynchronizedObject -> Group */
ALTER TABLE [dbo].[SynchronizedObject]  WITH CHECK ADD  CONSTRAINT [FK_SynchronizedObject_Group] FOREIGN KEY([GroupId])
REFERENCES [dbo].[Group] ([Id])
GO
ALTER TABLE [dbo].[SynchronizedObject] CHECK CONSTRAINT [FK_SynchronizedObject_Group]
GO

/* FK SynchronizedObject -> Application */
ALTER TABLE [dbo].[SynchronizedObject]  WITH CHECK ADD  CONSTRAINT [FK_SynchronizedObject_Application] FOREIGN KEY([ApplicationId])
REFERENCES [dbo].[Application] ([Id])
GO
ALTER TABLE [dbo].[SynchronizedObject] CHECK CONSTRAINT [FK_SynchronizedObject_Application]
GO


/* FK Task -> User */
ALTER TABLE [dbo].[Task]  WITH CHECK ADD  CONSTRAINT [FK_Task_User] FOREIGN KEY([AuthorId])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[Task] CHECK CONSTRAINT [FK_Task_User]
GO

/* FK Task -> Application */
ALTER TABLE [dbo].[Task]  WITH CHECK ADD  CONSTRAINT [FK_Task_Application] FOREIGN KEY([ApplicationId])
REFERENCES [dbo].[Application] ([Id])
GO
ALTER TABLE [dbo].[Task] CHECK CONSTRAINT [FK_Task_Application]
GO


/* FK UserToGroup -> User */
ALTER TABLE [dbo].[UserToGroup]  WITH CHECK ADD  CONSTRAINT [FK_UserToGroup_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[UserToGroup] CHECK CONSTRAINT [FK_UserToGroup_User]
GO


/* FK UserToGroup -> Group */
ALTER TABLE [dbo].[UserToGroup]  WITH CHECK ADD  CONSTRAINT [FK_UserToGroup_Group] FOREIGN KEY([GroupId])
REFERENCES [dbo].[Group] ([Id])
GO
ALTER TABLE [dbo].[UserToGroup] CHECK CONSTRAINT [FK_UserToGroup_Group]
GO

--ROLLBACK
COMMIT