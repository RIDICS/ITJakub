USE [ITJakubDBBackup]
GO

CREATE TABLE [dbo].[User](
	[Id] [int] NOT NULL IDENTITY(1,1) PRIMARY KEY CLUSTERED,
	[FirstName] [varchar](50) NULL,
	[LastName] [varchar](50) NULL,
	[Email] [varchar](255) NOT NULL,
	[AuthenticationProvider] [tinyint] NOT NULL,
	[CommunicationToken] [varchar](255) NOT NULL,
	[CommunicationTokenCreateTime] [datetime] NULL,
	[PasswordHash] [varchar](255) NULL,
	[Salt] [varchar](50) NULL,
	[CreateTime] [datetime] NOT NULL,
	[AvatarUrl] [varchar](255) NULL,
	[UserName] [varchar](50) NOT NULL
) ON [PRIMARY]



CREATE TABLE [dbo].[Feedbacks](
	[Id] [bigint] NOT NULL IDENTITY(1,1) PRIMARY KEY CLUSTERED,
	[FeedbackType] [varchar](255) NOT NULL,
	[Text] [varchar](2000) NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[Name] [varchar](255) NULL,
	[Email] [varchar](255) NULL,
	[User] [int] NULL,
	[BookHeadword] [bigint] NULL,
	[Category] [smallint] NULL
) ON [PRIMARY]

GO



CREATE TABLE [dbo].[NewsSyndicationItem](
	[Id] [bigint] NOT NULL IDENTITY(1,1) PRIMARY KEY CLUSTERED,
	[Title] [varchar](255) NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[Text] [varchar](2000) NOT NULL,
	[Url] [varchar](max) NOT NULL,
	[ItemType] [smallint] NOT NULL,
	[User] [int] NULL
)






SET IDENTITY_INSERT ITJakubDBBackup.[dbo].[User] ON


GO


INSERT INTO ITJakubDBBackup.[dbo].[User]
(
    Id,
    [FirstName],
    [LastName],
    [Email],
    [AuthenticationProvider],
    [CommunicationToken],
    [CommunicationTokenCreateTime],
    [PasswordHash],
    [Salt],
    [CreateTime],
    [AvatarUrl],
    [UserName]
)

SELECT * FROM [ITJakubDB].[dbo].[User]


SET IDENTITY_INSERT ITJakubDBBackup.[dbo].[User] OFF
SET IDENTITY_INSERT ITJakubDBBackup.[dbo].[Feedbacks] ON


INSERT INTO [ITJakubDBBackup].[dbo].[Feedbacks]
(
    [Id],
    [FeedbackType],
    [Text],
    [CreateDate],
    [Name],
    [Email],
    [User],
    [BookHeadword],
    [Category]
)
SELECT * FROM [ITJakubDB].[dbo].[Feedbacks] f



SET IDENTITY_INSERT ITJakubDBBackup.[dbo].[Feedbacks] OFF
SET IDENTITY_INSERT ITJakubDBBackup.[dbo].[NewsSyndicationItem] ON



INSERT INTO [ITJakubDBBackup].[dbo].[NewsSyndicationItem]
(
     [Id],
    [Title],
    [CreateDate],
    [Text],
    [Url],
    [ItemType],
    [User]
)
SELECT * FROM [ITJakubDB].[dbo].[NewsSyndicationItem]






SET IDENTITY_INSERT ITJakubDBBackup.[dbo].[NewsSyndicationItem] OFF






--COPY BACK
SET IDENTITY_INSERT [ITJakubDB].[dbo].[User] ON


INSERT INTO [ITJakubDB].[dbo].[User]
(
    Id,
    [FirstName],
    [LastName],
    [Email],
    [AuthenticationProvider],
    [CommunicationToken],
    [CommunicationTokenCreateTime],
    [PasswordHash],
    [Salt],
    [CreateTime],
    [AvatarUrl],
    [UserName]
)

SELECT * FROM [ITJakubDBBackup].[dbo].[User]

SET IDENTITY_INSERT [ITJakubDB].[dbo].[User] OFF
SET IDENTITY_INSERT [ITJakubDB].[dbo].[Feedbacks] ON

INSERT INTO [ITJakubDB].[dbo].[Feedbacks]
(
    [Id],
    [FeedbackType],
    [Text],
    [CreateDate],
    [Name],
    [Email],
    [User],
    [BookHeadword],
    [Category]
)
SELECT * FROM [ITJakubDBBackup].[dbo].[Feedbacks] f


SET IDENTITY_INSERT [ITJakubDB].[dbo].[Feedbacks] OFF
SET IDENTITY_INSERT [ITJakubDB].[dbo].[NewsSyndicationItem] ON


INSERT INTO [ITJakubDB].[dbo].[NewsSyndicationItem]
(
     [Id],
    [Title],
    [CreateDate],
    [Text],
    [Url],
    [ItemType],
    [User]
)
SELECT * FROM [ITJakubDBBackup].[dbo].[NewsSyndicationItem]



SET IDENTITY_INSERT [ITJakubDB].[dbo].[NewsSyndicationItem] OFF