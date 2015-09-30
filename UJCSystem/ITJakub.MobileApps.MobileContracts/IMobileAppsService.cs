using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using ITJakub.MobileApps.MobileContracts.News;

namespace ITJakub.MobileApps.MobileContracts
{
    [ServiceContract]
    public interface IMobileAppsService:INewsService
    {
        [OperationContract]
        IList<BookContract> GetBookList(BookTypeContract category);

        [OperationContract]
        IList<BookContract> SearchForBook(BookTypeContract category, SearchDestinationContract searchBy, string query);

        [OperationContract]
        IList<PageContract> GetPageList(string bookGuid);

        [OperationContract]
        string GetPageAsRtf(string bookGuid, string pageId);

        [OperationContract]
        Stream GetPagePhoto(string bookGuid, string pageId);

        [OperationContract]
        BookContract GetBookInfo(string bookGuid);     
    }
}
