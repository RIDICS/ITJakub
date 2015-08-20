USE [ITJakubDBBackup]
GO


CREATE TABLE [dbo].[User](
	[Id] [int] NOT NULL,
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





/****** Object:  Table [dbo].[Feedbacks]    Script Date: 20-Aug-15 19:52:53 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO


CREATE TABLE [dbo].[Feedbacks](
	[Id] [bigint] NOT NULL,
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

SET ANSI_PADDING OFF
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