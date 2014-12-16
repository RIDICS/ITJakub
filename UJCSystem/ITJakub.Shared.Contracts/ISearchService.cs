using System.Collections.Generic;
using System.ServiceModel;

namespace ITJakub.Shared.Contracts
{
    [ServiceContract]
    public interface ISearchService
    {
        [OperationContract]
        string GetBookPageByPosition(string bookId, string versionId, int pagePosition, string transformationName);

        [OperationContract]
        string GetBookPageByName(string bookId, string versionId, string pageName, string transformationName);

        [OperationContract]
        string GetBookPagesByName(string bookId, string versionId, string startPageName, string endPageName, string transformationName);

        [OperationContract]
        IList<BookPage> GetBookPageList(string bookId,string versionId);

        [OperationContract]
        void UploadFile(FileUploadContract fileUploadContract);

    }
}
