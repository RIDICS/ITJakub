using System.Collections.Generic;
using System.IO;
using System.ServiceModel;

namespace ITJakub.MobileApps.MobileContracts
{
    [ServiceContract]
    public interface IMobileAppsService
    {
        [OperationContract]
        IList<BookContract> GetBookList(CategoryContract category);

        [OperationContract]
        IList<BookContract> SearchForBook(CategoryContract category, SearchDestinationContract searchBy, string query);

        [OperationContract]
        IList<PageContract> GetPageList(string bookGuid);

        [OperationContract]
        string GetPageAsRtf(string bookGuid, string pageId);

        [OperationContract]
        Stream GetPagePhoto(string bookGuid, string pageId);
    }
}
