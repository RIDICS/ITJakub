using System.Collections.Generic;
using System.ServiceModel;

namespace ITJakub.Shared.Contracts
{
    [ServiceContract]
    public interface ISearchService
    {
        [OperationContract]
        string GetBookPageByPosition(string bookId, string versionId, int pagePosition, string transformationName, ResourceLevelEnumContract transformationLevel);

        [OperationContract]
        string GetBookPageByName(string bookId, string versionId, string pageName, string transformationName, ResourceLevelEnumContract transformationLevel);

        [OperationContract]
        string GetBookPageByXmlId(string bookId, string versionId, string pageXmlId, string transformationName, ResourceLevelEnumContract transformationLevel);

        [OperationContract]
        string GetBookPagesByName(string bookId, string versionId, string startPageName, string endPageName, string transformationName, ResourceLevelEnumContract transformationLevel);

        [OperationContract]
        void UploadVersionFile(VersionResourceUploadContract versionResourceUploadContract);

        [OperationContract]
        void UploadBookFile(BookResourceUploadContract contract);

        [OperationContract]
        void UploadSharedFile(ResourceUploadContract contract);
    }
}
