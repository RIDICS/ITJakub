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
        Task<IList<BookContract>> GetBookListAsync(CategoryContract category);

        [OperationContract]
        Task<IList<BookContract>> SearchForBookAsync(CategoryContract category, SearchDestinationContract searchBy, string query);

        [OperationContract]
        Task<IList<PageContract>> GetPageListAsync(string bookGuid);

        [OperationContract]
        Task<string> GetPageAsRtfAsync(string bookGuid, string pageId);

        [OperationContract]
        Task<Stream> GetPagePhotoAsync(string bookGuid, string pageId);
    }
}
