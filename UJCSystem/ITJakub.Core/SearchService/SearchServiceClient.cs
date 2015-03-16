using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using ITJakub.Shared.Contracts;

namespace ITJakub.Core.SearchService
{
    public class SearchServiceClient : ClientBase<ISearchService>, ISearchService
    {
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

        public async Task<IList<BookPage>> GetBookPageListAsync(string bookId,string versionId)
        {
            return await Channel.GetBookPageListAsync(bookId, versionId);
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
    }
}
