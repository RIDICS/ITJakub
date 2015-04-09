using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Castle.Windsor;
using ITJakub.ITJakubService.Core;
using ITJakub.MobileApps.MobileContracts;
using OutputFormatEnumContract = ITJakub.Shared.Contracts.OutputFormatEnumContract;

namespace ITJakub.ITJakubService.Services
{
    public class MobileAppsServiceManager : IMobileAppsService
    {
        private readonly BookManager m_bookManager;
        private readonly SearchManager m_searchManager;
        private readonly WindsorContainer m_container = Container.Current;

        public MobileAppsServiceManager()
        {
            m_bookManager = m_container.Resolve<BookManager>();
            m_searchManager = m_container.Resolve<SearchManager>();
        }

        public Task<IList<BookContract>> GetBookListAsync(BookTypeContract category)
        {
            return Task.Run(() => m_searchManager.GetBooksByBookType(category));
        }

        public Task<IList<BookContract>> SearchForBookAsync(BookTypeContract category, SearchDestinationContract searchBy, string query)
        {
            return Task.Run(() => m_searchManager.Search(category, searchBy, query));
        }

        public Task<IList<PageContract>> GetPageListAsync(string bookGuid)
        {
            return m_bookManager.GetBookPageListMobileAsync(bookGuid);
        }

        public Task<string> GetPageAsRtfAsync(string bookGuid, string pageName)
        {
            return m_bookManager.GetBookPageByNameAsync(bookGuid, pageName, OutputFormatEnumContract.Html);
        }

        public Task<Stream> GetPagePhotoAsync(string bookGuid, string pageName)
        {
            return Task.Run(() => m_bookManager.GetBookPageImage(bookGuid, pageName));
        }
    }
}