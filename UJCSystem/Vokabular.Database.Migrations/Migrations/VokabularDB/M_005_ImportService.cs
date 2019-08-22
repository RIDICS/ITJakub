using FluentMigrator;
using Ridics.DatabaseMigrator.Shared.TagsAttributes;
using Vokabular.Database.Migrations.Extensions;

namespace Vokabular.Database.Migrations.Migrations.VokabularDB
{
    [DatabaseTags(DatabaseTagTypes.VokabularDB)]
    [MigrationTypeTags(CoreMigrationTypeTagTypes.Structure, CoreMigrationTypeTagTypes.All)]
    [Migration(005)]
    public class M_005_ImportService : ForwardOnlyMigration
    {
        public override void Up()
        {
            Create.Table("ExternalRepositoryType")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey("PK_ExternalRepositoryType(Id)").Identity()
                .WithColumn("Name").AsString(100).NotNullable().Unique("UQ_ExternalRepositoryType(Name)");

            Create.Table("BibliographicFormat")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey("PK_BibliographicFormat(Id)").Identity()
                .WithColumn("Name").AsString(50).NotNullable().Unique("UQ_BibliographicFormat(Name)");

            Create.Table("ExternalRepository")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey("PK_ExternalRepository(Id)").Identity()
                .WithColumn("Name").AsString(255).NotNullable()
                .WithColumn("Description").AsMaxString().Nullable()
                .WithColumn("Url").AsString(255).NotNullable()
                .WithColumn("UrlTemplate").AsString(512).Nullable()
                .WithColumn("License").AsMaxString().Nullable()
                .WithColumn("Configuration").AsMaxString().Nullable()
                .WithColumn("CreatedByUser").AsInt32().NotNullable()
                .ForeignKey("FK_ExternalRepository(CreatedByUser)_User(Id)", "User", "Id")
                .WithColumn("BibliographicFormat").AsInt32().NotNullable()
                .ForeignKey("FK_ExternalRepository(BibliographicFormat)_BibliographicFormat(Id)", "BibliographicFormat", "Id")
                .WithColumn("ExternalRepositoryType").AsInt32().NotNullable()
                .ForeignKey("FK_ExternalRepository(ExternalRepositoryType)_ExternalRepositoryType(Id)", "ExternalRepositoryType", "Id");

            Create.Table("ImportHistory")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey("PK_ImportHistory(Id)").Identity()
                .WithColumn("Date").AsDateTime().NotNullable()
                .WithColumn("Status").AsByte().NotNullable()
                .WithColumn("Message").AsMaxString().Nullable()
                .WithColumn("CreatedByUser").AsInt32().NotNullable()
                .ForeignKey("FK_ImportHistory(CreatedByUser)_User(Id)", "User", "Id")
                .WithColumn("ExternalRepository").AsInt32().NotNullable()
                .ForeignKey("FK_ImportHistory(ExternalRepository)_ExternalRepository(Id)", "ExternalRepository", "Id");

            Create.Table("ImportedProjectMetadata")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey("PK_ImportedProjectMetadata(Id)").Identity()
                .WithColumn("ExternalId").AsString(50).NotNullable()
                .WithColumn("ExternalRepository").AsInt32().NotNullable()
                .ForeignKey("FK_ImportedProjectMetadata(ExternalRepository)_ExternalRepository(Id)", "ExternalRepository", "Id")
                .WithColumn("Project").AsInt64().NotNullable()
                .ForeignKey("FK_ImportedProjectMetadata(Project)_Project(Id)", "Project", "Id");
            Create.UniqueConstraint("UQ_ImportedProjectMetadata(ExternalId)(ExternalRepository)")
                .OnTable("ImportedProjectMetadata").Columns("ExternalId", "ExternalRepository");

            Create.Table("ImportedRecordMetadata")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey("PK_ImportedRecordMetadata(Id)").Identity()
                .WithColumn("LastUpdateMessage").AsMaxString().Nullable()
                .WithColumn("LastUpdate").AsInt32().NotNullable()
                .ForeignKey("FK_ImportedRecordMetadata(LastUpdate)_ImportHistory(Id)", "ImportHistory", "Id")
                .WithColumn("ImportedProjectMetadata").AsInt32().Nullable()
                .ForeignKey("FK_ImportedRecordMetadata(ImportedProjectMetadata)_ImportedProjectMetadata(Id)", "ImportedProjectMetadata", "Id")
                .WithColumn("Snapshot").AsInt64().Nullable()
                .ForeignKey("FK_ImportedRecordMetadata(Snapshot)_Snapshot(Id)", "Snapshot", "Id");

            Create.Table("FilteringExpressionSet")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey("PK_FilteringExpressionSet(Id)").Identity()
                .WithColumn("Name").AsMaxString().NotNullable()
                .WithColumn("BibliographicFormat").AsInt32().NotNullable()
                .ForeignKey("FK_FilteringExpressionSet(BibliographicFormat)_BibliographicFormat(Id)", "BibliographicFormat", "Id")
                .WithColumn("CreatedByUser").AsInt32().NotNullable()
                .ForeignKey("FK_FilteringExpressionSet(CreatedByUser)_User(Id)", "User", "Id");

            Create.Table("ExternalRepository_FilteringExpressionSet")
                .WithColumn("ExternalRepository").AsInt32().NotNullable()
                .ForeignKey("FK_ExternalRepository_FilteringExpressionSet(ExternalRepository)_ExternalRepository(Id)", "ExternalRepository", "Id")
                .WithColumn("FilteringExpressionSet").AsInt32().NotNullable()
                .ForeignKey("FK_ExternalRepository_FilteringExpressionSet(FilteringExpressionSet)_FilteringExpressionSet(Id)", "FilteringExpressionSet", "Id");
            Create.PrimaryKey("PK_ExternalRepository_FilteringExpressionSet(ExternalRepository)_ExternalRepo_FilteringExpressionSet(FilteringExpressionSet)")
                .OnTable("ExternalRepository_FilteringExpressionSet").Columns("ExternalRepository", "FilteringExpressionSet");
            
            Create.Table("FilteringExpression")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey("PK_FilteringExpression(Id)").Identity()
                .WithColumn("Value").AsString(255).NotNullable()
                .WithColumn("Field").AsString(255).NotNullable()
                .WithColumn("FilteringExpressionSet").AsInt32().NotNullable()
                .ForeignKey("FK_FilteringExpression(FilteringExpressionSet)_FilteringExpressionSet(Id)", "FilteringExpressionSet", "Id");

            Create.Column("OriginalUrl").OnTable("Project").AsString(512).Nullable();
        }
    }
}