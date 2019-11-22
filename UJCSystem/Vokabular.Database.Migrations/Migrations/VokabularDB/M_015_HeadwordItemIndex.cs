using FluentMigrator;
using Ridics.DatabaseMigrator.Shared.TagsAttributes;

namespace Vokabular.Database.Migrations.Migrations.VokabularDB
{
    [DatabaseTags(DatabaseTagTypes.VokabularDB)]
    [MigrationTypeTags(CoreMigrationTypeTagTypes.Structure, CoreMigrationTypeTagTypes.All)]
    [Migration(015)]
    public class M_015_HeadwordItemIndex : ForwardOnlyMigration
    {
        public override void Up()
        {
            Create.Index("IDX_HeadwordItem(Headword)").OnTable("HeadwordItem").OnColumn("Headword");
            
            Create.Index("IDX_HeadwordItem(HeadwordResource)").OnTable("HeadwordItem").OnColumn("HeadwordResource");
        }
    }
}