
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, and Azure
-- --------------------------------------------------
-- Date Created: 10/16/2012 11:27:37
-- Generated from EDMX file: C:\UJCSystem\DocumentStorage\Model1.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [jacobdocs-test];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------


-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------


-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Documents'
CREATE TABLE [dbo].[Documents] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [name] nvarchar(max)  NOT NULL,
    [author] nvarchar(max)  NOT NULL,
    [date] datetime  NOT NULL
);
GO

-- Creating table 'Revisions'
CREATE TABLE [dbo].[Revisions] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [file] varbinary(max)  NOT NULL,
    [date] datetime  NOT NULL,
    [Document_Id] int  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'Documents'
ALTER TABLE [dbo].[Documents]
ADD CONSTRAINT [PK_Documents]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Revisions'
ALTER TABLE [dbo].[Revisions]
ADD CONSTRAINT [PK_Revisions]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [Document_Id] in table 'Revisions'
ALTER TABLE [dbo].[Revisions]
ADD CONSTRAINT [FK_DocumentRevision]
    FOREIGN KEY ([Document_Id])
    REFERENCES [dbo].[Documents]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_DocumentRevision'
CREATE INDEX [IX_FK_DocumentRevision]
ON [dbo].[Revisions]
    ([Document_Id]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------