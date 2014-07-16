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



--ROLLBACK
COMMIT