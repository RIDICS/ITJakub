using FluentMigrator;
using Ridics.DatabaseMigrator.Shared.TagsAttributes;

namespace Vokabular.Database.Migrations.Migrations.VokabularDB
{
    [DatabaseTags(DatabaseTagTypes.VokabularDB)]
    [MigrationTypeTags(CoreMigrationTypeTagTypes.Structure, CoreMigrationTypeTagTypes.All)]
    [Migration(014)]
    public class M_014_AdditionalUserInfo : ForwardOnlyMigration
    {
        public override void Up()
        {
            Alter.Table("User")
                .AddColumn("ExtUsername").AsString(100).Nullable()
                .AddColumn("ExtFirstName").AsString(100).Nullable()
                .AddColumn("ExtLastName").AsString(100).Nullable();
        }
    }
}