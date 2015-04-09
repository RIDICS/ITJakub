using System;
using System.Collections.Generic;
using System.IO;
using Castle.Windsor;
using ITJakub.ITJakubService.Core;
using ITJakub.MobileApps.MobileContracts;
using OutputFormatEnumContract = ITJakub.Shared.Contracts.OutputFormatEnumContract;

namespace ITJakub.ITJakubService.Services
{
    public class MobileAppsServiceManager : IMobileAppsService
    {
        private readonly BookManager m_bookManager;
        private readonly MobileManager m_mobileManager;
        private readonly WindsorContainer m_container = Container.Current;
        
        public MobileAppsServiceManager()
        {
            m_bookManager = m_container.Resolve<BookManager>();
            m_mobileManager = m_container.Resolve<MobileManager>();
        }

        public IList<BookContract> GetBookList(CategoryContract category)
        {
            return m_mobileManager.GetBooksByCategory(category);
        }

        public IList<BookContract> SearchForBook(CategoryContract category, SearchDestinationContract searchBy, string query)
        {
            throw new NotImplementedException();
        }

        public IList<PageContract> GetPageList(string bookGuid)
        {
            throw new NotImplementedException();
        }

        public string GetPageAsRtf(string bookGuid, string pageId)
        {
            // TODO switch contracts to async
            return m_bookManager.GetBookPageByNameAsync(bookGuid, pageId, OutputFormatEnumContract.Rtf).Result;
        }

        public Stream GetPagePhoto(string bookGuid, string pageId)
        {
            throw new NotImplementedException();
        }
    }
}