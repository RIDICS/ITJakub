using System.Collections.Generic;
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

        public string GetBookPageByPosition(string bookId, string versionId, int pagePosition, string transformationName, ResourceLevelEnumContract transformationLevel)
        {
            return m_bookDao.GetPageByPositionFromStart(bookId, versionId, pagePosition, transformationName, transformationLevel);
        }

        public string GetBookPageByName(string bookId, string versionId, string pageName, string transformationName, ResourceLevelEnumContract transformationLevel)
        {
            return m_bookDao.GetPageByName(bookId, versionId, pageName, transformationName, transformationLevel);
        }

        public string GetBookPagesByName(string bookId, string versionId, string startPageName, string endPageName, string transformationName, ResourceLevelEnumContract transformationLevel)
        {
            return m_bookDao.GetPagesByName(bookId, versionId, startPageName, endPageName, transformationName, transformationLevel);
        }

        public void UploadVersionFile(VersionResourceUploadContract contract)
        {
            m_bookDao.UploadVersionFile(contract.BookId, contract.BookVersionId, contract.FileName, contract.DataStream);
        }

        public void UploadBookFile(BookResourceUploadContract contract)
        {
            m_bookDao.UploadBookFile(contract.BookId, contract.FileName, contract.DataStream);
        }

        public void UploadSharedFile(ResourceUploadContract contract)
        {
            m_bookDao.UploadSharedFile(contract.FileName, contract.DataStream);
        }
    }
}