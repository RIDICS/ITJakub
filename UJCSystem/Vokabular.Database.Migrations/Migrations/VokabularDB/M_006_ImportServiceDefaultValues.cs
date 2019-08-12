using FluentMigrator;
using Ridics.DatabaseMigrator.Shared.TagsAttributes;

namespace Vokabular.Database.Migrations.Migrations.VokabularDB
{
    [DatabaseTags(DatabaseTagTypes.VokabularDB)]
    [MigrationTypeTags(CoreMigrationTypeTagTypes.Data, CoreMigrationTypeTagTypes.All)]
    [Migration(006)]
    public class M_006_ImportServiceDefaultValues : ForwardOnlyMigration
    {
        public override void Up()
        {
            Insert.IntoTable("ExternalRepositoryType").Row(new {Name = "OaiPmh"});
            Insert.IntoTable("ExternalRepositoryType").Row(new {Name = "Marc21"});
        }
    }
}