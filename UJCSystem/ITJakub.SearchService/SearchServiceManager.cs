using ITJakub.SearchService.Core.Exist.DAOs;
using ITJakub.Shared.Contracts;

namespace ITJakub.SearchService
{
    public class SearchServiceManager:ISearchService
    {
        private readonly BookDao m_bookDao;

        public SearchServiceManager(BookDao bookDao)
        {
            m_bookDao = bookDao;
        }

        public string GetBookPageByPosition(string documentId, int pagePosition)
        {
            return m_bookDao.GetPageByPositionFromStart(documentId,pagePosition);
        }

        public string GetBookPageByName(string documentId, string pageName)
        {
            return m_bookDao.GetPageByName(documentId, pageName);
        }

        public string GetBookPagesByName(string documentId, string startPageName, string endPageName)
        {
            return m_bookDao.GetPagesByName(documentId, startPageName, endPageName);
        }
    }
}