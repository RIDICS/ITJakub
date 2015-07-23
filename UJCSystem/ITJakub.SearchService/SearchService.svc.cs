using System.Collections.Generic;
using System.Reflection;
using System.ServiceModel;
using ITJakub.Shared.Contracts;
using ITJakub.Shared.Contracts.Searching.Criteria;
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

        public string GetBookPageByPosition(string bookId, string versionId, int pagePosition, string transformationName, OutputFormatEnumContract outputFormat, ResourceLevelEnumContract transformationLevel)
        {
            return m_searchServiceManager.GetBookPageByPosition(bookId, versionId, pagePosition, transformationName, outputFormat, transformationLevel);
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

        public string GetDictionaryEntryByXmlId(string bookId, string versionId, string xmlEntryId, string transformationName,
            OutputFormatEnumContract outputFormat, ResourceLevelEnumContract transformationLevel)
        {
            return m_searchServiceManager.GetDictionaryEntryByXmlId(bookId, versionId, xmlEntryId, transformationName, outputFormat, transformationLevel);
        }

        public string GetDictionaryEntryFromSearch(List<SearchCriteriaContract> searchCriterias, string bookId, string versionId, string xmlEntryId,
            string transformationName, OutputFormatEnumContract outputFormat, ResourceLevelEnumContract transformationLevel)
        {
            return m_searchServiceManager.GetDictionaryEntryFromSearch(searchCriterias, bookId, versionId, xmlEntryId, transformationName, outputFormat, transformationLevel);
        }

        public void ListSearchEditionsResults(List<SearchCriteriaContract> searchCriterias)
        {
            m_searchServiceManager.ListSearchEditionsResults(searchCriterias);
        }

        public string ListSearchDictionariesResults(List<SearchCriteriaContract> searchCriterias)
        {
            return m_searchServiceManager.ListSearchDictionariesResults(searchCriterias);
        }

        public int ListSearchDictionariesResultsCount(List<SearchCriteriaContract> searchCriterias)
        {
            return m_searchServiceManager.ListSearchDictionariesResultsCount(searchCriterias);
        }

        public int GetSearchCriteriaResultsCount(List<SearchCriteriaContract> nonMetadataCriterias)
        {
            return m_searchServiceManager.GetSearchCriteriaResultsCount(nonMetadataCriterias);
        }

        public string GetBookPageByName(string bookId, string versionId, string pageName, string transformationName,
            OutputFormatEnumContract outputFormat, ResourceLevelEnumContract transformationLevel)
        {
            if (m_log.IsDebugEnabled)
                m_log.DebugFormat("SearchService request recieved...");
            return m_searchServiceManager.GetBookPageByName(bookId, versionId, pageName, transformationName,
                outputFormat, transformationLevel);
        }

        public string GetBookPageByXmlId(string bookId, string versionId, string pageXmlId, string transformationName,
            OutputFormatEnumContract outputFormat, ResourceLevelEnumContract transformationLevel)
        {
            if (m_log.IsDebugEnabled)
                m_log.DebugFormat("SearchService request recieved...");
            return m_searchServiceManager.GetBookPageByXmlId(bookId, versionId, pageXmlId, transformationName,
                outputFormat, transformationLevel);
        }

        public string GetBookPagesByName(string bookId, string versionId, string startPageName, string endPageName,
            string transformationName, OutputFormatEnumContract outputFormat,
            ResourceLevelEnumContract transformationLevel)
        {
            return m_searchServiceManager.GetBookPagesByName(bookId, versionId, startPageName, endPageName,
                transformationName, outputFormat, transformationLevel);
        }

    }

    [ServiceContract]
    public interface ISearchServiceLocal : ISearchService
    {
    }
}