using System.Collections.Generic;
using ITJakub.Contracts.Categories;
using ITJakub.Contracts.Searching;
using ITJakub.Core.Database;

namespace ITJakub.Core
{
    public class ItJakubServiceManager
    {
        private readonly ReleationDatabaseMock m_releationDatabaseMock;
        private readonly SearchServiceClient m_searchClient;

        public ItJakubServiceManager(SearchServiceClient searchClient, ReleationDatabaseMock releationDatabaseMock)
        {
            m_searchClient = searchClient;
            m_releationDatabaseMock = releationDatabaseMock;
        }

        public List<SearchResultWithKwicContext> GetContextForKeyWord(string keyWord)
        {
            return m_searchClient.GetContextForKeyWord(keyWord);
        }

        public SelectionBase[] GetCategoryChildrenById(string categoryId)
        {
            return m_releationDatabaseMock.GetChildren(categoryId).ToArray();
        }

        public SelectionBase[] GetRootCategories()
        {
            return m_releationDatabaseMock.GetRootCategories().ToArray();
        }

        public KeyWordsResponse GetAllExtendedTermsForKey(string key, List<string> categorieIds, List<string> booksIds)
        {
            if(categorieIds == null)
                categorieIds = new List<string>();
            if (booksIds == null)
                booksIds = new List<string>();

            var result = new KeyWordsResponse();
           

            List<string> bookIdsByCategories = m_releationDatabaseMock.GetBookIdsByCategories(categorieIds);

            foreach (var bookId in booksIds)
            {
                if(!bookIdsByCategories.Contains(bookId))   
                    bookIdsByCategories.Add(bookId);
            }

            var selectedTreePart = m_releationDatabaseMock.GetSelectedTreePart(categorieIds, booksIds);



            List<string> keyWords;
            if (bookIdsByCategories.Count == 0)
                keyWords = m_searchClient.AllExtendedTermsForKey(key);
            else
                keyWords = m_searchClient.AllExtendedTermsForKeyWithBooksRestriction(key, bookIdsByCategories);


            result.FoundTerms = keyWords.ToArray();
            result.CategoryTree = selectedTreePart;

            return result;
        }
    }
}