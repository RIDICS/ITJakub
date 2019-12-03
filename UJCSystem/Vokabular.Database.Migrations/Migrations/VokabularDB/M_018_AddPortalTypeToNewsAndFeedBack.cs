using FluentMigrator;
using Ridics.DatabaseMigrator.Shared.TagsAttributes;

namespace Vokabular.Database.Migrations.Migrations.VokabularDB
{
    [DatabaseTags(DatabaseTagTypes.VokabularDB)]
    [MigrationTypeTags(CoreMigrationTypeTagTypes.Structure, CoreMigrationTypeTagTypes.All)]
    [Migration(018)]
    public class M_018_AddPortalTypeToNewsAndFeedBack : ForwardOnlyMigration
    {
        public override void Up()
        {
            Alter.Table("Feedback")
                .AddColumn("ProjectType").AsInt16().NotNullable().SetExistingRowsTo(0);
            
            Alter.Table("NewsSyndicationItem")
                .AddColumn("ProjectType").AsInt16().NotNullable().SetExistingRowsTo(0);
        }
    }
}