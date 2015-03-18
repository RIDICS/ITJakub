using System.Collections.Generic;
using System.Threading.Tasks;
using ITJakub.SearchService.Core.Exist.DAOs;
using ITJakub.Shared.Contracts;

namespace ITJakub.SearchService
{
    public class SearchServiceManager : ISearchService
    {
        private readonly BookDao m_bookDao;

        public SearchServiceManager(BookDao bookDao)
        {
            m_bookDao = bookDao;
        }

        public async Task<string> GetBookPageByPositionAsync(string bookId, string versionId, int pagePosition, string transformationName)
        {
            return await m_bookDao.GetPageByPositionFromStart(bookId, versionId, pagePosition, transformationName);
        }

        public async Task<string> GetBookPageByNameAsync(string bookId, string versionId, string pageName, string transformationName)
        {
            return await m_bookDao.GetPageByName(bookId, versionId, pageName, transformationName);
        }

        public async Task<string> GetBookPagesByNameAsync(string bookId, string versionId, string startPageName, string endPageName,
            string transformationName)
        {
            return await m_bookDao.GetPagesByName(bookId, versionId, startPageName, endPageName, transformationName);
        }

        public async Task UploadVersionFileAsync(VersionResourceUploadContract contract)
        {
            await m_bookDao.UploadVersionFile(contract.BookId, contract.BookVersionId, contract.FileName, contract.DataStream);
        }

        public async Task UploadBookFileAsync(BookResourceUploadContract contract)
        {
            await m_bookDao.UploadBookFile(contract.BookId, contract.FileName, contract.DataStream);
        }

        public async Task UploadSharedFileAsync(ResourceUploadContract contract)
        {
            await m_bookDao.UploadSharedFile(contract.FileName, contract.DataStream);
        }


        public async Task<IList<BookPageContract>> GetBookPageListAsync(string bookId, string versionId)
        {
            return await m_bookDao.GetBookPageList(bookId, versionId);
        }
    }
}