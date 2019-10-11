using FluentMigrator;
using Ridics.DatabaseMigrator.Shared.TagsAttributes;

namespace Vokabular.Database.Migrations.Migrations.VokabularDB
{
    [DatabaseTags(DatabaseTagTypes.VokabularDB)]
    [MigrationTypeTags(CoreMigrationTypeTagTypes.Structure, CoreMigrationTypeTagTypes.All)]
    [Migration(008)]
    public class M_008_AddProjectType : ForwardOnlyMigration
    {
        public override void Up()
        {
            const short projectTypeResearch = 0;

            Alter.Table("Project").AddColumn("ProjectType").AsInt16().NotNullable().SetExistingRowsTo(projectTypeResearch);
        }
    }
}