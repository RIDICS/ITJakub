SET XACT_ABORT ON;
USE ITJakubDB;

BEGIN TRAN;

    ALTER TABLE [dbo].[Feedbacks] ADD [Category] smallint  NULL

    INSERT INTO [dbo].[DatabaseVersion]
		 ( DatabaseVersion
		 )
    VALUES( '014' );
    -- DatabaseVersion - varchar
--ROLLBACK
COMMIT;