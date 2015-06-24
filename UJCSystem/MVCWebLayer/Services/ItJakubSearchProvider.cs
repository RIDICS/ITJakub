using System.Collections.Generic;
using ITJakub.Contracts.Categories;
using ITJakub.Contracts.Searching;
using ITJakub.Core;

namespace ITJakub.MVCWebLayer.Services
{
    public class ItJakubSearchProvider : ISearchResultProvider
    {

        private readonly ItJakubServiceClient m_serviceClient;
        private List<SearchResultWithHtmlContext> m_searchResult;
        public ItJakubSearchProvider()
        {
            m_serviceClient = Container.Current.Resolve<ItJakubServiceClient>();
        }
        

        public KeyWordsResponse GetSearchResults(string query, List<string> categorieIds, List<string> booksIds)
        {
            KeyWordsResponse response = m_serviceClient.GetAllExtendedTermsForKey(query, categorieIds, booksIds);

            return response;
        }

        public List<SearchResultWithHtmlContext> GetSearchResultsByType(string book, string searchTerm)
        {
            if (m_searchResult == null)
                m_searchResult = m_serviceClient.GetResultsByBooks(book, searchTerm);
            return m_searchResult;
        }


        public List<SearchResultWithHtmlContext> GetHtmlContextForKeyWord(string searchTerm, List<string> categorieIds, List<string> booksIds)
        {
            if (m_searchResult == null)
                m_searchResult = m_serviceClient.GetHtmlContextForKeyWord(searchTerm, categorieIds, booksIds);
            return m_searchResult;
        }

        public List<SelectionBase> GetCategoryChildrenById(string categoryId)
        {
            return m_serviceClient.GetCategoryChildrenById(categoryId);
        }

        public List<SelectionBase> GetRootCategories()
        {
            return m_serviceClient.GetRootCategories();
        }
    }

    
}