using System.Collections.Generic;
using System.Reflection;
using System.ServiceModel;
using ITJakub.Shared.Contracts;
using log4net;

namespace ITJakub.SearchService
{
    public class SearchService : ISearchServiceLocal
    {
        private readonly SearchServiceManager m_searchServiceManager;

        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public SearchService()
        {
            m_searchServiceManager = Container.Current.Resolve<SearchServiceManager>();
        }

        public string GetBookPageByPosition(string bookId, string versionId, int pagePosition, string transformationName, ResourceLevelEnumContract transformationLevel)
        {
            return m_searchServiceManager.GetBookPageByPosition(bookId, versionId, pagePosition, transformationName, transformationLevel);
        }

        public void UploadVersionFile(VersionResourceUploadContract versionResourceUploadContract)
        {
            m_searchServiceManager.UploadVersionFile(versionResourceUploadContract);
        }

        public void UploadBookFile(BookResourceUploadContract contract)
        {
            m_searchServiceManager.UploadBookFile(contract);
        }

        public void UploadSharedFile(ResourceUploadContract contract)
        {
            m_searchServiceManager.UploadSharedFile(contract);
        }

        public string GetBookPageByName(string bookId, string versionId, string pageName, string transformationName, ResourceLevelEnumContract transformationLevel)
        {
            if(m_log.IsDebugEnabled)
                m_log.DebugFormat("SearchService request recieved...");
            return m_searchServiceManager.GetBookPageByName(bookId, versionId, pageName, transformationName, transformationLevel);
        }

        public string GetBookPagesByName(string bookId, string versionId, string startPageName, string endPageName, string transformationName, ResourceLevelEnumContract transformationLevel)
        {
            return m_searchServiceManager.GetBookPagesByName(bookId, versionId, startPageName, endPageName, transformationName, transformationLevel);
        }

        public IList<BookPageContract> GetBookPageList(string bookId, string versionId)
        {
            return m_searchServiceManager.GetBookPageList(bookId, versionId);
        }
    }

    [ServiceContract]
    public interface ISearchServiceLocal : ISearchService
    {
    }
}