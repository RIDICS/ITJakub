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
        Stream GetPageList(string document);

        [OperationContract]
        [ExistQuery(XqueryName = "get-pages.xquery")]
        string GetPageByPositionFromStart(string document, int page);

        [OperationContract]
        [ExistQuery(XqueryName = "get-pages.xquery")]
        string GetPageByName(string document, string start);

        [OperationContract]
        [ExistQuery(XqueryName = "get-pages.xquery")]
        string GetPagesByName(string document, string start, string end);
    
            
        
        [OperationContract]
        [ExistResource(Method = "PUT")]
        void UploadFile(string bookId, string bookVersionId, string fileName, Stream readStream);
    }
}