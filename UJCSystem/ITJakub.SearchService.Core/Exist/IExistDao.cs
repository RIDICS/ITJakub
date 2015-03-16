using System.IO;
using System.ServiceModel;
using System.Threading.Tasks;
using ITJakub.SearchService.Core.Exist.Attributes;

namespace ITJakub.SearchService.Core.Exist
{
    [ServiceContract]
    public interface IExistClient
    {
        [OperationContract]
        [ExistQuery(XqueryName = "get-page-list.xquery")]
        Task<Stream> GetPageListAsync(string bookId, string versionId);

        [OperationContract]
        [ExistQuery(XqueryName = "get-pages.xquery")]
        Task<string> GetPageByPositionFromStart(string bookId, string versionId, int page);

        [OperationContract]
        [ExistQuery(XqueryName = "get-pages.xquery")]
        Task<string> GetPageByName(string bookId, string versionId, string start);

        [OperationContract]
        [ExistQuery(XqueryName = "get-pages.xquery")]
        Task<string> GetPagesByName(string bookId, string versionId, string start, string end);

        [OperationContract]
        [ExistResource(Method = "PUT", Type = ResourceLevelType.Version)]
        Task UploadVersionFileAsync(string bookId, string versionId, string fileName, Stream dataStream);

        [OperationContract]
        [ExistResource(Method = "PUT", Type = ResourceLevelType.Book)]
        Task UploadBookFileAsync(string bookId, string fileName, Stream dataStream);

        [OperationContract]
        [ExistResource(Method = "PUT", Type = ResourceLevelType.Shared)]
        Task UploadSharedFileAsync(string fileName, Stream dataStream);
    }
}