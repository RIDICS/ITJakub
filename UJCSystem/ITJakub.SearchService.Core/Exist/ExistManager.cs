using System;
using System.Collections.Generic;
using System.IO;
using ITJakub.SearchService.Core.Search;
using ITJakub.SearchService.Core.Search.DataContract;
using ITJakub.SearchService.DataContracts.Contracts.SearchResults;
using ITJakub.SearchService.DataContracts.Types;
using Vokabular.Shared.DataContracts.Search.Criteria;
using Vokabular.Shared.DataContracts.Search.Old;
using Vokabular.Shared.DataContracts.Types;

namespace ITJakub.SearchService.Core.Exist
{
    public class ExistManager
    {
        private readonly ExistCommunicationManager m_communicationManager;
        private readonly IExistResourceManager m_existResourceManager;
        private readonly FulltextSearchCriteriaDirector m_searchCriteriaDirector;

        public ExistManager(ExistCommunicationManager existCommunicationManager, IExistResourceManager existResourceManager, FulltextSearchCriteriaDirector searchCriteriaDirector)
        {
            m_communicationManager = existCommunicationManager;
            m_existResourceManager = existResourceManager;
            m_searchCriteriaDirector = searchCriteriaDirector;
        }

        public void UploadBookFile(string bookId, string fileName, Stream dataStream)
        {
            m_communicationManager.UploadBookFile(bookId, fileName, dataStream);
        }

        public void UploadVersionFile(string bookId, string versionId, string fileName, Stream fileStream)
        {
            m_communicationManager.UploadVersionFile(bookId, versionId, fileName, fileStream);
        }

        public void UploadSharedFile(string fileName, Stream filStream)
        {
            m_communicationManager.UploadSharedFile(fileName, filStream);
        }


        public void UploadBibliographyFile(string bookId, string versionId, string fileName, Stream dataStream)
        {
            m_communicationManager.UploadBibliographyFile(bookId, versionId, fileName, dataStream);
        }

        public string GetPageByPositionFromStart(string bookId, string versionId, int pagePosition,
            string transformationName, OutputFormatEnumContract outputFormat,
            ResourceLevelEnumContract transformationLevel)
        {
            var xslPath = m_existResourceManager.GetTransformationUri(transformationName, outputFormat,
                transformationLevel, bookId, versionId);
            return m_communicationManager.GetPageByPositionFromStart(bookId, versionId, pagePosition,
                Enum.GetName(typeof (OutputFormatEnumContract), outputFormat), xslPath);
        }

        public string GetPageByName(string bookId, string versionId, string pageName, string transformationName,
            OutputFormatEnumContract outputFormat, ResourceLevelEnumContract transformationLevel)
        {
            var xslPath = m_existResourceManager.GetTransformationUri(transformationName, outputFormat,
                transformationLevel, bookId, versionId);
            return m_communicationManager.GetPageByName(bookId, versionId, pageName,
                Enum.GetName(typeof (OutputFormatEnumContract), outputFormat), xslPath);
        }

        public string GetPagesByName(string bookId, string versionId, string start, string end,
            string transformationName, OutputFormatEnumContract outputFormat,
            ResourceLevelEnumContract transformationLevel)
        {
            var xslPath = m_existResourceManager.GetTransformationUri(transformationName, outputFormat,
                transformationLevel, bookId, versionId);
            return m_communicationManager.GetPagesByName(bookId, versionId, start, end,
                Enum.GetName(typeof (OutputFormatEnumContract), outputFormat), xslPath);
        }

        public string GetPageByXmlId(string bookId, string versionId, string pageXmlId, string transformationName,
            OutputFormatEnumContract outputFormat, ResourceLevelEnumContract transformationLevel)
        {
            var xslPath = m_existResourceManager.GetTransformationUri(transformationName, outputFormat,
                transformationLevel, bookId, versionId);
            return m_communicationManager.GetPageByXmlId(bookId, versionId, pageXmlId,
                Enum.GetName(typeof (OutputFormatEnumContract), outputFormat), xslPath);
        }

        public string GetDictionaryEntryByXmlId(string bookId, string versionId, string xmlEntryId,
            string transformationName, OutputFormatEnumContract outputFormat,
            ResourceLevelEnumContract transformationLevel)
        {
            var xslPath = m_existResourceManager.GetTransformationUri(transformationName, outputFormat,
               transformationLevel, bookId, versionId);
            return m_communicationManager.GetDictionaryEntryByXmlId(bookId, versionId, xmlEntryId,
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
            
            return m_communicationManager.GetDictionaryEntryFromSearch(resultSearchConjunctions.ToXml(), bookId, versionId, xmlEntryId,
                Enum.GetName(typeof(OutputFormatEnumContract), outputFormat), xslPath);
        }

        private ResultSearchCriteriaContract GetFilteredResultSearchCriterias(IList<SearchCriteriaContract> searchCriterias)
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

            return new ResultSearchCriteriaContract
            {
                ConjunctionSearchCriterias = filteredCriterias,
                ResultSpecifications = resultCriteriaContract,
                ResultBooks = resultRestrictionCriteriaContract != null
                    ? resultRestrictionCriteriaContract.ResultBooks
                    : null,
            };
        }

