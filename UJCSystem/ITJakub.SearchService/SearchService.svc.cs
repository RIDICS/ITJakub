using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using ITJakub.Shared.Contracts;

namespace ITJakub.SearchService
{
    public class SearchService : ISearchServiceLocal
    {
        private readonly SearchServiceManager m_searchServiceManager;

        public SearchService()
        {
            m_searchServiceManager = Container.Current.Resolve<SearchServiceManager>();
        }

        public async Task<string> GetBookPageByPositionAsync(string bookId, string versionId, int pagePosition, string transformationName)
        {
            return await m_searchServiceManager.GetBookPageByPositionAsync(bookId, versionId, pagePosition, transformationName);
        }

        public async Task UploadVersionFileAsync(VersionResourceUploadContract versionResourceUploadContract)
        {
            await m_searchServiceManager.UploadVersionFileAsync(versionResourceUploadContract);
        }

        public async Task UploadBookFileAsync(BookResourceUploadContract contract)
        {
            await m_searchServiceManager.UploadBookFileAsync(contract);
        }

        public async Task UploadSharedFileAsync(ResourceUploadContract contract)
        {
            await m_searchServiceManager.UploadSharedFileAsync(contract);
        }

        public async Task<string> GetBookPageByNameAsync(string bookId, string versionId, string pageName, string transformationName)
        {
            return await m_searchServiceManager.GetBookPageByNameAsync(bookId, versionId, pageName, transformationName);
        }

        public async Task<string> GetBookPagesByNameAsync(string bookId, string versionId, string startPageName, string endPageName, string transformationName)
        {
            return await m_searchServiceManager.GetBookPagesByNameAsync(bookId, versionId, startPageName, endPageName, transformationName);
        }

        public async Task<IList<BookPage>> GetBookPageListAsync(string bookId, string versionId)
        {
            return await m_searchServiceManager.GetBookPageListAsync(bookId, versionId);
        }
    }

    [ServiceContract]
    public interface ISearchServiceLocal : ISearchService
    {
    }
}