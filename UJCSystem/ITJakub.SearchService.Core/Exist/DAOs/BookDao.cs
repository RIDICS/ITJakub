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

        public string GetPagesByName(string bookId, string versionId, string start, string end, string transformationName, ResourceLevelEnumContract transformationLevel)
        {
            return m_existManager.GetPagesByName(bookId, versionId, start, end, transformationName, transformationLevel);
        }

        public string GetPageByName(string bookId, string versionId, string pageName, string transformationName, ResourceLevelEnumContract transformationLevel)
        {
            return m_existManager.GetPageByName(bookId, versionId, pageName, transformationName, transformationLevel);
        }

        public string GetPageByPositionFromStart(string bookId, string versionId, int pagePosition, string transformationName, ResourceLevelEnumContract transformationLevel)
        {
            return m_existManager.GetPageByPositionFromStart(bookId, versionId, pagePosition, transformationName, transformationLevel);
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