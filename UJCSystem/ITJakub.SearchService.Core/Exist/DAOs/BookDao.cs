using System.Collections.Generic;
using System.IO;
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

        public string GetPagesByName(string bookId, string versionId, string start, string end, string transformationPath)
        {
            return m_existManager.GetPagesByName(bookId, versionId, start, end);
        }

        public string GetPageByName(string bookId, string versionId, string pageName, string transformationPath)
        {
            return m_existManager.GetPageByName(bookId, versionId, pageName);
        }

        public string GetPageByPositionFromStart(string bookId, string versionId, int pagePosition, string transformationPath)
        {
            return m_existManager.GetPageByPositionFromStart(bookId, versionId, pagePosition);
        }

        public IList<BookPage> GetBookPageList(string bookId, string versionId)
        {
            return m_existManager.GetPageList(bookId, versionId);
        }

        public void UploadVersionFile(string bookId, string bookVersionid, string fileName, Stream dataStream)
        {
            m_existManager.UploadVersionFile(bookId, bookVersionid, fileName, dataStream);
        }

        public void UploadBookFile(string bookId, string fileName, Stream dataStream)
        {
            m_existManager.UploadBookFile(bookId, fileName, dataStream);
        }

        public void UploadSharedFile(string fileName, Stream dataStream)
        {
            m_existManager.UploadSharedFile(fileName, dataStream);
        }
    }
}