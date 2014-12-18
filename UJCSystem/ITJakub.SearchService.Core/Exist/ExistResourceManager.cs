using ITJakub.SearchService.Core.Exist.Attributes;

namespace ITJakub.SearchService.Core.Exist
{
    public class ExistResourceManager : IExistResourceManager
    {
        private readonly ExistConnectionSettingsSkeleton m_existSettings;

        public ExistResourceManager(ExistConnectionSettingsSkeleton existSettings)
        {
            m_existSettings = existSettings;
        }

        public string GetResourceUriTemplate(ResourceLevelType type)
        {
            switch (type)
            {
                case ResourceLevelType.Book:
                    return GetBookResourceUriTemplate();
                case ResourceLevelType.Shared:
                    return GetSharedResourceUriTemplate();
                default:
                    return GetVersionResourceUriTemplate();
            }
        }

        public string GetQueryUriTemplate(string xqueryName, string queryStringParams)
        {
            return string.Format("{0}{1}{2}{3}", m_existSettings.BaseUri, m_existSettings.XQueriesRelativeUri,
                xqueryName, queryStringParams);
        }

        private string GetVersionResourceUriTemplate()
        {
            return string.Format("{0}{1}/{{0}}/{{1}}/{{2}}", m_existSettings.BaseUri,
                m_existSettings.ResourceRelativeUri);
        }

        private string GetBookResourceUriTemplate()
        {
            return string.Format("{0}{1}/{{0}}/{{1}}", m_existSettings.BaseUri,
                m_existSettings.ResourceRelativeUri);
        }

        private string GetSharedResourceUriTemplate()
        {
            return string.Format("{0}{1}/{{0}}", m_existSettings.BaseUri,
                m_existSettings.ResourceRelativeUri);
        }
    }
}