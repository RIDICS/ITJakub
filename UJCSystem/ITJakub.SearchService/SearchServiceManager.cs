using System;
using System.Collections.Generic;
using ITJakub.SearchService.Core.Exist;
using ITJakub.Shared.Contracts;
using ITJakub.Shared.Contracts.Searching;
using ITJakub.Shared.Contracts.Searching.Criteria;

namespace ITJakub.SearchService
{
    public class SearchServiceManager : ISearchService
    {
        private readonly ExistManager m_existManager;

        public SearchServiceManager(ExistManager existManager)
        {
            m_existManager = existManager;
        }

        public string GetBookPageByPosition(string bookId, string versionId, int pagePosition, string transformationName,
            OutputFormatEnumContract outputFormat, ResourceLevelEnumContract transformationLevel)
        {
            return m_existManager.GetPageByPositionFromStart(bookId, versionId, pagePosition, transformationName,
                outputFormat, transformationLevel);
        }

        public string GetBookPageByName(string bookId, string versionId, string pageName, string transformationName,
            OutputFormatEnumContract outputFormat, ResourceLevelEnumContract transformationLevel)
        {
            return m_existManager.GetPageByName(bookId, versionId, pageName, transformationName, outputFormat,
                transformationLevel);
        }

        public string GetBookPageByXmlId(string bookId, string versionId, string pageXmlId, string transformationName,
            OutputFormatEnumContract outputFormat, ResourceLevelEnumContract transformationLevel)
        {
            return m_existManager.GetPageByXmlId(bookId, versionId, pageXmlId, transformationName, outputFormat,
                transformationLevel);
        }

        public string GetBookPagesByName(string bookId, string versionId, string startPageName, string endPageName,
            string transformationName, OutputFormatEnumContract outputFormat,
            ResourceLevelEnumContract transformationLevel)
        {
            return m_existManager.GetPagesByName(bookId, versionId, startPageName, endPageName, transformationName,
                outputFormat, transformationLevel);
        }

        public void UploadVersionFile(VersionResourceUploadContract contract)
        {
            m_existManager.UploadVersionFile(contract.BookId, contract.BookVersionId, contract.FileName, contract.DataStream);
        }

        public void UploadBookFile(BookResourceUploadContract contract)
        {
            m_existManager.UploadBookFile(contract.BookId, contract.FileName, contract.DataStream);
        }

        public void UploadSharedFile(ResourceUploadContract contract)
        {
            m_existManager.UploadSharedFile(contract.FileName, contract.DataStream);
        }

        public string GetDictionaryEntryByXmlId(string bookId, string versionId, string xmlEntryId,
            string transformationName, OutputFormatEnumContract outputFormat,
            ResourceLevelEnumContract transformationLevel)
        {
            return m_existManager.GetDictionaryEntryByXmlId(bookId, versionId, xmlEntryId, transformationName, outputFormat,
                transformationLevel);
        }

        public void ListSearchEditionsResults(List<SearchCriteriaContract> searchCriterias)
        {
            m_existManager.ListSearchEditionsResults(searchCriterias);
        }
    }
}