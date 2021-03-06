﻿using System.Collections.Generic;
using System.ServiceModel;
using ITJakub.SearchService.DataContracts.Contracts;
using ITJakub.SearchService.DataContracts.Contracts.SearchResults;
using ITJakub.SearchService.DataContracts.Types;
using Vokabular.Shared.DataContracts.Search.Criteria;
using Vokabular.Shared.DataContracts.Search.Old;

namespace ITJakub.SearchService.DataContracts
{
    [ServiceContract]
    public interface ISearchService
    {

        [OperationContract]
        string GetBookPageByXmlId(string bookId, string versionId, string pageXmlId, string transformationName,
            OutputFormatEnumContract outputFormat, ResourceLevelEnumContract transformationLevel);

        [OperationContract]
        void UploadVersionFile(VersionResourceUploadContract versionResource);

        [OperationContract]
        void UploadBibliographyFile(VersionResourceUploadContract bibliographyResource);

        [OperationContract]
        void UploadBookFile(BookResourceUploadContract contract);

        [OperationContract]
        void UploadSharedFile(ResourceUploadContract contract);

        [OperationContract]
        string GetDictionaryEntryByXmlId(string bookId, string versionId, string xmlEntryId, string transformationName,
            OutputFormatEnumContract outputFormat, ResourceLevelEnumContract transformationLevel);

        [OperationContract]
        string GetDictionaryEntryFromSearch(List<SearchCriteriaContract> searchCriterias, string bookId, string versionId, string xmlEntryId, string transformationName,
            OutputFormatEnumContract outputFormat, ResourceLevelEnumContract transformationLevel);

        [OperationContract]
        SearchResultContractList ListSearchEditionsResults(List<SearchCriteriaContract> searchCriterias);

        [OperationContract]
        HeadwordListContract ListSearchDictionariesResults(List<SearchCriteriaContract> searchCriterias);

        [OperationContract]
        int ListSearchDictionariesResultsCount(List<SearchCriteriaContract> searchCriterias);

        [OperationContract]
        int GetSearchCriteriaResultsCount(List<SearchCriteriaContract> nonMetadataCriterias);

        [OperationContract]
        PageListContract GetSearchEditionsPageList(List<SearchCriteriaContract> searchCriterias);

        [OperationContract]
        int GetCorpusSearchResultsCount(List<SearchCriteriaContract> searchCriterias);

        [OperationContract]
        CorpusSearchResultContractList GetCorpusSearchResults(List<SearchCriteriaContract> searchCriterias);

        [OperationContract]
        string GetEditionPageFromSearch(IList<SearchCriteriaContract> searchCriterias, string bookId,
            string versionId, string pageXmlId, string transformationName, OutputFormatEnumContract outputFormat, ResourceLevelEnumContract transformationLevel);

        [OperationContract]
        string GetBookEditionNote(string bookId, string versionId, string transformationName, OutputFormatEnumContract outputFormat, ResourceLevelEnumContract transformationLevel);
    }
}