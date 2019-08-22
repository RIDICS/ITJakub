using FluentMigrator;
using FluentMigrator.SqlServer;
using Ridics.DatabaseMigrator.Shared.TagsAttributes;

namespace Vokabular.Database.Migrations.Migrations.WebHub
{
    [DatabaseTags(DatabaseTagTypes.VokabularWebDB)]
    [MigrationTypeTags(CoreMigrationTypeTagTypes.Data, CoreMigrationTypeTagTypes.All)]
    [Migration(003)]
    public class M_003_InsertCulture_cs_CZ : ForwardOnlyMigration
    {
        public override void Up()
        {
            Insert.IntoTable("Culture").WithIdentityInsert().Row(new
            {
                Id = 1,
                Name = "cs-CZ"
            });

            Insert.IntoTable("CultureHierarchy").Row(new
            {
                Culture = 1,
                ParentCulture = 1,
                LevelProperty = 0
            });
        }
    }
}