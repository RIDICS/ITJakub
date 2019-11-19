using FluentMigrator;
using Ridics.DatabaseMigrator.Shared.TagsAttributes;

namespace Vokabular.Database.Migrations.Migrations.VokabularDB
{
    [DatabaseTags(DatabaseTagTypes.VokabularDB)]
    [MigrationTypeTags(CoreMigrationTypeTagTypes.Structure, CoreMigrationTypeTagTypes.All)]
    [Migration(016)]
    public class M_016_FixFeedbackAuthorEmail : ForwardOnlyMigration
    {
        public override void Up()
        {
            Rename.Column("AuthorTitle").OnTable("Feedback").To("AuthorEmail");
        }
    }
}