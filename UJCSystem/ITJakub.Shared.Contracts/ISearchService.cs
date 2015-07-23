using System.Collections.Generic;
using System.ServiceModel;
using ITJakub.Shared.Contracts.Searching.Criteria;

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
        void ListSearchEditionsResults(List<SearchCriteriaContract> searchCriterias);

        [OperationContract]
        string ListSearchDictionariesResults(List<SearchCriteriaContract> searchCriterias);

        [OperationContract]
        int ListSearchDictionariesResultsCount(List<SearchCriteriaContract> searchCriterias);

        [OperationContract]
        int GetSearchCriteriaResultsCount(List<SearchCriteriaContract> nonMetadataCriterias);
    }
}