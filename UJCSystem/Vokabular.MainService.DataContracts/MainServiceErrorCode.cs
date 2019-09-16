namespace Vokabular.MainService.DataContracts
{
    public class MainServiceErrorCode
    {
        public const string EntityNotFound = "entity-not-found";
        public const string HeadwordNotFound = "headword-not-found";
        public const string CategoryHasSubCategories = "category-has-sub-categories";
        public const string RepositoryContainsHistory = "repository-contains-history";
        public const string CommentsToSecondLevel = "comments-to-second-level";
        public const string EditionNoteConflict = "edition-note-conflict";
        public const string ResourceVersionIdNull = "resource-version-id-null";
        public const string CannotMoveCategoryToSubcategory = "cannot-move-category-to-subcategory";
        public const string UnallowedAuthorizationCriteria = "unallowed-authorization-criteria";
        public const string UnregisteredCardFileAccessForbidden = "unregistered-user-cardfile-access-forbidden";
        public const string UnregisteredUserBookAccessForbidden = "unregistered-user-book-access-forbidden";
        public const string UnregisteredUserResourceAccessForbidden = "unregistered-user-resource-access-forbidden";
        public const string UserCardFileAccessForbidden = "user-cardfile-access-forbidden";
        public const string UserBookAccessForbidden = "user-book-access-forbidden";
        public const string UserResourceAccessForbidden = "user-resource-access-forbidden";
        public const string ProjectIdOrResourceId = "project-id-or-resource-id";
        public const string CreateAnonymousFeedback = "create-anonymous-feedback";
        public const string UserHasMissingExternalId = "user-has-missing-external-id";
        public const string CannotLocateUser = "cannot-locate-user";
        public const string NoSupportedSearch = "no-supported-search";
        public const string NullBookTypeNotSupported = "null-book-type-not-supported";
        public const string ImportFailed = "import-failed";
        public const string ImportFailedWithError = "import-failed-with-error";
        public const string ImportSucceedForumFailed = "import-succeed-forum-failed";
        public const string ProjectNotExist = "project-not-exist";
        public const string ForumAlreadyCreated = "forum-already-created";
        public const string CannotRemoveDefaultFavoriteLabel = "cannot-remove-default-favorite-label";
        public const string CannotModifyDefaultFavoriteLabel = "cannot-modify-default-favorite-label";
        public const string FavoriteLabelNotFound = "favorite-label-not-found";
        public const string UserDoesNotOwnLabel = "user-does-not-own-label";
        public const string PageNotFound = "page-not-found";
        public const string ItemNotFound = "item-not-found";
        public const string NotSupportedCriteriaKey = "not-supported-criteria-key";
        public const string MissingFulltextCriteria = "missing-fulltext-criteria";
        public const string DeleteResourceProjectRelation = "delete-resource-project-relation";
        public const string DeleteResourcePageRelation = "delete-resource-page-relation";
        public const string AddUserToDefaultRole = "add-user-to-default-role";
        public const string RenameDefaultRole = "rename-default-role";
        public const string DeleteDefaultRole = "delete-default-role";
        public const string ResourceModified = "resource-modified";
        public const string UserHasNotPermissionToManipulate = "user-has-not-permission-to-manipulate";
        public const string RepositoryImportManagerNotFound = "repository-import-manager-not-found";
        public const string RepositoryImportCancelled = "repository-import-cancelled";
        public const string RepositoryImportFailed = "repository-import-failed";
        public const string OaiPmhCommunicationError = "oai-pmh-communication-error";
        public const string OaiPmhCommunicationFailed = "oai-pmh-communication-failed";
        public const string ProjectParserNotFound = "project-parser-not-found";
        public const string UnsupportedFormatType = "unsupported-format-type";
    }
}
