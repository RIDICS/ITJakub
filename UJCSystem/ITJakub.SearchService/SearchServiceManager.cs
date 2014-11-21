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

        public string GetBookPageByPosition(string documentId, int pagePosition, string transformationName)
        {
            return m_bookDao.GetPageByPositionFromStart(documentId, pagePosition,transformationName);
        }

        public string GetBookPageByName(string documentId, string pageName, string transformationName)
        {
            return m_bookDao.GetPageByName(documentId, pageName, transformationName);
        }

        public string GetBookPagesByName(string documentId, string startPageName, string endPageName, string transformationName)
        {
            return m_bookDao.GetPagesByName(documentId, startPageName, endPageName, transformationName);
        }

        public IList<BookPage> GetBookPageList(string documentId)
        {
            return m_bookDao.GetBookPageList(documentId);
        }
    }
}