using FluentMigrator;
using Ridics.DatabaseMigrator.Shared.TagsAttributes;

namespace Vokabular.Database.Migrations.Migrations.VokabularDB
{
    [DatabaseTags(DatabaseTagTypes.VokabularDB)]
    [MigrationTypeTags(CoreMigrationTypeTagTypes.Structure, CoreMigrationTypeTagTypes.All)]
    [Migration(017)]
    public class M_017_ProjectGroup : ForwardOnlyMigration
    {
        public override void Up()
        {
            const int transcribedTextEnumVal = 2;

            Create.Table("ProjectGroup")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey("PK_ProjectGroup(Id)").Identity()
                .WithColumn("CreateTime").AsDateTime().NotNullable();

            Alter.Table("Project")
                .AddColumn("TextType").AsInt16().NotNullable().SetExistingRowsTo(transcribedTextEnumVal)
                .AddColumn("ProjectGroup").AsInt32().Nullable().ForeignKey("FK_Project(ProjectGroup)_ProjectGroup(Id)", "ProjectGroup", "Id");
        }
    }
}