using System;
using System.Collections.Generic;
using System.IO;
using ITJakub.SearchService.Core.Search;
using ITJakub.SearchService.Core.Search.DataContract;
using ITJakub.Shared.Contracts;
using ITJakub.Shared.Contracts.Searching.Criteria;
using ITJakub.Shared.Contracts.Searching.Results;

namespace ITJakub.SearchService.Core.Exist
{
    public class ExistManager
    {
        private readonly ExistClient m_client;
        private readonly IExistResourceManager m_existResourceManager;
        private readonly FulltextSearchCriteriaDirector m_searchCriteriaDirector;

        public ExistManager(ExistClient existClient, IExistResourceManager existResourceManager, FulltextSearchCriteriaDirector searchCriteriaDirector)
        {
            m_client = existClient;
            m_existResourceManager = existResourceManager;
            m_searchCriteriaDirector = searchCriteriaDirector;
        }

        public void UploadBookFile(string bookId, string fileName, Stream dataStream)
        {
            m_client.UploadBookFile(bookId, fileName, dataStream);
        }

        public void UploadVersionFile(string bookId, string versionId, string fileName, Stream filStream)
        {
            m_client.UploadVersionFile(bookId, versionId, fileName, filStream);
        }

        public void UploadSharedFile(string fileName, Stream filStream)
        {
            m_client.UploadSharedFile(fileName, filStream);
        }

        public string GetPageByPositionFromStart(string bookId, string versionId, int pagePosition,
            string transformationName, OutputFormatEnumContract outputFormat,
            ResourceLevelEnumContract transformationLevel)
        {
            var xslPath = m_existResourceManager.GetTransformationUri(transformationName, outputFormat,
                transformationLevel, bookId, versionId);
            return m_client.GetPageByPositionFromStart(bookId, versionId, pagePosition,
                Enum.GetName(typeof (OutputFormatEnumContract), outputFormat), xslPath);
        }

        public string GetPageByName(string bookId, string versionId, string pageName, string transformationName,
            OutputFormatEnumContract outputFormat, ResourceLevelEnumContract transformationLevel)
        {
            var xslPath = m_existResourceManager.GetTransformationUri(transformationName, outputFormat,
                transformationLevel, bookId, versionId);
            return m_client.GetPageByName(bookId, versionId, pageName,
                Enum.GetName(typeof (OutputFormatEnumContract), outputFormat), xslPath);
        }

        public string GetPagesByName(string bookId, string versionId, string start, string end,
            string transformationName, OutputFormatEnumContract outputFormat,
            ResourceLevelEnumContract transformationLevel)
        {
            var xslPath = m_existResourceManager.GetTransformationUri(transformationName, outputFormat,
                transformationLevel, bookId, versionId);
            return m_client.GetPagesByName(bookId, versionId, start, end,
                Enum.GetName(typeof (OutputFormatEnumContract), outputFormat), xslPath);
        }

        public string GetPageByXmlId(string bookId, string versionId, string pageXmlId, string transformationName,
            OutputFormatEnumContract outputFormat, ResourceLevelEnumContract transformationLevel)
        {
            var xslPath = m_existResourceManager.GetTransformationUri(transformationName, outputFormat,
                transformationLevel, bookId, versionId);
            return m_client.GetPageByXmlId(bookId, versionId, pageXmlId,
                Enum.GetName(typeof (OutputFormatEnumContract), outputFormat), xslPath);
        }

        public string GetDictionaryEntryByXmlId(string bookId, string versionId, string xmlEntryId,
            string transformationName, OutputFormatEnumContract outputFormat,
            ResourceLevelEnumContract transformationLevel)
        {
            var xslPath = m_existResourceManager.GetTransformationUri(transformationName, outputFormat,
               transformationLevel, bookId, versionId);
            return m_client.GetDictionaryEntryByXmlId(bookId, versionId, xmlEntryId,
                Enum.GetName(typeof(OutputFormatEnumContract), outputFormat), xslPath);
        }

