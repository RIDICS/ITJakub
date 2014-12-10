namespace ITJakub.SearchService.Core.Exist
{
    public class ExistResourceManager: IExistResourceManager
    {
        private readonly ExistConnectionSettingsSkeleton m_existSettings;

        public ExistResourceManager(ExistConnectionSettingsSkeleton existSettings)
        {
            m_existSettings = existSettings;
        }

        public string GetResourceUriTemplate()
        {
            return string.Format("{0}{1}/{{0}}/{{1}}/{{2}}", m_existSettings.BaseUri,
                m_existSettings.ResourceRelativeUri);
        }

        public string GetQueryUriTemplate(string xqueryName, string queryStringParams)
        {
            return string.Format("{0}{1}{2}{3}", m_existSettings.BaseUri, m_existSettings.XQueriesRelativeUri,
                xqueryName, queryStringParams);
        }
    }
}