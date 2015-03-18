using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ITJakub.Shared.Contracts;

namespace ITJakub.SearchService.Core.Exist.DAOs
{
    public class BookDao
    {
        private readonly ExistManager m_existManager;

        public BookDao(ExistManager existManager)
        {
            m_existManager = existManager;
        }

        public async Task<string> GetPagesByName(string bookId, string versionId, string start, string end, string transformationPath)
        {
            return await m_existManager.GetPagesByName(bookId, versionId, start, end);
        }

        public async Task<string> GetPageByName(string bookId, string versionId, string pageName, string transformationPath)
        {
            return await m_existManager.GetPageByName(bookId, versionId, pageName);
        }

        public async Task<string> GetPageByPositionFromStart(string bookId, string versionId, int pagePosition, string transformationPath)
        {
            return await m_existManager.GetPageByPositionFromStart(bookId, versionId, pagePosition);
        }

        public async Task<List<BookPageContract>> GetBookPageList(string bookId, string versionId)
        {
            return await m_existManager.GetPageList(bookId, versionId);
        }

        public async Task UploadVersionFile(string bookId, string bookVersionid, string fileName, Stream dataStream)
        {
            await m_existManager.UploadVersionFile(bookId, bookVersionid, fileName, dataStream);
        }

        public async Task UploadBookFile(string bookId, string fileName, Stream dataStream)
        {
            await m_existManager.UploadBookFile(bookId, fileName, dataStream);
        }

        public async Task UploadSharedFile(string fileName, Stream dataStream)
        {
            await m_existManager.UploadSharedFile(fileName, dataStream);
        }
    }
}