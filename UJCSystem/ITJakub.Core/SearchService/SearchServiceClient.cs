using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.ServiceModel;
using System.Threading.Tasks;
using ITJakub.Shared.Contracts;
using log4net;

namespace ITJakub.Core.SearchService
{
    public class SearchServiceClient : ClientBase<ISearchService>, ISearchService
    {
        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public async Task<string> GetBookPageByPositionAsync(string bookId, string versionId, int pagePosition, string transformationName)
        {
            return await Channel.GetBookPageByPositionAsync(bookId, versionId, pagePosition, transformationName);
        }

        public async Task<string> GetBookPageByNameAsync(string bookId, string versionId, string pageName, string transformationName)
        {
            return await Channel.GetBookPageByNameAsync(bookId, versionId, pageName, transformationName);
        }

        public async Task<string> GetBookPagesByNameAsync(string bookId, string versionId, string startPageName, string endPageName, string transformationName)
        {
            return await Channel.GetBookPagesByNameAsync(bookId, versionId, startPageName, endPageName, transformationName);
        }

        public async Task<IList<BookPageContract>> GetBookPageListAsync(string bookId,string versionId)
        {
            try
            {
                return await Channel.GetBookPageListAsync(bookId, versionId);
            }
            catch (CommunicationException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("{0} failed with: {1}", GetCurrentMethod(), ex);
                throw;
            }
            catch (TimeoutException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("{0} failed with: {1}", GetCurrentMethod(), ex);
                throw;
            }
            catch (ObjectDisposedException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("{0} failed with: {1}", GetCurrentMethod(), ex);
                throw;
            }

        }

        public async Task UploadVersionFileAsync(VersionResourceUploadContract versionResourceUploadContract)
        {
            await Channel.UploadVersionFileAsync(versionResourceUploadContract);
        }

        public async Task UploadBookFileAsync(BookResourceUploadContract contract)
        {
            await Channel.UploadBookFileAsync(contract);
        }

        public async Task UploadSharedFileAsync(ResourceUploadContract contract)
        {
            await Channel.UploadSharedFileAsync(contract);
        }



        private string GetCurrentMethod([CallerMemberName] string methodName = null)
        {
            return methodName;
        }
    }
}
