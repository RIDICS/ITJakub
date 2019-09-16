using FluentMigrator;
using Ridics.DatabaseMigrator.Shared.TagsAttributes;

namespace Vokabular.Database.Migrations.Migrations.VokabularDB
{
    [DatabaseTags(DatabaseTagTypes.VokabularDB)]
    [MigrationTypeTags(CoreMigrationTypeTagTypes.Structure, CoreMigrationTypeTagTypes.All)]
    [Migration(009)]
    public class M_009_AddEditionNoteText : ForwardOnlyMigration
    {
        public override void Up()
        {
            Alter.Table("EditionNoteResource").AddColumn("Text").AsString(int.MaxValue).Nullable();

            Delete.Column("ExternalId").FromTable("EditionNoteResource");
        }
    }
}