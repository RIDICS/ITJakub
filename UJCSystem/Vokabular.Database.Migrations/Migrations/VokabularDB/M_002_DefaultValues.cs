using System.Linq;
using FluentMigrator;
using FluentMigrator.SqlServer;
using Ridics.DatabaseMigrator.QueryBuilder;
using Ridics.DatabaseMigrator.Shared.TagsAttributes;

namespace Vokabular.Database.Migrations.Migrations.VokabularDB
{
    [DatabaseTags(DatabaseTagTypes.VokabularDB)]
    [MigrationTypeTags(CoreMigrationTypeTagTypes.Data, CoreMigrationTypeTagTypes.All)]
    [Migration(002)]
    public class M_002_DefaultValues : ForwardOnlyMigration
    {
        public override void Up()
        {
            Insert.IntoTable("BookType").WithIdentityInsert().Row(new { Id = 0, Type = 0});
            Insert.IntoTable("BookType").WithIdentityInsert().Row(new { Id = 1, Type = 1});
            Insert.IntoTable("BookType").WithIdentityInsert().Row(new { Id = 2, Type = 2});
            Insert.IntoTable("BookType").WithIdentityInsert().Row(new { Id = 3, Type = 3});
            Insert.IntoTable("BookType").WithIdentityInsert().Row(new { Id = 4, Type = 4});
            Insert.IntoTable("BookType").WithIdentityInsert().Row(new { Id = 5, Type = 5});
            Insert.IntoTable("BookType").WithIdentityInsert().Row(new { Id = 6, Type = 6});
            Insert.IntoTable("BookType").WithIdentityInsert().Row(new { Id = 7, Type = 7});

            Execute.WithConnection((connection, transaction) =>
            {
                var editionTypeId = Query.Conn(connection, transaction).Select<int>("Id").From("BookType")
                    .Where("Type", "0").Run().Single();

                var dictionaryTypeId = Query.Conn(connection, transaction).Select<int>("Id").From("BookType")
                    .Where("Type", "1").Run().Single();

                var grammarTypeId = Query.Conn(connection, transaction).Select<int>("Id").From("BookType")
                    .Where("Type", "2").Run().Single();

                var professionalLiteratureTypeId = Query.Conn(connection, transaction).Select<int>("Id").From("BookType")
                    .Where("Type", "3").Run().Single();

                var textBankTypeId = Query.Conn(connection, transaction).Select<int>("Id").From("BookType")
                    .Where("Type", "4").Run().Single();

               Query.Conn(connection, transaction).Insert("Transformation")
                    .Row(new
                    {
                        Name = "pageToHtml.xsl",
                        Description = "def for paged",
                        OutputFormat = 1,
                        BookType = editionTypeId,
                        IsDefaultForBookType = 1,
                        ResourceLevel = 2
                    }).Run();

                Query.Conn(connection, transaction).Insert("Transformation")
                    .Row(new
                    {
                        Name = "dictionaryToHtml.xsl",
                        Description = "def for dicts",
                        OutputFormat = 1,
                        BookType = dictionaryTypeId,
                        IsDefaultForBookType = 1,
                        ResourceLevel = 2
                    }).Run();

                Query.Conn(connection, transaction).Insert("Transformation")
                    .Row(new
                    {
                        Name = "pageToHtml.xsl",
                        Description = "def for paged",
                        OutputFormat = 1,
                        BookType = grammarTypeId,
                        IsDefaultForBookType = 1,
                        ResourceLevel = 2
                    }).Run();

                Query.Conn(connection, transaction).Insert("Transformation")
                    .Row(new
                    {
                        Name = "pageToHtml.xsl",
                        Description = "def for paged",
                        OutputFormat = 1,
                        BookType = professionalLiteratureTypeId,
                        IsDefaultForBookType = 1,
                        ResourceLevel = 2
                    }).Run();

                Query.Conn(connection, transaction).Insert("Transformation")
                    .Row(new
                    {
                        Name = "pageToHtml.xsl",
                        Description = "def for paged",
                        OutputFormat = 1,
                        BookType = textBankTypeId,
                        IsDefaultForBookType = 1,
                        ResourceLevel = 2
                    }).Run();

                Query.Conn(connection, transaction).Insert("Transformation")
                    .Row(new
                    {
                        Name = "pageToRtf.xsl",
                        Description = "def for paged",
                        OutputFormat = 2,
                        BookType = editionTypeId,
                        IsDefaultForBookType = 1,
                        ResourceLevel = 2
                    }).Run();

                Query.Conn(connection, transaction).Insert("Transformation")
                    .Row(new
                    {
                        Name = "pageToRtf.xsl",
                        Description = "def for paged",
                        OutputFormat = 2,
                        BookType = grammarTypeId,
                        IsDefaultForBookType = 1,
                        ResourceLevel = 2
                    }).Run();

                Query.Conn(connection, transaction).Insert("Transformation")
                    .Row(new
                    {
                        Name = "pageToRtf.xsl",
                        Description = "def for paged",
                        OutputFormat = 2,
                        BookType = professionalLiteratureTypeId,
                        IsDefaultForBookType = 1,
                        ResourceLevel = 2
                    }).Run();
            });
        }
    }
}