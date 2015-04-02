using System.Collections.Generic;
using System.IO;
using ITJakub.MobileApps.MobileContracts;

namespace ITJakub.ITJakubService.Services
{
    public class MobileAppsServiceManager : IMobileAppsService
    {
        public IList<BookContract> GetBookList(CategoryContract category)
        {
            return new List<BookContract> {new BookContract{Author = "Pepa", Title = "Aaaaa"}};
        }

        public IList<BookContract> SearchForBook(CategoryContract category, SearchDestinationContract searchBy, string query)
        {
            throw new System.NotImplementedException();
        }

        public IList<string> GetPageList(string bookGuid)
        {
            throw new System.NotImplementedException();
        }

        public Stream GetPageAsRtf(string bookGuid, string pageId)
        {
            throw new System.NotImplementedException();
        }

        public Stream GetPagePhoto(string bookGuid, string pageId)
        {
            throw new System.NotImplementedException();
        }
    }
}