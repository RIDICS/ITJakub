SET XACT_ABORT ON;
USE ITJakubDB

BEGIN TRAN

   --****** Book types ********
   --     Edition = 0,                   //Edice
   --     Dictionary = 1,                //Slovnik
   --     Grammar = 2,                   //Mluvnice
   --     ProfessionalLiterature = 3,    //Odborna literatura
   --     TextBank = 4,				 //Textova banka

	  INSERT INTO [dbo].[BookType]
	  (    
	      [Type]	-- Type - smallint
	  )
	  VALUES
	  (0),(1),(2),(3),(4)

	  DECLARE @EditionTypeId INT
	  SELECT @EditionTypeId = [Id] FROM [dbo].[BookType] WHERE [dbo].[BookType].[Type]=0

	  DECLARE @DictionaryTypeId INT
	  SELECT @DictionaryTypeId = [Id] FROM [dbo].[BookType] WHERE [dbo].[BookType].[Type]=1
	  
	  DECLARE @GrammarTypeId INT
	  SELECT @GrammarTypeId = [Id] FROM [dbo].[BookType] WHERE [dbo].[BookType].[Type]=2
	  
	  DECLARE @ProfessionalLiteratureTypeId INT
	  SELECT @ProfessionalLiteratureTypeId = [Id] FROM [dbo].[BookType] WHERE [dbo].[BookType].[Type]=3
	  
	  DECLARE @TextBankTypeId INT
	  SELECT @TextBankTypeId = [Id] FROM [dbo].[BookType] WHERE [dbo].[BookType].[Type]=4

   --****** Output formats ********
   --Unknown = 0,
   --Html = 1,
   --Rtf = 2,
   --Xml = 3,

   --***** Resource levels *******
   --Version = 0,
   --Book = 1,
   --Shared = 2,

	  INSERT INTO [dbo].[Transformation]
	  (
	      [Name],			      -- Name - varchar
	      [Description],	      -- Description - varchar
	      [OutputFormat],		 -- OutputFormat - smallint
	      [BookType],			 -- BookType Id - int
	      [IsDefaultForBookType],	 -- IsDefaultForBookType - bit
	      [ResourceLevel]		 -- ResourceLevel - smallint
	  )
	  VALUES
	  ('pageToHtml.xsl', 'def for paged', 1, @EditionTypeId , 1, 2),				-- Edice
	  ('dictionaryToHtml.xsl', 'def for dicts', 1, @DictionaryTypeId, 1, 2),	     -- Slovnik
	  ('pageToHtml.xsl', 'def for paged', 1, @GrammarTypeId, 1, 2),			     -- Mluvnice
	  ('pageToHtml.xsl', 'def for paged', 1, @ProfessionalLiteratureTypeId, 1, 2),	-- Odborna literatura
	  ('pageToHtml.xsl', 'def for paged', 1, @TextBankTypeId, 1, 2)	-- Textova banka



	  --TODO add root categories with bookType relationship (i.e slovniky_cat_xmlId -> Dictionary)

	  INSERT INTO [dbo].[Category]
	  (
	      [XmlId], -- XmlId - varchar
	      [Description], -- Description - varchar
	      [ParentCategory],  -- ParentCategory - FK int
	      [BookType],  -- BookType - FK int
		  [Path]
	  )
	  VALUES
	  ('output-editions','Edice',NULL,@EditionTypeId,'/output-editions/'),
	  ('output-dictionary','Slovnik',NULL,@DictionaryTypeId,'/output-dictionary/'),
	  ('output-text_bank','Textova banka',NULL,@TextBankTypeId,'/output-text_bank/'),
	  ('output-scholary_literature','Odborna literatura',NULL,@ProfessionalLiteratureTypeId,'/output-scholary_literature/'),
	  ('output-digitized-grammar','Mluvnice',NULL,@GrammarTypeId,'/output-digitized-grammar/')
	  

    INSERT INTO [dbo].[DatabaseVersion]
		(DatabaseVersion)
    VALUES
		('011' )
		-- DatabaseVersion - varchar


	--ROLLBACK
COMMIT