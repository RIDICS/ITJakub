using FluentMigrator;
using Ridics.DatabaseMigrator.Shared.TagsAttributes;

namespace Vokabular.Database.Migrations.Migrations.VokabularDB
{
    [DatabaseTags(DatabaseTagTypes.VokabularDB)]
    [MigrationTypeTags(CoreMigrationTypeTagTypes.Structure, CoreMigrationTypeTagTypes.All)]
    [Migration(004)]
    public class M_004_LemmatizationService : ForwardOnlyMigration
    {
        public override void Up()
        {
            Create.Table("HyperCanonicalForm")
                .WithColumn("Id").AsInt64().NotNullable().PrimaryKey("PK_HyperCanonicalForm(Id)").Identity()
                .WithColumn("Text").AsString(255).NotNullable()
                .WithColumn("Type").AsByte().NotNullable()
                .WithColumn("Description").AsString(255).Nullable();

            Create.Table("Token")
                .WithColumn("Id").AsInt64().NotNullable().PrimaryKey("PK_Token(Id)").Identity()
                .WithColumn("Text").AsString(255).NotNullable().Unique("UQ_Token(Text)")
                .WithColumn("Description").AsString(255).NotNullable();

            Create.Table("TokenCharacteristic")
                .WithColumn("Id").AsInt64().NotNullable().PrimaryKey("PK_TokenCharacteristic(Id)").Identity()
                .WithColumn("Token").AsInt64().Nullable().ForeignKey("FK_TokenCharacteristic(Token)_Token(Id)", "Token", "Id")
                .WithColumn("MorphologicalCharakteristic").AsString(17).NotNullable()
                .WithColumn("Description").AsString(255).NotNullable();

            Create.Table("CanonicalForm")
                .WithColumn("Id").AsInt64().NotNullable().PrimaryKey("PK_CanonicalForm(Id)").Identity()
                .WithColumn("Text").AsString(255).NotNullable()
                .WithColumn("Type").AsByte().NotNullable()
                .WithColumn("Description").AsString(255).NotNullable()
                .WithColumn("HyperCanonicalForm").AsInt64().Nullable().ForeignKey("FK_CanonicalForm(HyperCanonicalForm)_HyperCanonicalForm(Id)", "HyperCanonicalForm", "Id");

            Create.Table("CanonicalForm_TokenCharacteristic")
                .WithColumn("Id").AsInt64().NotNullable().PrimaryKey("PK_CanonicalForm_TokenCharacteristic(Id)").Identity()
                .WithColumn("TokenCharacteristic").AsInt64().NotNullable().ForeignKey("FK_CanonicalForm_TokenCharacteristic(TokenCharacteristic)_TokenCharacteristic(Id)", "TokenCharacteristic", "Id")
                .WithColumn("CanonicalForm").AsInt64().Nullable().ForeignKey("FK_CanonicalForm_TokenCharacteristic(CanonicalForm)_CanonicalForm(Id)", "CanonicalForm", "Id");
        }
    }
}