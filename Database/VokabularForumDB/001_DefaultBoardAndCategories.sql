SET XACT_ABORT ON;

BEGIN TRAN

	  CREATE UNIQUE INDEX IX_ExternalID ON [dbo].[yaf_Category](ExternalID) WHERE ExternalID IS NOT NULL

-- Insert default board 
	  INSERT INTO [dbo].[yaf_Board]
	  (    
	      [Name],	-- Type - smallint
		  [AllowThreaded]
	  )
	  VALUES
	  ('Vokabulář webový', 1)

	  DECLARE @BoardId INT
	   SELECT @BoardId = [BoardId] FROM [dbo].[yaf_Board] WHERE [dbo].[yaf_Board].[Name] = 'Vokabulář webový'

   --****** Book types ********
   --     Edition = 0,                   //Edice
   --     Dictionary = 1,                //Slovnik
   --     Grammar = 2,                   //Mluvnice
   --     ProfessionalLiterature = 3,    //Odborna literatura
   --     TextBank = 4,					 //Textova banka
   --     BibliographicalItem = 5,		 //Bibliograficke zaznamy
   --     CardFile = 6,					 //Kartoteka
   --     AudioBook = 7,				 //Audiokniha


-- Insert book types as categories
	  INSERT INTO [dbo].[yaf_Category]
	  (    
	      [BoardID],
		  [Name],
		  [SortOrder],
		  [ExternalID]
	  )
	  VALUES
	  (@BoardId, 'Edice', 2, 0),
	  (@BoardId, 'Slovníky', 1, 1),
	  (@BoardId, 'Mluvnice', 4, 2),
	  (@BoardId, 'Odborná literatura', 5, 3),
	  (@BoardId, 'Korpusy', 3, 4),
	  (@BoardId, 'Bibliografie',6, 5),
	  (@BoardId, 'Kartotéky', 7, 6),
	  (@BoardId, 'Audioknihy', 8, 7) 

	INSERT INTO [dbo].[DatabaseVersion]
		(DatabaseVersion)
    VALUES
		('001' )
		-- DatabaseVersion - varchar


	--ROLLBACK
COMMIT