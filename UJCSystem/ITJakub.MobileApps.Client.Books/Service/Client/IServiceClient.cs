using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ITJakub.MobileApps.MobileContracts;

namespace ITJakub.MobileApps.Client.Books.Service.Client
{
    public interface IServiceClient
    {
        Task<IList<BookContract>> GetBookListAsync(CategoryContract category);
        Task<IList<BookContract>> SearchForBookAsync(CategoryContract category, SearchDestinationContract searchBy, string query);
        Task<IList<PageContract>> GetPageListAsync(string bookGuid);
        Task<string> GetPageAsRtfAsync(string bookGuid, string pageId);
        Task<Stream> GetPagePhotoAsync(string bookGuid, string pageId);
    }
}