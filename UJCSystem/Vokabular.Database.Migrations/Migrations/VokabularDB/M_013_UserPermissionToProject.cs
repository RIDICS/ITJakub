using FluentMigrator;
using Ridics.DatabaseMigrator.Shared.TagsAttributes;

namespace Vokabular.Database.Migrations.Migrations.VokabularDB
{
    [DatabaseTags(DatabaseTagTypes.VokabularDB)]
    [MigrationTypeTags(CoreMigrationTypeTagTypes.Structure, CoreMigrationTypeTagTypes.All)]
    [Migration(013)]
    public class M_013_UserPermissionToProject : ForwardOnlyMigration
    {
        public override void Up()
        {
            Delete.Index("UQ_UserGroup(ExternalId)").OnTable("UserGroup");

            Alter.Table("UserGroup")
                .AlterColumn("ExternalId").AsInt32().Nullable()
                .AddColumn("Discriminator").AsString(10).NotNullable().SetExistingRowsTo("Group")
                .AddColumn("User").AsInt32().Nullable().ForeignKey("FK_UserGroup(User)_User(Id)", "User", "Id");

            Create.UniqueConstraint("UQ_UserGroup(ExternalId_User)").OnTable("UserGroup")
                .Columns("ExternalId", "User");
        }
    }
}