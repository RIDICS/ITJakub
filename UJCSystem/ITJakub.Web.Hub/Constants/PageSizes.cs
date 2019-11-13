namespace ITJakub.Web.Hub.Constants
{
    public static class PageSizes
    {
        public const int BibliographyModule = 20;
        public const int CorpusSearch = 30;
        public const int CorpusSearchBooks = 10;
        public const int ProjectList = 20;
        public const int SnapshotList = 10;

        public const int MinCorpusSearch = 1;
        public const int MaxCorpusSearch = 100;
        public const int DefaultContextLength = 100;
        public const int MinContextLength = 40; //search backend may crash if context is too short
        public const int MaxContextLength = 500;
    }
}
