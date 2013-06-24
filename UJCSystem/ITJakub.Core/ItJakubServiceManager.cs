using System.Collections.Generic;
using ITJakub.Contracts.Categories;
using ITJakub.Contracts.Searching;
using ITJakub.Core.Database;
using ITJakub.Xml.XMLOperations;

namespace ITJakub.Core
{
    public class ItJakubServiceManager
    {
        private readonly ReleationDatabaseMock m_releationDatabaseMock;
        private readonly SearchServiceClient m_searchClient;


        public ItJakubServiceManager(SearchServiceClient searchClient, ReleationDatabaseMock releationDatabaseMock, XslTransformDirector xsltTransformDirector)
        {
            m_searchClient = searchClient;

            m_releationDatabaseMock = releationDatabaseMock;
        }

        public List<SearchResultWithHtmlContext> GetContextForKeyWord(string keyWord, List<string> categorieIds, List<string> booksIds)
        {
            var bookIdsByCategories = GetBookIdsByCategorie(ref categorieIds, ref booksIds);
            if (bookIdsByCategories.Count == 0)
                return m_searchClient.GetHtmlContextForKeyWord(keyWord);
            else
                return m_searchClient.GetHtmlContextForKeyWordWithBooksRestriction(keyWord, bookIdsByCategories);
          
        }

        public List<SelectionBase> GetCategoryChildrenById(string categoryId)
        {
            return m_releationDatabaseMock.GetChildren(categoryId);
        }

        public List<SelectionBase> GetRootCategories()
        {
            return m_releationDatabaseMock.GetRootCategories();
        }

        public KeyWordsResponse GetAllExtendedTermsForKey(string key, List<string> categorieIds, List<string> booksIds)
        {

            var result = new KeyWordsResponse();

            var bookIdsByCategories = GetBookIdsByCategorie(ref categorieIds, ref booksIds);

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

        private List<string> GetBookIdsByCategorie(ref List<string> categorieIds, ref List<string> booksIds)
        {
            if (categorieIds == null)
                categorieIds = new List<string>();
            if (booksIds == null)
                booksIds = new List<string>();
            List<string> bookIdsByCategories = m_releationDatabaseMock.GetBookIdsByCategories(categorieIds);

            foreach (var bookId in booksIds)
            {
                if (!bookIdsByCategories.Contains(bookId))
                    bookIdsByCategories.Add(bookId);
            }
            return bookIdsByCategories;
        }
    }
}