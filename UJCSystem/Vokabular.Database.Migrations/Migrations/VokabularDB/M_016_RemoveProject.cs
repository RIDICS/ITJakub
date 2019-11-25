using FluentMigrator;
using Ridics.DatabaseMigrator.Shared.TagsAttributes;

namespace Vokabular.Database.Migrations.Migrations.VokabularDB
{
    [DatabaseTags(DatabaseTagTypes.VokabularDB)]
    [MigrationTypeTags(CoreMigrationTypeTagTypes.Structure, CoreMigrationTypeTagTypes.All)]
    [Migration(016)]
    public class M_016_RemoveProject : ForwardOnlyMigration
    {
        public override void Up()
        {
            Alter.Table("Project").AddColumn("IsRemoved").AsBoolean().NotNullable().SetExistingRowsTo(0);
        }
    }
}