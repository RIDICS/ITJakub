
USE ITJakubDB

BEGIN TRAN

   --****** Book types ********
   --     Edition = 0,                   //Edice
   --     Dictionary = 1,                //Slovnik
   --     Grammar = 2,                   //Mluvnice
   --     ProfessionalLiterature = 3,    //Odborna literatura


	  INSERT INTO [dbo].[BookType]
	  (	    
	      [Type]	-- Type - smallint
	  )
	  VALUES
	  (0),(1),(2),(3)


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
	  ('pageToHtml.xsl', 'def for paged', 1, 1, 1, 2),	    -- Edice
	  ('dictionaryToHtml.xsl', 'def for dicts', 1, 2, 1, 2),   -- Slovnik
	  ('pageToHtml.xsl', 'def for paged', 1, 3, 1, 2),	    -- Mluvnice
	  ('pageToHtml.xsl', 'def for paged', 1, 4, 1, 2)		    -- Odborna literatura



	  --TODO add root categories with bookType relationship (i.e slovniky_cat_xmlId -> Dictionary)

	--ROLLBACK
COMMIT