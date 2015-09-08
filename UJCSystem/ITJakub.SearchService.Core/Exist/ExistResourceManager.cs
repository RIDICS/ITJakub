using System;
using ITJakub.Shared.Contracts;

namespace ITJakub.SearchService.Core.Exist
{
    public class ExistResourceManager : IExistResourceManager
    {
        private readonly ExistConnectionSettingsSkeleton m_existSettings;

        public ExistResourceManager(ExistConnectionSettingsSkeleton existSettings)
        {
            m_existSettings = existSettings;
        }

        public string GetResourceUriTemplate(ResourceLevelEnumContract type)
        {
            switch (type)
            {
                case ResourceLevelEnumContract.Book:
                    return GetBookResourceUriTemplate();
                case ResourceLevelEnumContract.Shared:
                    return GetSharedResourceUriTemplate();
                case ResourceLevelEnumContract.Bibliography:
                    return GetBibliographyResourceUriTemplate();
                default:
                    return GetVersionResourceUriTemplate();
            }
        }
  

        public string GetTransformationUri(string transformationName, OutputFormatEnumContract outputFormat, ResourceLevelEnumContract transformationLevel,
            string bookGuid, string bookVersion)
        {
            switch (transformationLevel)
            {
                case ResourceLevelEnumContract.Book:
                    return string.Format("{0}{1}/{2}", m_existSettings.BooksRelativeUri, Uri.EscapeUriString(bookGuid), transformationName);
                case ResourceLevelEnumContract.Version:
                    return string.Format("{0}{1}/{2}/{3}", m_existSettings.BooksRelativeUri, Uri.EscapeUriString(bookGuid), bookVersion,
                        transformationName);
                case ResourceLevelEnumContract.Shared:
                    return string.Format("{0}{1}", m_existSettings.TransformationRelativeUri, transformationName);                
                default:
                    return "";
            }
        }

        public string GetQueryUriWithParams(string xqueryName, string queryStringParams)
        {
            return string.Format("{0}?{1}", GetQueryUri(xqueryName), queryStringParams);
        }

        public string GetQueryUri(string xqueryName)
        {
            return string.Format("{0}{1}{2}", m_existSettings.BaseUri, m_existSettings.XQueriesRelativeUri,
                xqueryName);
        }

        private string GetVersionResourceUriTemplate()
        {
            return string.Format("{0}{1}/{{0}}/{{1}}/{{2}}", m_existSettings.BaseUri,m_existSettings.BooksRelativeUri);
        }

        private string GetBibliographyResourceUriTemplate()
        {
            return string.Format("{0}{1}/{{0}}/{{1}}/{{2}}", m_existSettings.BaseUri,m_existSettings.BibliographyRelativeUri);
        }

        private string GetBookResourceUriTemplate()
        {
            return string.Format("{0}{1}/{{0}}/{{1}}", m_existSettings.BaseUri,m_existSettings.BooksRelativeUri);
        }

        private string GetSharedResourceUriTemplate()
        {
            return string.Format("{0}{1}/{{0}}", m_existSettings.BaseUri,m_existSettings.BooksRelativeUri);
        }
    }
}