using System.Collections.Generic;
using System.ServiceModel;
using ITJakub.ITJakubService.DataContracts;
using ITJakub.Shared.Contracts.Searching.Criteria;
using ITJakub.Shared.Contracts.Searching.Results;

namespace ITJakub.Shared.Contracts
{
    [ServiceContract]
    public interface ISearchService
    {

        [OperationContract]
        string GetBookPageByXmlId(string bookId, string versionId, string pageXmlId, string transformationName,
            OutputFormatEnumContract outputFormat, ResourceLevelEnumContract transformationLevel);

        [OperationContract]
        void UploadVersionFile(VersionResourceUploadContract versionResourceUploadContract);

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
        string GetEditionPageFromSearch(IList<SearchCriteriaContract> searchCriterias, string bookId,
            string versionId, string pageXmlId, string transformationName, OutputFormatEnumContract outputFormat, ResourceLevelEnumContract transformationLevel);
    }
}