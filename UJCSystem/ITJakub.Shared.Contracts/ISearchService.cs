using System.Collections.Generic;
using System.ServiceModel;

namespace ITJakub.Shared.Contracts
{
    [ServiceContract]
    public interface ISearchService
    {
        [OperationContract]
        string GetBookPageByPosition(string documentId, int pagePosition, string transformationName);

        [OperationContract]
        string GetBookPageByName(string documentId, string pageName, string transformationName);

        [OperationContract]
        string GetBookPagesByName(string documentId, string startPageName, string endPageName, string transformationName);

        [OperationContract]
        IList<BookPage> GetBookPageList(string documentId);
    }
}