        public SearchResultContractList ListSearchEditionsResults(List<SearchCriteriaContract> searchCriterias)
        {
            var filteredCriterias = GetFilteredResultSearchCriterias(searchCriterias);
            if (filteredCriterias.ResultBooks == null)
                return null;

            //AdjustStartIndexes(filteredCriterias.ResultSpecifications);

            var serializedCriterias = filteredCriterias.ToXml();
            var result = m_communicationManager.ListSearchEditionsResults(serializedCriterias);
            return SearchResultContractList.FromXml(result);
        }

        private void AdjustStartIndexes(ResultCriteriaContract resultCriteriaContract)
        {
            if (resultCriteriaContract != null)
            {
                if (resultCriteriaContract.Start.HasValue)
                {
                    resultCriteriaContract.Start++;
                }

                if (resultCriteriaContract.HitSettingsContract != null && resultCriteriaContract.HitSettingsContract.Start.HasValue)
                {
                    resultCriteriaContract.HitSettingsContract.Start++;
                }
            }
        }

        public HeadwordListContract ListSearchDictionariesResults(List<SearchCriteriaContract> searchCriterias)
        {
            var resultSearchCriteria = GetFilteredResultSearchCriterias(searchCriterias);
            if (resultSearchCriteria.ResultBooks == null)
                return null;

            AdjustStartIndexes(resultSearchCriteria.ResultSpecifications);

            var stringResult = m_communicationManager.ListSearchDictionariesResults(resultSearchCriteria.ToXml());
            return HeadwordListContract.FromXml(stringResult);
        }

        public int ListSearchDictionariesResultsCount(List<SearchCriteriaContract> searchCriterias)
        {
            var resultSearchCriteria = GetFilteredResultSearchCriterias(searchCriterias);
            if (resultSearchCriteria.ResultBooks == null)
                return 0;

            var result = m_communicationManager.ListSearchDictionariesResultsCount(resultSearchCriteria.ToXml());
            return result;
        }

        public int GetSearchCriteriaResultsCount(List<SearchCriteriaContract> searchCriterias)
        {
            var resultSearchCriteria = GetFilteredResultSearchCriterias(searchCriterias);
            if (resultSearchCriteria.ResultBooks == null)
                return 0;

            var serializedCriteria = resultSearchCriteria.ToXml();
            return m_communicationManager.GetSearchCriteriaResultsCount(serializedCriteria);
        }

        public PageListContract GetSearchEditionsPageList(List<SearchCriteriaContract> searchCriterias)
        {
            var filteredCriterias = GetFilteredResultSearchCriterias(searchCriterias);
            if (filteredCriterias.ResultBooks == null)
                return null;

            return PageListContract.FromXml(m_communicationManager.GetSearchEditionsPageList(filteredCriterias.ToXml()));
        }

        public string GetEditionPageFromSearch(IList<SearchCriteriaContract> searchCriterias, string bookId, string versionId, string pageXmlId, string transformationName, OutputFormatEnumContract outputFormat, ResourceLevelEnumContract transformationLevel)
        {
            var resultSearchConjunctions = new ResultConjunctionsCriteriaContract
            {
                ConjunctionSearchCriterias = GetFilteredResultSearchCriterias(searchCriterias).ConjunctionSearchCriterias
            };

            var xslPath = m_existResourceManager.GetTransformationUri(transformationName, outputFormat,
                transformationLevel, bookId, versionId);
            return m_communicationManager.GetEditionPageFromSearch(resultSearchConjunctions.ToXml(), bookId, versionId, pageXmlId,
                Enum.GetName(typeof(OutputFormatEnumContract), outputFormat), xslPath);
        }


        public CorpusSearchResultContractList GetCorpusSearchResults(List<SearchCriteriaContract> searchCriterias)
        {
            var resultSearchCriteria = GetFilteredResultSearchCriterias(searchCriterias);
            if (resultSearchCriteria.ResultBooks == null)
                return null;

            AdjustStartIndexes(resultSearchCriteria.ResultSpecifications);

            var stringResult = m_communicationManager.GetSearchCorpus(resultSearchCriteria.ToXml());
            return CorpusSearchResultContractList.FromXml(stringResult);
        }

        public int GetCorpusSearchResultsCount(List<SearchCriteriaContract> searchCriterias)
        {
            var resultSearchCriteria = GetFilteredResultSearchCriterias(searchCriterias);
            if (resultSearchCriteria.ResultBooks == null)
                return 0;

            var result = m_communicationManager.GetSearchCorpusCount(resultSearchCriteria.ToXml());
            return result;
        }


        public string GetBookEditionNote(string bookId, string versionId, string transformationName,
            OutputFormatEnumContract outputFormat, ResourceLevelEnumContract transformationLevel)
        {
            var xslPath = m_existResourceManager.GetTransformationUri(transformationName, outputFormat,
                transformationLevel, bookId, versionId);
            return m_communicationManager.GetBookEditionNote(bookId, versionId, 
                Enum.GetName(typeof(OutputFormatEnumContract), outputFormat), xslPath);
        }
    }
}