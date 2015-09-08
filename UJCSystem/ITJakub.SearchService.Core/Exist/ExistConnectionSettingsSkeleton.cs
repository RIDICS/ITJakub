namespace ITJakub.SearchService.Core.Exist
{
    public class ExistConnectionSettingsSkeleton
    {
        public ExistConnectionSettingsSkeleton(string baseUri, string viewsCollection, string resourcesCollection,
            string dbUser, string dbPassword, string xQueriesRelativeUri, string transformationRelativeUri,
            int dbMaxResults, string booksRelativeUri, string bibliographyRelativeUri)
        {
            BaseUri = baseUri;
            ViewsCollection = viewsCollection;
            ResourcesCollection = resourcesCollection;
            DbUser = dbUser;
            DbPassword = dbPassword;
            XQueriesRelativeUri = xQueriesRelativeUri;
            TransformationRelativeUri = transformationRelativeUri;
            DbMaxResults = dbMaxResults;
            BooksRelativeUri = booksRelativeUri;
            BibliographyRelativeUri = bibliographyRelativeUri;
        }

        public string BaseUri { get; private set; }

        public string ViewsCollection { get; private set; }

        public string ResourcesCollection { get; private set; }

        public string XQueriesRelativeUri { get; private set; }

        public string TransformationRelativeUri { get; private set; }

        public int DbMaxResults { get; set; }

        public string BooksRelativeUri { get; set; }

        public string BibliographyRelativeUri { get; set; }

        public string DbUser { get; private set; }

        public string DbPassword { get; private set; }
    }
}