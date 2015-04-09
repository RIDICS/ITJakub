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

        public Task<string> GetBookPageByXmlIdAsync(string bookGuid, string versionId, string xmlId, string transformationName, ResourceLevelEnumContract transformationLevel)
        {
            return m_searchServiceManager.GetBookPageByXmlIdAsync(bookGuid, versionId, xmlId, transformationName, transformationLevel);
        }

        public async Task<string> GetBookPageByPositionAsync(string bookId, string versionId, int pagePosition, string transformationName, ResourceLevelEnumContract transformationLevel)
        {
            return await m_searchServiceManager.GetBookPageByPositionAsync(bookId, versionId, pagePosition, transformationName, transformationLevel);
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

        public async Task<string> GetBookPageByNameAsync(string bookId, string versionId, string pageName, string transformationName, ResourceLevelEnumContract transformationLevel)
        {
            return await m_searchServiceManager.GetBookPageByNameAsync(bookId, versionId, pageName, transformationName, transformationLevel);
        }

        public async Task<string> GetBookPagesByNameAsync(string bookId, string versionId, string startPageName, string endPageName, string transformationName, ResourceLevelEnumContract transformationLevel)
        {
            return await m_searchServiceManager.GetBookPagesByNameAsync(bookId, versionId, startPageName, endPageName, transformationName, transformationLevel);
        }

        public async Task<IList<BookPageContract>> GetBookPageListAsync(string bookId, string versionId)
        {
            return await m_searchServiceManager.GetBookPageListAsync(bookId, versionId);
        }
    }

    [ServiceContract]
    public interface ISearchServiceLocal : ISearchService
    {
    }
}