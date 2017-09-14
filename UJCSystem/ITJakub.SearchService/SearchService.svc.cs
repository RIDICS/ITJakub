using System.Collections.Generic;
using System.Reflection;
using System.ServiceModel;
using ITJakub.ITJakubService.DataContracts;
using ITJakub.Shared.Contracts;
using ITJakub.Shared.Contracts.Searching.Results;
using log4net;
using Vokabular.Shared.DataContracts.Search.Criteria;

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

        public void UploadBibliographyFile(VersionResourceUploadContract bibliographyResource)
        {
            m_searchServiceManager.UploadBibliographyFile(bibliographyResource);
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

        public SearchResultContractList ListSearchEditionsResults(List<SearchCriteriaContract> searchCriterias)
        {
            return m_searchServiceManager.ListSearchEditionsResults(searchCriterias);
        }

        public HeadwordListContract ListSearchDictionariesResults(List<SearchCriteriaContract> searchCriterias)
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

        public PageListContract GetSearchEditionsPageList(List<SearchCriteriaContract> searchCriterias)
        {
            return m_searchServiceManager.GetSearchEditionsPageList(searchCriterias);
        }

        public int GetCorpusSearchResultsCount(List<SearchCriteriaContract> searchCriterias)
        {
            return m_searchServiceManager.GetCorpusSearchResultsCount(searchCriterias);
        }

        public CorpusSearchResultContractList GetCorpusSearchResults(List<SearchCriteriaContract> searchCriterias)
        {
            return m_searchServiceManager.GetCorpusSearchResults(searchCriterias);
        }

        public string GetEditionPageFromSearch(IList<SearchCriteriaContract> searchCriterias, string bookId, string versionId, string pageXmlId,
            string transformationName, OutputFormatEnumContract outputFormat, ResourceLevelEnumContract transformationLevel)
        {
            return m_searchServiceManager.GetEditionPageFromSearch(searchCriterias, bookId, versionId, pageXmlId, transformationName, outputFormat, transformationLevel);
        }

        public string GetBookEditionNote(string bookId, string versionId, string transformationName,
            OutputFormatEnumContract outputFormat, ResourceLevelEnumContract transformationLevel)
        {
            return m_searchServiceManager.GetBookEditionNote(bookId, versionId, transformationName, outputFormat, transformationLevel);
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