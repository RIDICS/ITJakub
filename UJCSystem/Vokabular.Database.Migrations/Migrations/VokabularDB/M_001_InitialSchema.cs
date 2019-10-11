using System;
using FluentMigrator;
using Ridics.DatabaseMigrator.Shared.TagsAttributes;
using Vokabular.Database.Migrations.Extensions;

namespace Vokabular.Database.Migrations.Migrations.VokabularDB
{
    [DatabaseTags(DatabaseTagTypes.VokabularDB)]
    [MigrationTypeTags(CoreMigrationTypeTagTypes.Structure, CoreMigrationTypeTagTypes.All)]
    [Migration(001)]
    public class M_001_InitialSchema : ForwardOnlyMigration
    {
        public override void Up()
        {
            // Create generic tables

            Create.Table("User")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey("PK_User(Id)").Identity()
                .WithColumn("CreateTime").AsDateTime().NotNullable().WithDefaultValue(DateTime.UtcNow)
                .WithColumn("ExternalId").AsInt32().Nullable().Unique("UQ_User(ExternalId)");

            Create.Table("OriginalAuthor")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey("PK_OriginalAuthor(Id)").Identity()
                .WithColumn("FirstName").AsString(50).NotNullable()
                .WithColumn("LastName").AsString(100).NotNullable();

            Create.UniqueConstraint("UQ_OriginalAuthor(FirstName)(LastName)")
                .OnTable("OriginalAuthor").Columns("FirstName", "LastName");

            Create.Table("ResponsibleType")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey("PK_ResponsibleType(Id)").Identity()
                .WithColumn("Text").AsString(100).NotNullable().Unique("UQ_ResponsibleType(Text)")
                .WithColumn("Type").AsInt16().NotNullable();

            Create.Table("ResponsiblePerson")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey("PK_ResponsiblePerson(Id)").Identity()
                .WithColumn("FirstName").AsString(50).NotNullable()
                .WithColumn("LastName").AsString(50).NotNullable();

            Create.UniqueConstraint("UQ_ResponsiblePerson(FirstName)(LastName)")
                .OnTable("ResponsiblePerson").Columns("FirstName", "LastName");

            Create.Table("BookType")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey("PK_BookType(Id)").Identity()
                .WithColumn("Type").AsInt16().NotNullable().Unique("UQ_BookType(Type)");


            Create.Table("Category")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey("PK_Category(Id)").Identity()
                .WithColumn("ExternalId").AsString(150).Nullable()
                .WithColumn("Description").AsString(150).Nullable()
                .WithColumn("Path").AsMaxString().Nullable()
                .WithColumn("ParentCategory").AsInt32().Nullable().ForeignKey("FK_Category(ParentCategory)_Category(Id)", "Category", "Id");

            Create.Table("LiteraryGenre")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey("PK_LiteraryGenre(Id)").Identity()
                .WithColumn("Name").AsString(255).NotNullable().Unique("UQ_LiteraryGenre(Name)");

            Create.Table("LiteraryKind")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey("PK_LiteraryKind(Id)").Identity()
                .WithColumn("Name").AsString(255).NotNullable().Unique("UQ_LiteraryKind(Name)");

            Create.Table("LiteraryOriginal")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey("PK_LiteraryOriginal(Id)").Identity()
                .WithColumn("Name").AsString(255).NotNullable().Unique("UQ_LiteraryOriginal(Name)");

            Create.Table("TermCategory")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey("PK_TermCategory(Id)").Identity()
                .WithColumn("Name").AsString(255).NotNullable().Unique("UQ_TermCategory(Name)");

            Create.Table("Term")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey("PK_Term(Id)").Identity()
                .WithColumn("Text").AsString(255).NotNullable().Unique("UQ_Term(Text)")
                .WithColumn("ExternalId").AsString(255).Nullable()
                .WithColumn("Position").AsInt64().NotNullable()
                .WithColumn("TermCategory").AsInt32().ForeignKey("FK_Term(TermCategory)_TermCategory(Id)", "TermCategory", "Id");

            Create.Table("Keyword")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey("PK_Keyword(Id)").Identity()
                .WithColumn("Text").AsString(255).NotNullable().Unique("UQ_Keyword(Text)");

            // Create project and resource tables

            Create.Table("Project")
                .WithColumn("Id").AsInt64().NotNullable().PrimaryKey("PK_Project(Id)").Identity()
                .WithColumn("Name").AsString(2000).NotNullable()
                .WithColumn("CreateTime").AsDateTime().NotNullable()
                .WithColumn("ExternalId").AsString(255).Nullable()
                .WithColumn("CreatedByUser").AsInt32().NotNullable().ForeignKey("FK_Project(CreatedByUser)_User(Id)", "User", "Id");

            Create.Table("NamedResourceGroup")
                .WithColumn("Id").AsInt64().NotNullable().PrimaryKey("PK_NamedResourceGroup(Id)").Identity()
                .WithColumn("Name").AsString(255).NotNullable()
                .WithColumn("TextType").AsInt16().NotNullable()
                .WithColumn("Project").AsInt64().NotNullable().ForeignKey("FK_NamedResourceGroup(Project)_Project(Id)", "Project", "Id");

            Create.Table("Resource")
                .WithColumn("Id").AsInt64().NotNullable().PrimaryKey("PK_Resource(Id)").Identity()
                .WithColumn("Name").AsString(2000).NotNullable()
                .WithColumn("ResourceType").AsInt16().NotNullable()
                .WithColumn("ContentType").AsInt16().NotNullable()
                .WithColumn("IsRemoved").AsBoolean().NotNullable()
                .WithColumn("Project").AsInt64().NotNullable().ForeignKey("FK_Resource(Project)_Project(Id)", "Project", "Id")
                .WithColumn("NamedResourceGroup").AsInt64().Nullable().ForeignKey("FK_Resource(NamedResourceGroup)_NamedResourceGroup(Id)", "NamedResourceGroup", "Id");

            Create.Table("ResourceVersion")
                .WithColumn("Id").AsInt64().NotNullable().PrimaryKey("PK_ResourceVersion(Id)").Identity()
                .WithColumn("VersionNumber").AsInt32().NotNullable()
                .WithColumn("CreateTime").AsDateTime().NotNullable()
                .WithColumn("Comment").AsString(2000).Nullable()
                .WithColumn("Resource").AsInt64().NotNullable().ForeignKey("FK_ResourceVersion(Resource)_Resource(Id)", "Resource", "Id")
                .WithColumn("CreatedByUser").AsInt32().NotNullable().ForeignKey("FK_ResourceVersion(CreatedByUser)_User(Id)", "User", "Id");
            Create.UniqueConstraint("UQ_ResourceVersion(Id)(VersionNumber)")
                .OnTable("ResourceVersion").Columns("Id", "VersionNumber");

            Create.Column("LatestVersion").OnTable("Resource").AsInt64().Nullable()
                .ForeignKey("FK_Resource(LatestVersion)_ResourceVersion(Id)", "ResourceVersion", "Id");

            Create.Table("BookVersionResource")
                .WithColumn("ResourceVersionId").AsInt64().NotNullable().PrimaryKey("PK_BookVersionResource(ResourceVersionId)")
                .ForeignKey("ResourceVersion", "Id")
                .WithColumn("ExternalId").AsString(255).Nullable();

            Create.Table("MetadataResource")
                .WithColumn("ResourceVersionId").AsInt64().NotNullable().PrimaryKey("PK_MetadataResource(ResourceVersionId)")
                .ForeignKey("ResourceVersion", "Id").Nullable()
                .WithColumn("AuthorsLabel").AsString(2000).Nullable()
                .WithColumn("Title").AsString(2000).Nullable()
                .WithColumn("SubTitle").AsString(2000).Nullable()
                .WithColumn("RelicAbbreviation").AsString(100).Nullable()
                .WithColumn("SourceAbbreviation").AsString(255).Nullable()
                .WithColumn("PublishPlace").AsString(100).Nullable()
                .WithColumn("PublishDate").AsString(50).Nullable()
                .WithColumn("PublisherText").AsString(2000).Nullable()
                .WithColumn("PublisherEmail").AsString(255).Nullable()
                .WithColumn("Copyright").AsMaxString().Nullable()
                .WithColumn("BiblText").AsMaxString().Nullable()
                .WithColumn("OriginDate").AsString(50).Nullable()
                .WithColumn("NotBefore").AsDate().Nullable()
                .WithColumn("NotAfter").AsDate().Nullable()
                .WithColumn("ManuscriptIdno").AsString(100).Nullable()
                .WithColumn("ManuscriptSettlement").AsString(100).Nullable()
                .WithColumn("ManuscriptCountry").AsString(100).Nullable()
                .WithColumn("ManuscriptRepository").AsString(200).Nullable()
                .WithColumn("ManuscriptExtent").AsString(2000).Nullable()
                .WithColumn("ManuscriptTitle").AsString(2000).Nullable();

            Create.Table("PageResource")
                .WithColumn("ResourceVersionId").AsInt64().NotNullable().PrimaryKey("PK_PageResource(ResourceVersionId)").ForeignKey("ResourceVersion", "Id")
                .WithColumn("Name").AsString(50).NotNullable()
                .WithColumn("Position").AsInt32().NotNullable();

            Create.Table("TextResource")
                .WithColumn("ResourceVersionId").AsInt64().NotNullable().PrimaryKey("PK_TextResource(ResourceVersionId)").ForeignKey("ResourceVersion", "Id")
                .WithColumn("ExternalId").AsString(100).Nullable()
                .WithColumn("ResourcePage").AsInt64().Nullable().ForeignKey("FK_TextResource(ResourcePage)_Resource(Id)", "Resource", "Id")
                .WithColumn("BookVersion").AsInt64().Nullable().ForeignKey("FK_TextResource(BookVersion)_BookVersionResource(ResourceVersionId)", "BookVersionResource", "ResourceVersionId");

            Create.Table("ImageResource")
                .WithColumn("ResourceVersionId").AsInt64().NotNullable().PrimaryKey("PK_ImageResource(ResourceVersionId)").ForeignKey("ResourceVersion", "Id")
                .WithColumn("FileName").AsString(255).NotNullable()
                .WithColumn("FileId").AsString(100).Nullable()
                .WithColumn("MimeType").AsString(255).NotNullable()
                .WithColumn("Size").AsInt64().NotNullable()
                .WithColumn("ResourcePage").AsInt64().Nullable().ForeignKey("FK_ImageResource(ResourcePage)_Resource(Id)", "Resource", "Id");

            Create.Table("AudioResource")
                .WithColumn("ResourceVersionId").AsInt64().NotNullable().PrimaryKey("PK_AudioResource(ResourceVersionId)").ForeignKey("ResourceVersion", "Id")
                .WithColumn("Duration").AsInt64().Nullable()
                .WithColumn("FileName").AsString(255).NotNullable()
                .WithColumn("FileId").AsString(100).Nullable()
                .WithColumn("AudioType").AsByte().NotNullable()
                .WithColumn("MimeType").AsString(255).NotNullable()
                .WithColumn("ResourceTrack").AsInt64().Nullable().ForeignKey("FK_AudioResource(ResourceTrack)_Resource(Id)", "Resource", "Id");

            Create.Table("TrackResource")
                .WithColumn("ResourceVersionId").AsInt64().NotNullable().PrimaryKey("PK_TrackResource(ResourceVersionId)").ForeignKey("ResourceVersion", "Id")
                .WithColumn("Name").AsString(50).NotNullable()
                .WithColumn("Text").AsMaxString().Nullable()
                .WithColumn("Position").AsInt16().NotNullable()
                .WithColumn("ResourceChapter").AsInt64().Nullable().ForeignKey("FK_TrackResource(ResourceChapter)_Resource(Id)", "Resource", "Id")
                .WithColumn("ResourceBeginningPage").AsInt64().Nullable().ForeignKey("FK_TrackResource(ResourceBeginningPage)_Resource(Id)", "Resource", "Id");

            Create.Table("ChapterResource")
                .WithColumn("ResourceVersionId").AsInt64().NotNullable().PrimaryKey("PK_ChapterResource(ResourceVersionId)")
                .ForeignKey("ResourceVersion", "Id")
                .WithColumn("Name").AsString(1000).NotNullable()
                .WithColumn("Position").AsInt16().NotNullable()
                .WithColumn("ParentResource").AsInt64().Nullable().ForeignKey("FK_ChapterResource(ParentResource)_Resource(Id)", "Resource", "Id")
                .WithColumn("ResourceBeginningPage").AsInt64().Nullable().ForeignKey("FK_ChapterResource(ResourceBeginningPage)_Resource(Id)", "Resource", "Id");

            Create.Table("HeadwordResource")
                .WithColumn("ResourceVersionId").AsInt64().NotNullable().PrimaryKey("PK_HeadwordResource(ResourceVersionId)").ForeignKey("ResourceVersion", "Id")
                .WithColumn("ExternalId").AsString(100).NotNullable()
                .WithColumn("DefaultHeadword").AsString(255).NotNullable()
                .WithColumn("Sorting").AsString(255).NotNullable()
                .WithColumn("BookVersion").AsInt64().Nullable().ForeignKey("FK_HeadwordResource(BookVersion)_BookVersionResource(ResourceVersionId)", "BookVersionResource", "ResourceVersionId");

            // HeadwordItem is not Resource but it is part of HeadwordResource
            Create.Table("HeadwordItem")
                .WithColumn("Id").AsInt64().NotNullable().PrimaryKey("PK_HeadwordItem(Id)").Identity()
                .WithColumn("HeadwordResource").AsInt64().NotNullable().ForeignKey("FK_HeadwordItem(HeadwordResource)_HeadwordResource(ResourceVersionId)", "HeadwordResource", "ResourceVersionId")
                .WithColumn("Headword").AsString(255).NotNullable()
                .WithColumn("HeadwordOriginal").AsString(255).Nullable()
                .WithColumn("ResourcePage").AsInt64().Nullable().ForeignKey("FK_HeadwordItem(ResourcePage)_Resource(Id)", "Resource", "Id");

            Create.Table("BinaryResource")
                .WithColumn("ResourceVersionId").AsInt64().NotNullable().PrimaryKey("PK_BinaryResource(ResourceVersionId)").ForeignKey("ResourceVersion", "Id")
                .WithColumn("Name").AsString(255).NotNullable()
                .WithColumn("FileName").AsString(255).NotNullable()
                .WithColumn("FileId").AsString(100).Nullable();

            Create.Table("EditionNoteResource")
                .WithColumn("ResourceVersionId").AsInt64().NotNullable().PrimaryKey("PK_EditionNoteResource(ResourceVersionId)").ForeignKey("ResourceVersion", "Id")
                .WithColumn("ExternalId").AsString(100).Nullable()
                .WithColumn("BookVersion").AsInt64().Nullable().ForeignKey("FK_EditionNoteResource(BookVersion)_BookVersionResource(ResourceVersionId)", "BookVersionResource", "ResourceVersionId");

            // Other tables

            Create.Table("FavoriteLabel")
                .WithColumn("Id").AsInt64().NotNullable().PrimaryKey("PK_FavoriteLabel(Id)").Identity()
                .WithColumn("Name").AsString(150).NotNullable()
                .WithColumn("Color").AsString(7).NotNullable()
                .WithColumn("User").AsInt32().NotNullable().ForeignKey("FK_FavoriteLabel(User)_User(Id)", "User", "Id")
                //.WithColumn("ParentLabel").AsInt64().Nullable().ForeignKey("FK_FavoriteLabel(ParentLabel)_FavoriteLabel(Id)", "FavoriteLabel", "Id")
                .WithColumn("IsDefault").AsBoolean().NotNullable()
                .WithColumn("LastUseTime").AsDateTime().Nullable();

            Create.Table("Favorite")
                .WithColumn("Id").AsInt64().NotNullable().PrimaryKey("PK_Favorite(Id)").Identity()
                .WithColumn("FavoriteType").AsString(255).NotNullable()
                .WithColumn("FavoriteLabel").AsInt64().NotNullable().ForeignKey("FavoriteLabel", "Id")
                .WithColumn("Title").AsString(255).Nullable()
                .WithColumn("Description").AsString(2000).Nullable() //TODO is required?
                .WithColumn("CreateTime").AsDateTime().Nullable()
                .WithColumn("Project").AsInt64().Nullable().ForeignKey("Project", "Id")
                .WithColumn("Category").AsInt32().Nullable().ForeignKey("Category", "Id")
                .WithColumn("Resource").AsInt64().Nullable().ForeignKey("Resource", "Id")
                .WithColumn("ResourceVersion").AsInt64().Nullable().ForeignKey("ResourceVersion", "Id")
                //TODO FavoriteSnapshot
                .WithColumn("BookType").AsInt32().Nullable().ForeignKey("BookType", "Id")
                .WithColumn("Query").AsMaxString().Nullable()
                .WithColumn("QueryType").AsInt16().Nullable();

            Create.Table("Feedback")
                .WithColumn("Id").AsInt64().NotNullable().PrimaryKey("PK_Feedback(Id)").Identity()
                .WithColumn("FeedbackType").AsString(255).NotNullable()
                .WithColumn("Text").AsString(2000).NotNullable()
                .WithColumn("CreateTime").AsDateTime().NotNullable()
                .WithColumn("AuthorName").AsString(255).Nullable()
                .WithColumn("AuthorEmail").AsString(255).Nullable()
                .WithColumn("AuthorUser").AsInt32().Nullable().ForeignKey("FK_Feedback(AuthorUser)_User(Id)", "User", "Id")
                .WithColumn("FeedbackCategory").AsInt16().NotNullable()
                .WithColumn("Project").AsInt64().Nullable().ForeignKey("FK_Feedback(Project)_Project(Id)", "Project", "Id")
                .WithColumn("ResourceVersion").AsInt64().Nullable().ForeignKey("FK_Feedback(ResourceVersion)_ResourceVersion(Id)", "ResourceVersion", "Id");

            Create.Table("NewsSyndicationItem")
                .WithColumn("Id").AsInt64().NotNullable().PrimaryKey("PK_NewsSyndicationItem(Id)").Identity()
                .WithColumn("Title").AsString(255).NotNullable()
                .WithColumn("CreateTime").AsDateTime().NotNullable()
                .WithColumn("Text").AsString(2000).NotNullable()
                .WithColumn("Url").AsMaxString().NotNullable()
                .WithColumn("ItemType").AsInt16().NotNullable()
                .WithColumn("User").AsInt32().Nullable().ForeignKey("FK_NewsSyndicationItem(User)_User(Id)", "User", "Id");

            Create.Table("Transformation")
                .WithColumn("Id").AsInt64().NotNullable().PrimaryKey("PK_Transformation(Id)").Identity()
                .WithColumn("Name").AsString(100).NotNullable()
                .WithColumn("Description").AsString(2000).Nullable()
                .WithColumn("OutputFormat").AsInt16().NotNullable()
                .WithColumn("BookType").AsInt32().Nullable().ForeignKey("FK_Transformation(BookType)_BookType(Id)", "BookType", "Id")
                .WithColumn("IsDefaultForBookType").AsBoolean().NotNullable()
                .WithColumn("ResourceLevel").AsInt16().NotNullable();

            Create.Table("Snapshot")
                .WithColumn("Id").AsInt64().NotNullable().PrimaryKey("PK_Snapshot(Id)").Identity()
                .WithColumn("VersionNumber").AsInt32().NotNullable()
                .WithColumn("CreateTime").AsDateTime().NotNullable()
                .WithColumn("PublishTime").AsDateTime().Nullable()
                .WithColumn("Project").AsInt64().NotNullable().ForeignKey("FK_Snapshot(Project)_Project(Id)", "Project", "Id")
                .WithColumn("CreatedByUser").AsInt32().NotNullable().ForeignKey("FK_Snapshot(CreatedByUser)_User(Id)", "User", "Id")
                .WithColumn("Comment").AsString(2000).Nullable()
                .WithColumn("DefaultBookType").AsInt32().NotNullable().ForeignKey("FK_Snapshot(DefaultBookType)_BookType(Id)", "BookType", "Id")
                .WithColumn("BookVersion").AsInt64().Nullable().ForeignKey("FK_Snapshot(BookVersion)_BookVersionResource(ResourceVersionId)", "BookVersionResource", "ResourceVersionId");

            Create.Column("LatestPublishedSnapshot").OnTable("Project").AsInt64().Nullable().ForeignKey("FK_Project(LatestPublishedSnapshot)_Snapshot(Id)", "Snapshot", "Id");

            Create.Table("HistoryLog")
                .WithColumn("Id").AsInt64().NotNullable().PrimaryKey("PK_HistoryLog(Id)").Identity()
                .WithColumn("Project").AsInt64().Nullable().ForeignKey("FK_HistoryLog(Project)_Project(Id)", "Project", "Id")
                .WithColumn("User").AsInt32().Nullable().ForeignKey("FK_HistoryLog(User)_User(Id)", "User", "Id")
                .WithColumn("CreateTime").AsDateTime().NotNullable()
                .WithColumn("LogType").AsString(255).NotNullable()
                .WithColumn("Text").AsString(2000).NotNullable()
                .WithColumn("AdditionalDescription").AsString(2000).Nullable()
                .WithColumn("ExternalId").AsString(255).Nullable()
                //.WithColumn("DiscussionPost").AsInt64().Nullable().ForeignKey("FK_HistoryLog(DiscussionPost)_DiscussionPost(Id)", "DiscussionPost", "Id")
                .WithColumn("Snapshot").AsInt64().Nullable().ForeignKey("FK_HistoryLog(LatestPublishedSnapshot)_Snapshot(Id)", "Snapshot", "Id")
                .WithColumn("ResourceVersion").AsInt64().Nullable().ForeignKey("FK_HistoryLog(ResourceVersion)_ResourceVersion(Id)", "ResourceVersion", "Id");

            Create.Table("TextComment")
                .WithColumn("Id").AsInt64().NotNullable().PrimaryKey("PK_TextComment(Id)").Identity()
                .WithColumn("TextReferenceId").AsString(100).NotNullable()
                .WithColumn("Text").AsString(2000).NotNullable()
                .WithColumn("CreateTime").AsDateTime().NotNullable()
                .WithColumn("CreatedByUser").AsInt32().NotNullable().ForeignKey("FK_TextComment(CreatedByUser)_User(Id)", "User", "Id")
                .WithColumn("ParentComment").AsInt64().NotNullable().ForeignKey("FK_TextComment(ParentComment)_TextComment(Id)", "TextComment", "Id")
                .WithColumn("ResourceText").AsInt64().NotNullable().ForeignKey("FK_TextComment(ResourceText)_Resource(Id)", "Resource", "Id")
                .WithColumn("EditCount").AsInt32().Nullable()
                .WithColumn("LastEditTime").AsDateTime().Nullable();

            // Create M:N tables

            Create.Table("Project_OriginalAuthor")
                .WithColumn("Project").AsInt64().NotNullable().ForeignKey("FK_Project_OriginalAuthor(Project)_Project(Id)", "Project", "Id")
                .WithColumn("Author").AsInt32().NotNullable()
                .ForeignKey("FK_Project_OriginalAuthor(Author)_OriginalAuthor(Id)", "OriginalAuthor", "Id")
                .WithColumn("Sequence").AsInt32().NotNullable();
            Create.PrimaryKey("PK_Project_OriginalAuthor(Project)_Project_OriginalAuthor(Author)").OnTable("Project_OriginalAuthor")
                .Columns("Project", "Author");

            Create.Table("Project_ResponsiblePerson")
                .WithColumn("Project").AsInt64().NotNullable()
                .ForeignKey("FK_Project_ResponsiblePerson(Project)_Project(Id)", "Project", "Id")
                .WithColumn("Responsible").AsInt32().NotNullable()
                .ForeignKey("FK_Project_ResponsiblePerson(Responsible)_ResponsiblePerson(Id)", "ResponsiblePerson", "Id")
                .WithColumn("ResponsibleType").AsInt32().NotNullable()
                .ForeignKey("FK_Project_ResponsiblePerson(ResponsibleType)_ResponsibleType(Id)", "ResponsibleType", "Id");
            Create.PrimaryKey(
                    "PK_Project_ResponsiblePerson(Project)_Project_ResponsiblePerson(Responsible)_Project_ResponsiblePerson(ResponsibleType)")
                .OnTable("Project_ResponsiblePerson")
                .Columns("Project", "Responsible", "ResponsibleType");

            Create.Table("Project_Category")
                .WithColumn("Project").AsInt64().NotNullable().ForeignKey("FK_Project_Category(Project)_Project(Id)", "Project", "Id")
                .WithColumn("Category").AsInt32().NotNullable()
                .ForeignKey("FK_Project_Category(Category)_Category(Id)", "Category", "Id");
            Create.PrimaryKey("PK_Project_Category(Project)_Project_Category(Category)").OnTable("Project_Category")
                .Columns("Project", "Category");

            Create.Table("Project_LiteraryKind")
                .WithColumn("Project").AsInt64().NotNullable().ForeignKey("FK_Project_LiteraryKind(Project)_Project(Id)", "Project", "Id")
                .WithColumn("LiteraryKind").AsInt32().NotNullable()
                .ForeignKey("FK_Project_LiteraryKind(LiteraryKind)_LiteraryKind(Id)", "LiteraryKind", "Id");
            Create.PrimaryKey("PK_Project_LiteraryKind(Project)_Project_LiteraryKind(LiteraryKind)").OnTable("Project_LiteraryKind")
                .Columns("Project", "LiteraryKind");

            Create.Table("Project_LiteraryGenre")
                .WithColumn("Project").AsInt64().NotNullable().ForeignKey("FK_Project_LiteraryGenre(Project)_Project(Id)", "Project", "Id")
                .WithColumn("LiteraryGenre").AsInt32().NotNullable()
                .ForeignKey("FK_Project_LiteraryGenre(LiteraryGenre)_LiteraryGenre(Id)", "LiteraryGenre", "Id");
            Create.PrimaryKey("PK_Project_LiteraryGenre(Project)_Project_LiteraryGenre(LiteraryGenre)").OnTable("Project_LiteraryGenre")
                .Columns("Project", "LiteraryGenre");

            Create.Table("Project_LiteraryOriginal")
                .WithColumn("Project").AsInt64().NotNullable()
                .ForeignKey("FK_Project_LiteraryOriginal(Project)_Project(Id)", "Project", "Id")
                .WithColumn("LiteraryOriginal").AsInt32().NotNullable()
                .ForeignKey("FK_Project_LiteraryOriginal(LiteraryOriginal)_LiteraryOriginal(Id)", "LiteraryOriginal", "Id");
            Create.PrimaryKey("PK_Project_LiteraryOriginal(Project)_Project_LiteraryOriginal(LiteraryOriginal)")
                .OnTable("Project_LiteraryOriginal")
                .Columns("Project", "LiteraryOriginal");

            Create.Table("Project_Keyword")
                .WithColumn("Project").AsInt64().NotNullable()
                .ForeignKey("FK_Project_Keyword(Project)_Project(Id)", "Project", "Id")
                .WithColumn("Keyword").AsInt32().NotNullable()
                .ForeignKey("FK_Project_Keyword(Keyword)_Keyword(Id)", "Keyword", "Id");
            Create.PrimaryKey("PK_Project_Keyword(Project)_Project_Keyword(Keyword)").OnTable("Project_Keyword")
                .Columns("Project", "Keyword");

            Create.Table("PageResource_Term")
                .WithColumn("PageResource").AsInt64().NotNullable()
                .ForeignKey("FK_PageResource_Term(PageResource)_PageResource(Id)","PageResource", "ResourceVersionId")
                .WithColumn("Term").AsInt32().NotNullable()
                .ForeignKey("FK_PageResource_Term(Term)_Term(Id)", "Term", "Id");
            Create.PrimaryKey("PK_PageResource_Term(PageResource)_PageResource_Term(Term)").OnTable("PageResource_Term")
                .Columns("PageResource", "Term");

            Create.Table("Snapshot_ResourceVersion")
                .WithColumn("Snapshot").AsInt64().NotNullable()
                .ForeignKey("FK_Snapshot_ResourceVersion(Snapshot)_Snapshot(Id)", "Snapshot", "Id")
                .WithColumn("ResourceVersion").AsInt64().NotNullable()
                .ForeignKey("FK_Snapshot_ResourceVersion(ResourceVersion)_ResourceVersion(Id)", "ResourceVersion", "Id");
            Create.PrimaryKey("PK_Snapshot_ResourceVersion(Snapshot)_Snapshot_ResourceVersion(ResourceVersion)")
                .OnTable("Snapshot_ResourceVersion")
                .Columns("Snapshot", "ResourceVersion");

            Create.Table("Snapshot_BookType")
                .WithColumn("Snapshot").AsInt64().NotNullable()
                .ForeignKey("FK_Snapshot_BookType(Snapshot)_Snapshot(Id)", "Snapshot", "Id")
                .WithColumn("BookType").AsInt32().NotNullable()
                .ForeignKey("FK_Snapshot_BookType(BookType)_BookType(Id)", "BookType", "Id");
            Create.PrimaryKey("PK_Snapshot_BookType(Snapshot)_Snapshot_BookType(BookType)").OnTable("Snapshot_BookType")
                .Columns("Snapshot", "BookType");
        }
    }
}