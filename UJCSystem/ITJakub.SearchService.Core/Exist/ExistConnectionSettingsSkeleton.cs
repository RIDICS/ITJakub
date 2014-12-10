namespace ITJakub.SearchService.Core.Exist
{
    public class ExistConnectionSettingsSkeleton
    {
        public ExistConnectionSettingsSkeleton(string baseUri, string viewsCollection, string resourcesCollection,
            string dbUser, string dbPassword, string xQueriesRelativeUri, string transformationRelativeUri,
            int dbMaxResults, string resourceRelativeUri)
        {
            BaseUri = baseUri;
            ViewsCollection = viewsCollection;
            ResourcesCollection = resourcesCollection;
            DBUser = dbUser;
            DBPassword = dbPassword;
            XQueriesRelativeUri = xQueriesRelativeUri;
            TransformationRelativeUri = transformationRelativeUri;
            DbMaxResults = dbMaxResults;
            ResourceRelativeUri = resourceRelativeUri;
        }

        public string BaseUri { get; private set; }
        public string ViewsCollection { get; private set; }
        public string ResourcesCollection { get; private set; }
        public string XQueriesRelativeUri { get; private set; }
        public string TransformationRelativeUri { get; private set; }
        public int DbMaxResults { get; set; }
        public string ResourceRelativeUri { get; set; }
        public string DBUser { get; private set; }
        public string DBPassword { get; private set; }
    }
}