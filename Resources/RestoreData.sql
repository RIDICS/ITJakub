USE [ITJakubDBBackup]
GO


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