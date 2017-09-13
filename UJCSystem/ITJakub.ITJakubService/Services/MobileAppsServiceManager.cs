using System.Collections.Generic;
using System.IO;
using Castle.Windsor;
using ITJakub.ITJakubService.Core;
using ITJakub.MobileApps.MobileContracts;
using ITJakub.MobileApps.MobileContracts.News;
using Vokabular.Shared.DataContracts.Types;
using BookContract = ITJakub.MobileApps.MobileContracts.BookContract;
using OutputFormatEnumContract = ITJakub.Shared.Contracts.OutputFormatEnumContract;

namespace ITJakub.ITJakubService.Services
{
    public class MobileAppsServiceManager : IMobileAppsService
    {
        private readonly BookManager m_bookManager;
        private readonly SearchManager m_searchManager;
        private readonly WindsorContainer m_container = Container.Current;
        private readonly NewsManager m_newsManager;

        public MobileAppsServiceManager()
        {
            m_bookManager = m_container.Resolve<BookManager>();
            m_searchManager = m_container.Resolve<SearchManager>();
            m_newsManager = m_container.Resolve<NewsManager>();
        }

        public IList<BookContract> GetBookList(BookTypeContract category)
        {
            return m_searchManager.GetBooksByBookType(category);
        }

        public IList<BookContract> SearchForBook(BookTypeContract category, SearchDestinationContract searchBy, string query)
        {
            return m_searchManager.Search(category, searchBy, query);
        }

        public IList<PageContract> GetPageList(string bookGuid)
        {
            return m_bookManager.GetBookPagesListMobile(bookGuid);
        }

		public string GetPageAsRtf(string bookGuid, string pageId)
        {
            return m_bookManager.GetBookPageByXmlId(bookGuid, pageId, OutputFormatEnumContract.Rtf, BookTypeEnumContract.Edition);
        }

        public Stream GetPagePhoto(string bookGuid, string pageId)
        {
            return m_bookManager.GetBookPageImage(bookGuid, pageId);
        }

        public BookContract GetBookInfo(string bookGuid)
        {
            return m_bookManager.GetBookInfoMobile(bookGuid);
        }

        public IList<NewsSyndicationItemContract> GetNewsForMobileApps(int start, int count)
        {
            return m_newsManager.GetNewsForMobileApps(start, count);
        }
    }
}