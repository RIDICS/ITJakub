using System.ServiceModel;

namespace ITJakub.Shared.Contracts
{
    [ServiceContract]
    public interface ISearchService
    {
        [OperationContract]
        string GetBookPageByPosition(string documentId, int pagePosition);

        [OperationContract]
        string GetBookPageByName(string documentId, string pageName);

        [OperationContract]
        string GetBookPagesByName(string documentId, string startPageName, string endPageName);
    }
}