        public string GetDictionaryEntryFromSearch(List<SearchCriteriaContract> searchCriterias, string bookId, string versionId, string xmlEntryId, string transformationName, OutputFormatEnumContract outputFormat, ResourceLevelEnumContract transformationLevel)
        {
            var resultSearchConjunctions = new ResultConjunctionsCriteriaContract
            {
                ConjunctionSearchCriterias = GetFilteredResultSearchCriterias(searchCriterias).ConjunctionSearchCriterias
            };
            
            var xslPath = m_existResourceManager.GetTransformationUri(transformationName, outputFormat,
               transformationLevel, bookId, versionId);
            
            return m_client.GetDictionaryEntryFromSearch(resultSearchConjunctions.ToXml(), bookId, versionId, xmlEntryId,
                Enum.GetName(typeof(OutputFormatEnumContract), outputFormat), xslPath);
        }

        private ResultSearchCriteriaContract GetFilteredResultSearchCriterias(IList<SearchCriteriaContract> searchCriterias)
        {
            ResultRestrictionCriteriaContract resultRestrictionCriteriaContract = null;
            RegexCriteriaBuilder.ConvertWildcardToRegex(searchCriterias);
            var filteredCriterias = new List<SearchCriteriaContract>();
            foreach (var searchCriteriaContract in searchCriterias)
            {
                if (m_searchCriteriaDirector.IsCriteriaSupported(searchCriteriaContract))
                {
                    filteredCriterias.Add(RegexCriteriaBuilder.ConvertToRegexCriteria(searchCriteriaContract));
                }
                else if (searchCriteriaContract.Key == CriteriaKey.ResultRestriction)
                {
                    resultRestrictionCriteriaContract = (ResultRestrictionCriteriaContract)searchCriteriaContract;
                }
            }

            if (resultRestrictionCriteriaContract == null)
                return null;

            return new ResultSearchCriteriaContract
            {
                ConjunctionSearchCriterias = filteredCriterias,
                ResultBooks = resultRestrictionCriteriaContract.ResultBooks
            };
        }

        public SearchResultContractList ListSearchEditionsResults(List<SearchCriteriaContract> searchCriterias)
        {
            ResultRestrictionCriteriaContract resultRestrictionCriteriaContract = null;
            ResultCriteriaContract resultCriteriaContract = null;
            RegexCriteriaBuilder.ConvertWildcardToRegex(searchCriterias);
            var filteredCriterias = new List<SearchCriteriaContract>();
            foreach (var searchCriteriaContract in searchCriterias)
            {
                if (m_searchCriteriaDirector.IsCriteriaSupported(searchCriteriaContract))
                {
                    filteredCriterias.Add(RegexCriteriaBuilder.ConvertToRegexCriteria(searchCriteriaContract));
                }
                else if (searchCriteriaContract.Key == CriteriaKey.ResultRestriction)
                {
                    resultRestrictionCriteriaContract = (ResultRestrictionCriteriaContract) searchCriteriaContract;
                }
                else if (searchCriteriaContract.Key == CriteriaKey.Result)
                {
                    resultCriteriaContract = (ResultCriteriaContract) searchCriteriaContract;
                }
            }

            if (resultRestrictionCriteriaContract == null)
                return null;

            var searchCriteria = new ResultSearchCriteriaContract
            {
                ResultBooks = resultRestrictionCriteriaContract.ResultBooks,
                ResultSpecifications = resultCriteriaContract,
                ConjunctionSearchCriterias = filteredCriterias
            };

            return SearchResultContractList.FromXml(m_client.ListSearchEditionsResults(searchCriteria.ToXml()));
        }
        
        public string ListSearchDictionariesResults(List<SearchCriteriaContract> searchCriterias)
        {
            var resultSearchCriteria = GetFilteredResultSearchCriterias(searchCriterias);
            
            var stringResult = m_client.ListSearchDictionariesResults(resultSearchCriteria.ToXml());
            return stringResult;
        }

        public int ListSearchDictionariesResultsCount(List<SearchCriteriaContract> searchCriterias)
        {
            var resultSearchCriteria = GetFilteredResultSearchCriterias(searchCriterias);

            var result = m_client.ListSearchDictionariesResultsCount(resultSearchCriteria.ToXml());
            return result;
        }

        public int GetSearchCriteriaResultsCount(List<SearchCriteriaContract> searchCriterias)
        {
            var resultSearchCriteria = GetFilteredResultSearchCriterias(searchCriterias);
            return m_client.GetSearchCriteriaResultsCount(resultSearchCriteria.ToXml());
        }
    }
}