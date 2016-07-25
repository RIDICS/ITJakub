SET XACT_ABORT ON;

BEGIN TRAN;

    ALTER TABLE [dbo].[Feedbacks] ADD [Category] smallint  NULL

    INSERT INTO [dbo].[DatabaseVersion]
		 ( DatabaseVersion
		 )
    VALUES( '015' );
    -- DatabaseVersion - varchar
--ROLLBACK
COMMIT;