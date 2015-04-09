using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using System.Threading.Tasks;

namespace ITJakub.MobileApps.MobileContracts
{
    [ServiceContract]
    public interface IMobileAppsService
    {
        [OperationContract]
        Task<IList<BookContract>> GetBookListAsync(BookTypeContract category);

        [OperationContract]
        Task<IList<BookContract>> SearchForBookAsync(BookTypeContract category, SearchDestinationContract searchBy, string query);

        [OperationContract]
        Task<IList<PageContract>> GetPageListAsync(string bookGuid);

        [OperationContract]
        Task<string> GetPageAsRtfAsync(string bookGuid, string pageName);

        [OperationContract]
        Task<Stream> GetPagePhotoAsync(string bookGuid, string pageName);
    }
}
