using System;
using System.Data;
using FluentMigrator;
using Ridics.DatabaseMigrator.Shared.TagsAttributes;
using Vokabular.Database.Migrations.Extensions;

namespace Vokabular.Database.Migrations.Migrations.WebHub
{
    [DatabaseTags(DatabaseTagTypes.VokabularWebDB)]
    [MigrationTypeTags(CoreMigrationTypeTagTypes.Structure, CoreMigrationTypeTagTypes.All)]
    [Migration(001)]
    public class M_001_InitialSchema : ForwardOnlyMigration
    {
        public override void Up()
        {
            Create.Table("DictionaryScope")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey("PK_DictionaryScope(Id)").Identity()
                .WithColumn("Name").AsString(255).NotNullable().Unique("UQ_DictionaryScope(Name)");

            Create.Table("Culture")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey("PK_Culture(Id)").Identity()
                .WithColumn("Name").AsString(5).NotNullable().Unique("UQ_Culture(Name)");

            Create.Table("CultureHierarchy")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey("PK_CultureHierarchy(Id)").Identity()
                .WithColumn("Culture").AsInt32().NotNullable().ForeignKey("FK_CultureHierarchy(Culture)_Culture(Id)", "Culture", "Id").OnDelete(Rule.None)
                .WithColumn("ParentCulture").AsInt32().NotNullable().ForeignKey("FK_CultureHierarchy(ParentCulture)_Culture(Id)", "Culture", "Id").OnDelete(Rule.None)
                .WithColumn("LevelProperty").AsByte().NotNullable();
            Create.UniqueConstraint("UQ_CultureHierarchy(Culture,ParentCulture)").OnTable("CultureHierarchy")
                .Columns("Culture", "ParentCulture");

            Create.Table("BaseText")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey("PK_BaseText(Id)").Identity()
                .WithColumn("Culture").AsInt32().NotNullable().ForeignKey("FK_BaseText(Culture)_Culture(Id)", "Culture", "Id")
                .WithColumn("DictionaryScope").AsInt32().NotNullable().ForeignKey("FK_BaseText(DictionaryScope)_DictionaryScope(Id)", "DictionaryScope", "Id").OnDelete(Rule.None)
                .WithColumn("Name").AsString(255).NotNullable()
                .WithColumn("Format").AsByte().NotNullable()
                .WithColumn("Discriminator").AsMaxString().NotNullable()
                .WithColumn("Text").AsMaxString().NotNullable()
                .WithColumn("ModificationTime").AsDateTime().NotNullable().WithDefaultValue(DateTime.UtcNow)
                .WithColumn("ModificationUser").AsString(255).Nullable();
            Create.UniqueConstraint("UQ_BaseText(Culture,DictionaryScope,Name)").OnTable("BaseText")
                .Columns("Culture", "DictionaryScope", "Name");

            Create.Table("IntervalText")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey("PK_IntervalText(Id)").Identity()
                .WithColumn("IntervalEnd").AsInt32().NotNullable()
                .WithColumn("IntervalStart").AsInt32().NotNullable()
                .WithColumn("Text").AsMaxString().NotNullable()
                .WithColumn("PluralizedStaticText").AsInt32().NotNullable().ForeignKey("FK_IntervalText(PluralizedStaticText)_PluralizedStaticText(Id)", "BaseText", "Id").OnDelete(Rule.None);
        }
    }
}