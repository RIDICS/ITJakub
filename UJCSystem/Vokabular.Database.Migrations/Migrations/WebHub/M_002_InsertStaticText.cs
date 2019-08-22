using FluentMigrator;
using FluentMigrator.SqlServer;
using Ridics.DatabaseMigrator.Shared.TagsAttributes;

namespace Vokabular.Database.Migrations.Migrations.WebHub
{
    [DatabaseTags(DatabaseTagTypes.VokabularWebDB)]
    [MigrationTypeTags(CoreMigrationTypeTagTypes.Data, CoreMigrationTypeTagTypes.All)]
    [Migration(002)]
    public class M_002_InsertScopes : ForwardOnlyMigration
    {
        public override void Up()
        {
            Insert.IntoTable("DictionaryScope").WithIdentityInsert().Row(new
            {
                Id = 1,
                Name = "globe"
            });
            Insert.IntoTable("DictionaryScope").WithIdentityInsert().Row(new
            {
                Id = 2,
                Name = "home"
            });
            Insert.IntoTable("DictionaryScope").WithIdentityInsert().Row(new
            {
                Id = 3,
                Name = "dict"
            });
            Insert.IntoTable("DictionaryScope").WithIdentityInsert().Row(new
            {
                Id = 4,
                Name = "edition"
            });
            Insert.IntoTable("DictionaryScope").WithIdentityInsert().Row(new
            {
                Id = 5,
                Name = "textbank"
            });
            Insert.IntoTable("DictionaryScope").WithIdentityInsert().Row(new
            {
                Id = 6,
                Name = "grammar"
            });
            Insert.IntoTable("DictionaryScope").WithIdentityInsert().Row(new
            {
                Id = 7,
                Name = "professional"
            });
            Insert.IntoTable("DictionaryScope").WithIdentityInsert().Row(new
            {
                Id = 8,
                Name = "bibliographies"
            });
            Insert.IntoTable("DictionaryScope").WithIdentityInsert().Row(new
            {
                Id = 9,
                Name = "cardfiles"
            });
            Insert.IntoTable("DictionaryScope").WithIdentityInsert().Row(new
            {
                Id = 10,
                Name = "audio"
            });
            Insert.IntoTable("DictionaryScope").WithIdentityInsert().Row(new
            {
                Id = 11,
                Name = "tools"
            });
        }
    }
}