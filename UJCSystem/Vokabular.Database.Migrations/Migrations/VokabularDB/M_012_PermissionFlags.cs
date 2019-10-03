using FluentMigrator;
using Ridics.DatabaseMigrator.Shared.TagsAttributes;

namespace Vokabular.Database.Migrations.Migrations.VokabularDB
{
    [DatabaseTags(DatabaseTagTypes.VokabularDB)]
    [MigrationTypeTags(CoreMigrationTypeTagTypes.Structure, CoreMigrationTypeTagTypes.All)]
    [Migration(012)]
    public class M_012_PermissionFlags : ForwardOnlyMigration
    {
        public override void Up()
        {
            Alter.Table("Permission").AddColumn("Flags").AsInt32().NotNullable().SetExistingRowsTo(1);
        }
    }
}