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

        public string GetPagesByName(string documentId, string start, string end, string transformationPath)
        {
            return m_existManager.GetPagesByName(documentId, start, end);
        }

        public string GetPageByName(string documentId, string pageName, string transformationPath)
        {
            return m_existManager.GetPageByName(documentId, pageName);
        }

        public string GetPageByPositionFromStart(string documentId, int pagePosition, string transformationPath)
        {
            return m_existManager.GetPageByPositionFromStart(documentId, pagePosition);
        }

        public IList<BookPage> GetBookPageList(string documentId)
        {
            return m_existManager.GetPageList(documentId);
        }

        public void UploadFile(string bookId, string bookVersionid, string fileName, Stream dataStream)
        {
            m_existManager.UploadFile(bookId, bookVersionid, fileName, dataStream);
        }
    }
}