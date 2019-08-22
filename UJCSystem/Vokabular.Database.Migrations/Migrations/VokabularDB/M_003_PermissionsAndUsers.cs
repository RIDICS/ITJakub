using FluentMigrator;
using Ridics.DatabaseMigrator.Shared.TagsAttributes;

namespace Vokabular.Database.Migrations.Migrations.VokabularDB
{
    [DatabaseTags(DatabaseTagTypes.VokabularDB)]
    [MigrationTypeTags(CoreMigrationTypeTagTypes.Structure, CoreMigrationTypeTagTypes.All)]
    [Migration(003)]
    public class M_003_PermissionsAndUsers : ForwardOnlyMigration
    {
        public override void Up()
        {
            Create.Table("UserGroup")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey("PK_UserGroup(Id)").Identity()
                .WithColumn("Name").AsString(255).Nullable()
                .WithColumn("CreateTime").AsDateTime().NotNullable()
                .WithColumn("LastChange").AsDateTime().NotNullable()
                .WithColumn("ExternalId").AsInt32().NotNullable().Unique("UQ_UserGroup(ExternalId)");

            Create.Table("User_UserGroup")
                .WithColumn("User").AsInt32().NotNullable()
                .ForeignKey("FK_User_UserGroup(User)_User(Id)", "User", "Id")
                .WithColumn("UserGroup").AsInt32().NotNullable()
                .ForeignKey("FK_User_UserGroup(UserGroup)_UserGroup(Id)", "UserGroup", "Id");
            Create.PrimaryKey("PK_User_UserGroup(User)_User_UserGroup(UserGroup)").OnTable("User_UserGroup")
                .Columns("User", "UserGroup");

            Create.Table("Permission")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey("PK_Permission(Id)").Identity()
                .WithColumn("UserGroup").AsInt32().NotNullable()
                .ForeignKey("FK_Permission(UserGroup)_UserGroup(Id)", "UserGroup", "Id")
                .WithColumn("Project").AsInt64().NotNullable()
                .ForeignKey("FK_Permission(Project)_Project(Id)", "Project", "Id");
            Create.UniqueConstraint("UQ_Permission(UserGroup_Project)").OnTable("Permission")
                .Columns("UserGroup", "Project");
        }
    }
}