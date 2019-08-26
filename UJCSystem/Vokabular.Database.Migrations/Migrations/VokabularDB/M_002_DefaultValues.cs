using System.Linq;
using FluentMigrator;
using Ridics.DatabaseMigrator.QueryBuilder;
using Ridics.DatabaseMigrator.Shared.TagsAttributes;

namespace Vokabular.Database.Migrations.Migrations.VokabularDB
{
    [DatabaseTags(DatabaseTagTypes.VokabularDB)]
    [MigrationTypeTags(CoreMigrationTypeTagTypes.Data, CoreMigrationTypeTagTypes.All)]
    [Migration(002)]
    public class M_002_DefaultValues : ForwardOnlyMigration
    {
        private const string PageToHtml = "pageToHtml.xsl";
        private const string DictionaryToHtml = "dictionaryToHtml.xsl";
        private const string PageToRtf = "pageToRtf.xsl";
        private const string DescriptionPaged = "def for paged";
        private const string DescriptionDicts = "def for dicts";

        // Output formats
        private const int Unknown = 0;
        private const int Html = 1;
        private const int Rtf = 2;
        private const int Xml = 3;

        // Resource levels
        private const int Version = 0;
        private const int Book = 1;
        private const int Shared = 2;

        // Book types
        private const int Edition = 0;                   
        private const int Dictionary = 1;                
        private const int Grammar = 2;                   
        private const int ProfessionalLiterature = 3;    
        private const int TextBank = 4;                  
        private const int BibliographicalItem = 5;       
        private const int CardFile = 6;
        private const int AudioBook = 7;           
        
        private const int TrueValue = 1;                 
        private const int FalseValue = 0;                 


        public override void Up()
        {
            Insert.IntoTable("BookType")
                .Row(new {Type = Edition}) 
                .Row(new {Type = Dictionary}) 
                .Row(new {Type = Grammar}) 
                .Row(new {Type = ProfessionalLiterature}) 
                .Row(new {Type = TextBank})
                .Row(new {Type = BibliographicalItem})
                .Row(new {Type = CardFile})
                .Row(new {Type = AudioBook});

            Execute.WithConnection((connection, transaction) =>
            {
                var editionTypeId = Query.Conn(connection, transaction).Select<int>("Id").From("BookType")
                    .Where("Type", Edition.ToString()).Run().Single();

                var dictionaryTypeId = Query.Conn(connection, transaction).Select<int>("Id").From("BookType")
                    .Where("Type", Dictionary.ToString()).Run().Single();

                var grammarTypeId = Query.Conn(connection, transaction).Select<int>("Id").From("BookType")
                    .Where("Type", Grammar.ToString()).Run().Single();

                var professionalLiteratureTypeId = Query.Conn(connection, transaction).Select<int>("Id").From("BookType")
                    .Where("Type", ProfessionalLiterature.ToString()).Run().Single();

                var textBankTypeId = Query.Conn(connection, transaction).Select<int>("Id").From("BookType")
                    .Where("Type", TextBank.ToString()).Run().Single();

                Query.Conn(connection, transaction).Insert("Transformation")
                    .Row(new
                    {
                        Name = PageToHtml,
                        Description = DescriptionPaged,
                        OutputFormat = Html,
                        BookType = editionTypeId,
                        IsDefaultForBookType = TrueValue,
                        ResourceLevel = Shared
                    })
                    .Row(new
                    {
                        Name = DictionaryToHtml,
                        Description = DescriptionDicts,
                        OutputFormat = Html,
                        BookType = dictionaryTypeId,
                        IsDefaultForBookType = TrueValue,
                        ResourceLevel = Shared
                    })
                    .Row(new
                    {
                        Name = PageToHtml,
                        Description = DescriptionPaged,
                        OutputFormat = Html,
                        BookType = grammarTypeId,
                        IsDefaultForBookType = TrueValue,
                        ResourceLevel = Shared
                    })
                    .Row(new
                    {
                        Name = PageToHtml,
                        Description = DescriptionPaged,
                        OutputFormat = Html,
                        BookType = professionalLiteratureTypeId,
                        IsDefaultForBookType = TrueValue,
                        ResourceLevel = Shared
                    })
                    .Row(new
                    {
                        Name = PageToHtml,
                        Description = DescriptionPaged,
                        OutputFormat = Html,
                        BookType = textBankTypeId,
                        IsDefaultForBookType = TrueValue,
                        ResourceLevel = Shared
                    })
                    .Row(new
                    {
                        Name = PageToRtf,
                        Description = DescriptionPaged,
                        OutputFormat = Rtf,
                        BookType = editionTypeId,
                        IsDefaultForBookType = TrueValue,
                        ResourceLevel = Shared
                    })
                    .Row(new
                    {
                        Name = PageToRtf,
                        Description = DescriptionPaged,
                        OutputFormat = Rtf,
                        BookType = grammarTypeId,
                        IsDefaultForBookType = TrueValue,
                        ResourceLevel = Shared
                    })
                    .Row(new
                    {
                        Name = PageToRtf,
                        Description = DescriptionPaged,
                        OutputFormat = Rtf,
                        BookType = professionalLiteratureTypeId,
                        IsDefaultForBookType = TrueValue,
                        ResourceLevel = Shared
                    }).Run();
            });
        }
    }
}