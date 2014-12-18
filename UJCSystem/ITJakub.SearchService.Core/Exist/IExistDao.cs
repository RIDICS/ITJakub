using System.IO;
using System.ServiceModel;
using ITJakub.SearchService.Core.Exist.Attributes;

namespace ITJakub.SearchService.Core.Exist
{
    [ServiceContract]
    public interface IExistClient
    {
        [OperationContract]
        [ExistQuery(XqueryName = "get-page-list.xquery")]
        Stream GetPageList(string bookId, string versionId);

        [OperationContract]
        [ExistQuery(XqueryName = "get-pages.xquery")]
        string GetPageByPositionFromStart(string bookId, string versionId, int page);

        [OperationContract]
        [ExistQuery(XqueryName = "get-pages.xquery")]
        string GetPageByName(string bookId, string versionId, string start);

        [OperationContract]
        [ExistQuery(XqueryName = "get-pages.xquery")]
        string GetPagesByName(string bookId, string versionId, string start, string end);

        [OperationContract]
        [ExistResource(Method = "PUT", Type = ResourceLevelType.Version)]
        void UploadVersionFile(string bookId, string versionId, string fileName, Stream dataStream);

        [OperationContract]
        [ExistResource(Method = "PUT", Type = ResourceLevelType.Book)]
        void UploadBookFile(string bookId, string fileName, Stream dataStream);

        [OperationContract]
        [ExistResource(Method = "PUT", Type = ResourceLevelType.Shared)]
        void UploadSharedFile(string fileName, Stream dataStream);
    }
}