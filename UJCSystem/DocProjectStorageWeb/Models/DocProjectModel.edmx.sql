
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, and Azure
-- --------------------------------------------------
-- Date Created: 12/10/2012 17:26:56
-- Generated from EDMX file: C:\UJCSystem\DocProjectStorageWeb\Models\DocProjectModel.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [docprojectdb];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_EditorUser]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[EditorEntities] DROP CONSTRAINT [FK_EditorUser];
GO
IF OBJECT_ID(N'[dbo].[FK_UserUserRole]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[UserRoleEntities] DROP CONSTRAINT [FK_UserUserRole];
GO
IF OBJECT_ID(N'[dbo].[FK_DocumentProjectDocumentRevision]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RevisionEntities] DROP CONSTRAINT [FK_DocumentProjectDocumentRevision];
GO
IF OBJECT_ID(N'[dbo].[FK_DocumentProjectEditor]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[EditorEntities] DROP CONSTRAINT [FK_DocumentProjectEditor];
GO
IF OBJECT_ID(N'[dbo].[FK_DocumentProjectMessage]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[MessageEntities] DROP CONSTRAINT [FK_DocumentProjectMessage];
GO
IF OBJECT_ID(N'[dbo].[FK_EventDocumentProject]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[EventEntities] DROP CONSTRAINT [FK_EventDocumentProject];
GO
IF OBJECT_ID(N'[dbo].[FK_UserEvent]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[EventEntities] DROP CONSTRAINT [FK_UserEvent];
GO
IF OBJECT_ID(N'[dbo].[FK_ProjectStateDocumentProject]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ProjectStateEntities] DROP CONSTRAINT [FK_ProjectStateDocumentProject];
GO
IF OBJECT_ID(N'[dbo].[FK_UserProjectState]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ProjectStateEntities] DROP CONSTRAINT [FK_UserProjectState];
GO
IF OBJECT_ID(N'[dbo].[FK_MessageEntityUserEntity]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[MessageEntities] DROP CONSTRAINT [FK_MessageEntityUserEntity];
GO
IF OBJECT_ID(N'[dbo].[FK_DocumentTypeDocProjectEntity]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[DocProjectEntities] DROP CONSTRAINT [FK_DocumentTypeDocProjectEntity];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[DocProjectEntities]', 'U') IS NOT NULL
    DROP TABLE [dbo].[DocProjectEntities];
GO
IF OBJECT_ID(N'[dbo].[RevisionEntities]', 'U') IS NOT NULL
    DROP TABLE [dbo].[RevisionEntities];
GO
IF OBJECT_ID(N'[dbo].[MessageEntities]', 'U') IS NOT NULL
    DROP TABLE [dbo].[MessageEntities];
GO
IF OBJECT_ID(N'[dbo].[EditorEntities]', 'U') IS NOT NULL
    DROP TABLE [dbo].[EditorEntities];
GO
IF OBJECT_ID(N'[dbo].[UserEntities]', 'U') IS NOT NULL
    DROP TABLE [dbo].[UserEntities];
GO
IF OBJECT_ID(N'[dbo].[UserRoleEntities]', 'U') IS NOT NULL
    DROP TABLE [dbo].[UserRoleEntities];
GO
IF OBJECT_ID(N'[dbo].[EventEntities]', 'U') IS NOT NULL
    DROP TABLE [dbo].[EventEntities];
GO
IF OBJECT_ID(N'[dbo].[ProjectStateEntities]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ProjectStateEntities];
GO
IF OBJECT_ID(N'[dbo].[DocumentTypes]', 'U') IS NOT NULL
    DROP TABLE [dbo].[DocumentTypes];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'DocProjectEntities'
CREATE TABLE [dbo].[DocProjectEntities] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Title] nvarchar(max)  NOT NULL,
    [Author] nvarchar(max)  NOT NULL,
    [Type_Id] int  NOT NULL
);
GO

-- Creating table 'RevisionEntities'
CREATE TABLE [dbo].[RevisionEntities] (
    [Number] int  NOT NULL,
    [Released] datetime  NOT NULL,
    [DocxData] varbinary(max)  NOT NULL,
    [Id] int IDENTITY(1,1) NOT NULL,
    [DocumentProjectDocumentRevision_DocumentRevision_Id] int  NOT NULL
);
GO

-- Creating table 'MessageEntities'
CREATE TABLE [dbo].[MessageEntities] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Content] nvarchar(max)  NOT NULL,
    [SentTime] datetime  NOT NULL,
    [DocumentProjectMessage_Message_Id] int  NOT NULL,
    [Sender_Id] int  NOT NULL
);
GO

-- Creating table 'EditorEntities'
CREATE TABLE [dbo].[EditorEntities] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [CanWrite] bit  NOT NULL,
    [User_Id] int  NOT NULL,
    [DocumentProjectEditor_Editor_Id] int  NOT NULL
);
GO

-- Creating table 'UserEntities'
CREATE TABLE [dbo].[UserEntities] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Email] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'UserRoleEntities'
CREATE TABLE [dbo].[UserRoleEntities] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [RoleName] nvarchar(max)  NULL,
    [UserUserRole_UserRole_Id] int  NOT NULL
);
GO

-- Creating table 'EventEntities'
CREATE TABLE [dbo].[EventEntities] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [OccurTime] datetime  NOT NULL,
    [Type] nvarchar(max)  NOT NULL,
    [RelatedProject_Id] int  NULL,
    [RelatedUser_Id] int  NULL
);
GO

-- Creating table 'ProjectStateEntities'
CREATE TABLE [dbo].[ProjectStateEntities] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Type] nvarchar(max)  NOT NULL,
    [ProjectStateDocumentProject_ProjectState_Id] int  NOT NULL,
    [AssignedUser_Id] int  NULL
);
GO

