using FluentMigrator;
using Ridics.DatabaseMigrator.Shared.TagsAttributes;

namespace Vokabular.Database.Migrations.Migrations.VokabularDB
{
    [DatabaseTags(DatabaseTagTypes.VokabularDB)]
    [MigrationTypeTags(CoreMigrationTypeTagTypes.Structure, CoreMigrationTypeTagTypes.All)]
    [Migration(007)]
    public class M_007_MapVokabularForum : ForwardOnlyMigration
    {
        public override void Up()
        {
            Create.Column("ForumID").OnTable("Project").AsInt32().Nullable();
        }
    }
}