using System;
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
        private readonly MobileManager m_mobileManager;
        private readonly WindsorContainer m_container = Container.Current;
        
        public MobileAppsServiceManager()
        {
            m_bookManager = m_container.Resolve<BookManager>();
            m_mobileManager = m_container.Resolve<MobileManager>();
        }

        public Task<IList<BookContract>> GetBookListAsync(CategoryContract category)
        {
            return Task.Run(() => m_mobileManager.GetBooksByCategory(category));
        }

        public Task<IList<BookContract>> SearchForBookAsync(CategoryContract category, SearchDestinationContract searchBy, string query)
        {
            throw new NotImplementedException();
        }

        public Task<IList<PageContract>> GetPageListAsync(string bookGuid)
        {
            throw new NotImplementedException();
        }

        public async Task<string> GetPageAsRtfAsync(string bookGuid, string pageId)
        {
            return await m_bookManager.GetBookPageByNameAsync(bookGuid, pageId, OutputFormatEnumContract.Rtf);
        }

        public Task<Stream> GetPagePhotoAsync(string bookGuid, string pageId)
        {
            throw new NotImplementedException();
        }
    }
}