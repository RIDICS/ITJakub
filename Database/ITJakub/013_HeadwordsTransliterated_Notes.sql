SET XACT_ABORT ON;
USE ITJakubDB;

BEGIN TRAN;
    ALTER TABLE [dbo].[BookHeadword]
    ADD [Transliterated] NVARCHAR(255) NULL;

    CREATE TABLE [dbo].[Feedbacks]( [Id]           BIGINT IDENTITY(1, 1)
											   NOT NULL
											   CONSTRAINT [PK_Feedback(Id)] PRIMARY KEY CLUSTERED,
							 [FeedbackType] VARCHAR(255) NOT NULL,
							 [Text]         VARCHAR(2000) NOT NULL,
							 [CreateDate]   DATETIME NOT NULL,
							 [Name] varchar(255) NULL,
							 [Email] varchar(255) NULL,
							 [User]         INT NULL
											FOREIGN KEY REFERENCES [dbo].[User]( Id ),
							 [BookHeadword] BIGINT NULL
											   FOREIGN KEY REFERENCES [dbo].[BookHeadword]( Id ));
    INSERT INTO [dbo].[DatabaseVersion]
		 ( DatabaseVersion
		 )
    VALUES( '013' );
    -- DatabaseVersion - varchar
--ROLLBACK
COMMIT;