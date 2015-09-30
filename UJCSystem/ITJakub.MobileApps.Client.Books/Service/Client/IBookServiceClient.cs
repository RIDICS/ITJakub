using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ITJakub.MobileApps.MobileContracts;

namespace ITJakub.MobileApps.Client.Books.Service.Client
{
    public interface IBookServiceClient
    {
        Task<IList<BookContract>> GetBookListAsync(BookTypeContract category);

        Task<IList<BookContract>> SearchForBookAsync(BookTypeContract category, SearchDestinationContract searchBy, string query);

        Task<IList<PageContract>> GetPageListAsync(string bookGuid);

        Task<string> GetPageAsRtfAsync(string bookGuid, string pageId);

        Task<Stream> GetPagePhotoAsync(string bookGuid, string pageId);

        Task<BookContract> GetBookInfo(string bookGuid);
    }
}
