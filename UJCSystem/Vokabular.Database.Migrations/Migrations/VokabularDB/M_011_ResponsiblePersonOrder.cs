using FluentMigrator;
using Ridics.DatabaseMigrator.Shared.TagsAttributes;

namespace Vokabular.Database.Migrations.Migrations.VokabularDB
{
    [DatabaseTags(DatabaseTagTypes.VokabularDB)]
    [MigrationTypeTags(CoreMigrationTypeTagTypes.Structure, CoreMigrationTypeTagTypes.All)]
    [Migration(011)]
    public class M_011_ResponsiblePersonOrder : ForwardOnlyMigration
    {
        public override void Up()
        {
            Alter.Table("Project_ResponsiblePerson").AddColumn("Sequence").AsInt32().NotNullable().SetExistingRowsTo(1);
        }
    }
}