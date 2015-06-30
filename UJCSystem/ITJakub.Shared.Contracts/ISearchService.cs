using System.ServiceModel;

namespace ITJakub.Shared.Contracts
{
    [ServiceContract]
    public interface ISearchService
    {
        [OperationContract]
        string GetBookPageByPosition(string bookId, string versionId, int pagePosition, string transformationName,
            OutputFormatEnumContract outputFormat, ResourceLevelEnumContract transformationLevel);

        [OperationContract]
        string GetBookPageByName(string bookId, string versionId, string pageName, string transformationName,
            OutputFormatEnumContract outputFormat, ResourceLevelEnumContract transformationLevel);

        [OperationContract]
        string GetBookPageByXmlId(string bookId, string versionId, string pageXmlId, string transformationName,
            OutputFormatEnumContract outputFormat, ResourceLevelEnumContract transformationLevel);

        [OperationContract]
        string GetBookPagesByName(string bookId, string versionId, string startPageName, string endPageName,
            string transformationName, OutputFormatEnumContract outputFormat,
            ResourceLevelEnumContract transformationLevel);

        [OperationContract]
        void UploadVersionFile(VersionResourceUploadContract versionResourceUploadContract);

        [OperationContract]
        void UploadBookFile(BookResourceUploadContract contract);

        [OperationContract]
        void UploadSharedFile(ResourceUploadContract contract);

        [OperationContract]
        string GetDictionaryEntryByXmlId(string bookId, string versionId, string xmlEntryId, string transformationName,
            OutputFormatEnumContract outputFormat, ResourceLevelEnumContract transformationLevel);
    }
}