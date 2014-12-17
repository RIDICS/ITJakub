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

        public string GetBookPageByPosition(string bookId, string versionId, int pagePosition, string transformationName)
        {
            return m_bookDao.GetPageByPositionFromStart(bookId, versionId, pagePosition, transformationName);
        }

        public string GetBookPageByName(string bookId, string versionId, string pageName, string transformationName)
        {
            return m_bookDao.GetPageByName(bookId, versionId, pageName, transformationName);
        }

        public string GetBookPagesByName(string bookId, string versionId, string startPageName, string endPageName,
            string transformationName)
        {
            return m_bookDao.GetPagesByName(bookId, versionId, startPageName, endPageName, transformationName);
        }

        public void UploadFile(FileUploadContract contract)
        {
            m_bookDao.UploadFile(contract.BookId, contract.BookVersionId, contract.FileName, contract.DataStream);
        }

        public IList<BookPage> GetBookPageList(string bookId, string versionId)
        {
            return m_bookDao.GetBookPageList(bookId, versionId);
        }
    }
}