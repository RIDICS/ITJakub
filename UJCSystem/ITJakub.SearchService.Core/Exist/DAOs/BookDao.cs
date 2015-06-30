using System;
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

        public string GetPagesByName(string bookId, string versionId, string start, string end,
            string transformationName, OutputFormatEnumContract outputFormat,
            ResourceLevelEnumContract transformationLevel)
        {
            return m_existManager.GetPagesByName(bookId, versionId, start, end, transformationName, outputFormat,
                transformationLevel);
        }

        public string GetPageByName(string bookId, string versionId, string pageName, string transformationName,
            OutputFormatEnumContract outputFormat, ResourceLevelEnumContract transformationLevel)
        {
            return m_existManager.GetPageByName(bookId, versionId, pageName, transformationName, outputFormat,
                transformationLevel);
        }

        public string GetPageByPositionFromStart(string bookId, string versionId, int pagePosition,
            string transformationName, OutputFormatEnumContract outputFormat,
            ResourceLevelEnumContract transformationLevel)
        {
            return m_existManager.GetPageByPositionFromStart(bookId, versionId, pagePosition, transformationName,
                outputFormat, transformationLevel);
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

        public string GetPageByXmlId(string bookId, string versionId, string pageXmlId, string transformationName,
            OutputFormatEnumContract outputFormat, ResourceLevelEnumContract transformationLevel)
        {
            return m_existManager.GetPageByXmlId(bookId, versionId, pageXmlId, transformationName, outputFormat,
                transformationLevel);
        }

        public string GetDictionaryEntryByXmlId(string bookId, string versionId, string xmlEntryId,
            string transformationName, OutputFormatEnumContract outputFormat,
            ResourceLevelEnumContract transformationLevel)
        {
            return m_existManager.GetDictionaryEntryByXmlId(bookId, versionId, xmlEntryId, transformationName, outputFormat,
                transformationLevel);
        }
    }
}