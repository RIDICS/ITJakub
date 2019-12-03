using FluentMigrator;
using FluentMigrator.SqlServer;
using Ridics.DatabaseMigrator.Shared.TagsAttributes;

namespace Vokabular.Database.Migrations.Migrations.WebHub
{
    [DatabaseTags(DatabaseTagTypes.VokabularWebDB)]
    [MigrationTypeTags(CoreMigrationTypeTagTypes.Data, CoreMigrationTypeTagTypes.All)]
    [Migration(005)]
    public class M_005_InsertCommunityScopes : ForwardOnlyMigration
    {
        public override void Up()
        {
            Insert.IntoTable("DictionaryScope").WithIdentityInsert().Row(new
            {
                Id = 12,
                Name = "community-globe"
            });
            Insert.IntoTable("DictionaryScope").WithIdentityInsert().Row(new
            {
                Id = 13,
                Name = "community-home"
            });
            Insert.IntoTable("DictionaryScope").WithIdentityInsert().Row(new
            {
                Id = 14,
                Name = "community-edition"
            });
            Insert.IntoTable("DictionaryScope").WithIdentityInsert().Row(new
            {
                Id = 15,
                Name = "community-textbank"
            });
            Insert.IntoTable("DictionaryScope").WithIdentityInsert().Row(new
            {
                Id = 16,
                Name = "community-grammar"
            });
        }
    }
}