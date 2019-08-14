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
    }
}