-- Creating table 'DocumentTypes'
CREATE TABLE [dbo].[DocumentTypes] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'DocProjectEntities'
ALTER TABLE [dbo].[DocProjectEntities]
ADD CONSTRAINT [PK_DocProjectEntities]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Number], [Id] in table 'RevisionEntities'
ALTER TABLE [dbo].[RevisionEntities]
ADD CONSTRAINT [PK_RevisionEntities]
    PRIMARY KEY CLUSTERED ([Number], [Id] ASC);
GO

-- Creating primary key on [Id] in table 'MessageEntities'
ALTER TABLE [dbo].[MessageEntities]
ADD CONSTRAINT [PK_MessageEntities]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'EditorEntities'
ALTER TABLE [dbo].[EditorEntities]
ADD CONSTRAINT [PK_EditorEntities]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'UserEntities'
ALTER TABLE [dbo].[UserEntities]
ADD CONSTRAINT [PK_UserEntities]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'UserRoleEntities'
ALTER TABLE [dbo].[UserRoleEntities]
ADD CONSTRAINT [PK_UserRoleEntities]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'EventEntities'
ALTER TABLE [dbo].[EventEntities]
ADD CONSTRAINT [PK_EventEntities]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ProjectStateEntities'
ALTER TABLE [dbo].[ProjectStateEntities]
ADD CONSTRAINT [PK_ProjectStateEntities]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'DocumentTypes'
ALTER TABLE [dbo].[DocumentTypes]
ADD CONSTRAINT [PK_DocumentTypes]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [User_Id] in table 'EditorEntities'
ALTER TABLE [dbo].[EditorEntities]
ADD CONSTRAINT [FK_EditorUser]
    FOREIGN KEY ([User_Id])
    REFERENCES [dbo].[UserEntities]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_EditorUser'
CREATE INDEX [IX_FK_EditorUser]
ON [dbo].[EditorEntities]
    ([User_Id]);
GO

-- Creating foreign key on [UserUserRole_UserRole_Id] in table 'UserRoleEntities'
ALTER TABLE [dbo].[UserRoleEntities]
ADD CONSTRAINT [FK_UserUserRole]
    FOREIGN KEY ([UserUserRole_UserRole_Id])
    REFERENCES [dbo].[UserEntities]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_UserUserRole'
CREATE INDEX [IX_FK_UserUserRole]
ON [dbo].[UserRoleEntities]
    ([UserUserRole_UserRole_Id]);
GO

-- Creating foreign key on [DocumentProjectDocumentRevision_DocumentRevision_Id] in table 'RevisionEntities'
ALTER TABLE [dbo].[RevisionEntities]
ADD CONSTRAINT [FK_DocumentProjectDocumentRevision]
    FOREIGN KEY ([DocumentProjectDocumentRevision_DocumentRevision_Id])
    REFERENCES [dbo].[DocProjectEntities]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_DocumentProjectDocumentRevision'
