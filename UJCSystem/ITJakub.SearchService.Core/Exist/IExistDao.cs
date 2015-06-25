using System.IO;
using System.ServiceModel;
using ITJakub.SearchService.Core.Exist.Attributes;
using ITJakub.Shared.Contracts;

namespace ITJakub.SearchService.Core.Exist
{
    [ServiceContract]
    public interface IExistClient
    {
        [OperationContract]
        [ExistQuery(XqueryName = "get-pages.xquery")]
        string GetPageByPositionFromStart(string bookId, string versionId, int page, string outputFormat);

        [OperationContract]
        [ExistQuery(XqueryName = "get-pages.xquery")]
				string GetPageByName(string bookId, string versionId, string start, string outputFormat);
        
        [OperationContract]
        [ExistQuery(XqueryName = "get-pages.xquery")]
				string GetPageByXmlId(string bookId, string versionId, string pageXmlId, string outputFormat);

        [OperationContract]
        [ExistQuery(XqueryName = "get-pages.xquery")]
				string GetPagesByName(string bookId, string versionId, string start, string end, string outputFormat);
        
        [OperationContract]
        [ExistResource(Method = "PUT", Type = ResourceLevelEnumContract.Version)]
        void UploadVersionFile(string bookId, string versionId, string fileName, Stream dataStream);

        [OperationContract]
        [ExistResource(Method = "PUT", Type = ResourceLevelEnumContract.Book)]
        void UploadBookFile(string bookId, string fileName, Stream dataStream);

        [OperationContract]
        [ExistResource(Method = "PUT", Type = ResourceLevelEnumContract.Shared)]
        void UploadSharedFile(string fileName, Stream dataStream);
    }
}