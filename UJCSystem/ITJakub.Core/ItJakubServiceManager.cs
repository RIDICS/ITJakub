using System.Collections.Generic;
using ITJakub.Contracts.Categories;
using ITJakub.Contracts.Searching;
using ITJakub.Core.Database;

namespace ITJakub.Core
{
    public class ItJakubServiceManager
    {
        private readonly SearchServiceClient m_searchClient;
        private readonly ReleationDatabaseMock m_releationDatabaseMock;

        public ItJakubServiceManager(SearchServiceClient searchClient, ReleationDatabaseMock releationDatabaseMock)
        {
            m_searchClient = searchClient;
            m_releationDatabaseMock = releationDatabaseMock;
        }

        public SearchResult[] GetContextForKeyWord(string keyWord)
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

        public List<string> GetAllExtendedTermsForKey(string key, List<string> categorieIds, List<string> booksIds)
        {

            var bookIdsByCategories = m_releationDatabaseMock.GetBookIdsByCategories(categorieIds);

            bookIdsByCategories.AddRange(booksIds);


            return m_searchClient.AllExtendedTermsForKey(key, bookIdsByCategories);
        }
    }
}