CREATE INDEX [IX_FK_DocumentProjectDocumentRevision]
ON [dbo].[RevisionEntities]
    ([DocumentProjectDocumentRevision_DocumentRevision_Id]);
GO

-- Creating foreign key on [DocumentProjectEditor_Editor_Id] in table 'EditorEntities'
ALTER TABLE [dbo].[EditorEntities]
ADD CONSTRAINT [FK_DocumentProjectEditor]
    FOREIGN KEY ([DocumentProjectEditor_Editor_Id])
    REFERENCES [dbo].[DocProjectEntities]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_DocumentProjectEditor'
CREATE INDEX [IX_FK_DocumentProjectEditor]
ON [dbo].[EditorEntities]
    ([DocumentProjectEditor_Editor_Id]);
GO

-- Creating foreign key on [DocumentProjectMessage_Message_Id] in table 'MessageEntities'
ALTER TABLE [dbo].[MessageEntities]
ADD CONSTRAINT [FK_DocumentProjectMessage]
    FOREIGN KEY ([DocumentProjectMessage_Message_Id])
    REFERENCES [dbo].[DocProjectEntities]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_DocumentProjectMessage'
CREATE INDEX [IX_FK_DocumentProjectMessage]
ON [dbo].[MessageEntities]
    ([DocumentProjectMessage_Message_Id]);
GO

-- Creating foreign key on [RelatedProject_Id] in table 'EventEntities'
ALTER TABLE [dbo].[EventEntities]
ADD CONSTRAINT [FK_EventDocumentProject]
    FOREIGN KEY ([RelatedProject_Id])
    REFERENCES [dbo].[DocProjectEntities]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_EventDocumentProject'
CREATE INDEX [IX_FK_EventDocumentProject]
ON [dbo].[EventEntities]
    ([RelatedProject_Id]);
GO

-- Creating foreign key on [RelatedUser_Id] in table 'EventEntities'
ALTER TABLE [dbo].[EventEntities]
ADD CONSTRAINT [FK_UserEvent]
    FOREIGN KEY ([RelatedUser_Id])
    REFERENCES [dbo].[UserEntities]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_UserEvent'
CREATE INDEX [IX_FK_UserEvent]
ON [dbo].[EventEntities]
    ([RelatedUser_Id]);
GO

-- Creating foreign key on [ProjectStateDocumentProject_ProjectState_Id] in table 'ProjectStateEntities'
ALTER TABLE [dbo].[ProjectStateEntities]
ADD CONSTRAINT [FK_ProjectStateDocumentProject]
    FOREIGN KEY ([ProjectStateDocumentProject_ProjectState_Id])
    REFERENCES [dbo].[DocProjectEntities]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ProjectStateDocumentProject'
CREATE INDEX [IX_FK_ProjectStateDocumentProject]
ON [dbo].[ProjectStateEntities]
    ([ProjectStateDocumentProject_ProjectState_Id]);
GO

-- Creating foreign key on [AssignedUser_Id] in table 'ProjectStateEntities'
ALTER TABLE [dbo].[ProjectStateEntities]
ADD CONSTRAINT [FK_UserProjectState]
    FOREIGN KEY ([AssignedUser_Id])
    REFERENCES [dbo].[UserEntities]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_UserProjectState'
CREATE INDEX [IX_FK_UserProjectState]
ON [dbo].[ProjectStateEntities]
    ([AssignedUser_Id]);
GO

-- Creating foreign key on [Sender_Id] in table 'MessageEntities'
ALTER TABLE [dbo].[MessageEntities]
ADD CONSTRAINT [FK_MessageEntityUserEntity]
    FOREIGN KEY ([Sender_Id])
    REFERENCES [dbo].[UserEntities]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_MessageEntityUserEntity'
CREATE INDEX [IX_FK_MessageEntityUserEntity]
ON [dbo].[MessageEntities]
    ([Sender_Id]);
GO

-- Creating foreign key on [Type_Id] in table 'DocProjectEntities'
ALTER TABLE [dbo].[DocProjectEntities]
ADD CONSTRAINT [FK_DocumentTypeDocProjectEntity]
    FOREIGN KEY ([Type_Id])
    REFERENCES [dbo].[DocumentTypes]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_DocumentTypeDocProjectEntity'
CREATE INDEX [IX_FK_DocumentTypeDocProjectEntity]
ON [dbo].[DocProjectEntities]
    ([Type_Id]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------