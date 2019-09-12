using FluentMigrator;
using Ridics.DatabaseMigrator.Shared.TagsAttributes;

namespace Vokabular.Database.Migrations.Migrations.VokabularDB
{
    [DatabaseTags(DatabaseTagTypes.VokabularDB)]
    [MigrationTypeTags(CoreMigrationTypeTagTypes.Structure, CoreMigrationTypeTagTypes.All)]
    [Migration(010)]
    public class M_010_AllowNullParentComment : ForwardOnlyMigration
    {
        public override void Up()
        {
            Alter.Table("TextComment").AlterColumn("ParentComment").AsInt64().Nullable();
        }
    }
}