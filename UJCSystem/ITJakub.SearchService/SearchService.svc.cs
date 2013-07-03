using System.Collections.Generic;
using System.ServiceModel;
using ITJakub.Contracts;
using ITJakub.Contracts.Searching;

namespace ITJakub.SearchService
{
    public class SearchService : ISearchServiceLocal
    {
        private readonly SearchServiceManager m_searchServiceManager;

        public SearchService()
        {
            m_searchServiceManager = Container.Current.Resolve<SearchServiceManager>();
        }

        public void Search(List<SearchCriteriumBase> criteria)
        {
            m_searchServiceManager.Search(criteria);
        }

        public List<SearchResultWithKwicContext> GetKwicContextForKeyWord(string keyWord)
        {
            return m_searchServiceManager.GetKwicContextForKeyWord(keyWord);
        }

        public List<SearchResultWithXmlContext> GetXmlContextForKeyWord(string keyWord)
        {
            return m_searchServiceManager.GetXmlContextForKeyWord(keyWord);
        }

        public List<SearchResultWithHtmlContext> GetHtmlContextForKeyWord(string keyWord)
        {
            return m_searchServiceManager.GetHtmlContextForKeyWord(keyWord);
        }

        public List<SearchResultWithHtmlContext> GetHtmlContextForKeyWordWithBooksRestriction(string keyWord, List<string> bookIds)
        {
            return m_searchServiceManager.GetHtmlContextForKeyWord(keyWord, bookIds);
        }

        public IEnumerable<SearchResult> GetAllBooksContainingSearchTerm(string searchTerm)
        {
            return m_searchServiceManager.GetAllBooksContainingSearchTerm(searchTerm);
        }

        public string GetTitleById(string id)
        {
            return m_searchServiceManager.GetTitleById(id);
        }

        public SearchTermPossibleResult AllExtendedTermsForKey(string key)
        {
            return m_searchServiceManager.AllExtendedTermsForKey(key);
        }

        public SearchTermPossibleResult AllExtendedTermsForKeyWithBooksRestriction(string key, List<string> booksIds)
        {
            return m_searchServiceManager.AllExtendedTermsForKeyWithBooksRestriction(key, booksIds);
        }

    }

    [ServiceContract]
    public interface ISearchServiceLocal:ISearchService
    {
    }
}
