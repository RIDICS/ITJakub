using System;
using System.Configuration;
using Vokabular.Shared.Options;

namespace ITJakub.SearchService.Core.Exist
{
    public class ExistConnectionSettingsSkeleton
    {
        public ExistConnectionSettingsSkeleton(string baseUri, string viewsCollection, string resourcesCollection,
            string xQueriesRelativeUri, string transformationRelativeUri,
            int dbMaxResults, string booksRelativeUri, string bibliographyRelativeUri)
        {
            BaseUri = baseUri;
            ViewsCollection = viewsCollection;
            ResourcesCollection = resourcesCollection;
            DbUser = ConfigurationManager.AppSettings[SettingKeys.ExistDbUser] ?? throw new ArgumentException("eXistDB username not found");
            DbPassword = ConfigurationManager.AppSettings[SettingKeys.ExistDbPassword] ?? throw new ArgumentException("eXistDB user password not found");
            XQueriesRelativeUri = xQueriesRelativeUri;
            TransformationRelativeUri = transformationRelativeUri;
            DbMaxResults = dbMaxResults;
            BooksRelativeUri = booksRelativeUri;
            BibliographyRelativeUri = bibliographyRelativeUri;
        }

        public string BaseUri { get; }

        public string ViewsCollection { get; }

        public string ResourcesCollection { get; }

        public string XQueriesRelativeUri { get; }

        public string TransformationRelativeUri { get; }

        public int DbMaxResults { get; }

        public string BooksRelativeUri { get; }

        public string BibliographyRelativeUri { get; }

        public string DbUser { get; }

        public string DbPassword { get; }
    }
}