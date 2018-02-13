SET XACT_ABORT ON;

BEGIN TRAN

   --****** Book types ********
   --     Edition = 0,                   //Edice
   --     Dictionary = 1,                //Slovnik
   --     Grammar = 2,                   //Mluvnice
   --     ProfessionalLiterature = 3,    //Odborna literatura
   --     TextBank = 4,					 //Textova banka
   --     BibliographicalItem = 5,		 //Bibliograficke zaznamy
   --     CardFile = 6,					 //Kartoteka
   --     AudioBook = 7,				 //Audiokniha

	  INSERT INTO [dbo].[BookType]
	  (    
	      [Type]	-- Type - smallint
	  )
	  VALUES
	  (0),(1),(2),(3),(4),(5),(6),(7)

	  DECLARE @EditionTypeId INT, @DictionaryTypeId INT, @GrammarTypeId INT, @ProfessionalLiteratureTypeId INT, @TextBankTypeId INT, @BibliographicalItemTypeId INT, @CardFileTypeId INT, @AudioBookTypeId INT

	  SELECT @EditionTypeId = [Id] FROM [dbo].[BookType] WHERE [dbo].[BookType].[Type]=0
	  SELECT @DictionaryTypeId = [Id] FROM [dbo].[BookType] WHERE [dbo].[BookType].[Type]=1
	  SELECT @GrammarTypeId = [Id] FROM [dbo].[BookType] WHERE [dbo].[BookType].[Type]=2
	  SELECT @ProfessionalLiteratureTypeId = [Id] FROM [dbo].[BookType] WHERE [dbo].[BookType].[Type]=3
	  SELECT @TextBankTypeId = [Id] FROM [dbo].[BookType] WHERE [dbo].[BookType].[Type]=4
	  SELECT @BibliographicalItemTypeId = [Id] FROM [dbo].[BookType] WHERE [dbo].[BookType].[Type]=5
	  SELECT @CardFileTypeId = [Id] FROM [dbo].[BookType] WHERE [dbo].[BookType].[Type]=6
	  SELECT @AudioBookTypeId = [Id] FROM [dbo].[BookType] WHERE [dbo].[BookType].[Type]=7

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
	  ('pageToHtml.xsl', 'def for paged', 1, @TextBankTypeId, 1, 2),				-- Textova banka
	  ('pageToRtf.xsl', 'def for paged', 2, @EditionTypeId , 1, 2),
	  ('pageToRtf.xsl', 'def for paged', 2, @GrammarTypeId , 1, 2),
	  ('pageToRtf.xsl', 'def for paged', 2, @ProfessionalLiteratureTypeId , 1, 2)


	INSERT INTO [dbo].[User]
           ([FirstName]
           ,[LastName]
           ,[Email]
           ,[UserName]
           ,[AuthenticationProvider]
           ,[CommunicationToken]
           ,[CommunicationTokenCreateTime]
           ,[PasswordHash]
           ,[CreateTime]
           ,[AvatarUrl])
     VALUES
           ('Admin' -- FirstName
           ,'Admin' -- LastName
           ,'admin@example.com' -- Email
           ,'Admin' -- UserName
           ,0 -- AuthenticationProvider
           ,'admin-token' -- CommunicationToken
           ,'2017-08-21 00:00:00.000' -- CommunicationTokenCreateTime
           ,'PW:sha1:1000:FhLySoxcL/5CA0RqlRBZMiqblj4sZ0zV:Vocj0I6bhs9bF4p9Nh+Rk7vbCoToulg9' -- PasswordHash (password is 'Administrator')
           ,'2017-08-21 00:00:00.000' -- CreateTime
           ,NULL) -- AvatarUrl


	INSERT INTO [dbo].[DatabaseVersion]
		(DatabaseVersion)
    VALUES
		('001' )
		-- DatabaseVersion - varchar


	--ROLLBACK
COMMIT