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
            var bookIdsByCategories = GetBookIdsByCategorie(categorieIds, booksIds);
            List<SearchResultWithHtmlContext> dbResult;
            if (bookIdsByCategories.Count == 0)
                dbResult = m_searchClient.GetHtmlContextForKeyWord(keyWord);
            else
                dbResult = m_searchClient.GetHtmlContextForKeyWordWithBooksRestriction(keyWord, bookIdsByCategories);


            foreach (SearchResultWithHtmlContext searchResultWithHtmlContext in dbResult)
            {
                var categorieTextClassification = m_releationDatabaseMock.GetCategoryTextClassificationByBookId(searchResultWithHtmlContext.Id);
                searchResultWithHtmlContext.Categories = categorieTextClassification;
                searchResultWithHtmlContext.ShowOrder = m_releationDatabaseMock.GetCategoryShowOrderByBook(searchResultWithHtmlContext.Id);
            }

            return dbResult;
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

            var bookIdsByCategories = GetBookIdsByCategorie(categorieIds, booksIds);

            var selectedTreePart = m_releationDatabaseMock.GetSelectedTreePart(categorieIds, booksIds);

            SearchTermPossibleResult keyWords;
            if (bookIdsByCategories.Count == 0)
                keyWords = m_searchClient.AllExtendedTermsForKey(key);
            else
                keyWords = m_searchClient.AllExtendedTermsForKeyWithBooksRestriction(key, bookIdsByCategories);


            List<Book> possibleBooks = new List<Book>();
            foreach (var bookId in keyWords.AllPossibleBookIds)
            {
                possibleBooks.Add(m_releationDatabaseMock.GetBookById(bookId));
            }
            
            result.FoundTerms = keyWords.AllPossibleTerms;
            result.FoundInBooks = possibleBooks;
            result.CategoryTree = selectedTreePart;

            return result;
        }

        private List<string> GetBookIdsByCategorie(List<string> categorieIds, List<string> booksIds)
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

        public IEnumerable<SearchResult> GetBookBySearchTerm(string searchTerm)
        {
            IEnumerable<SearchResult> dbResult = m_searchClient.GetAllBooksContainingSearchTerm(searchTerm);
            return dbResult;
        }

        public IEnumerable<SearchResult> GetBooksTitleByLetter(string letter)
        {
            
            return m_searchClient.GetBooksByTitleFirstLetter(letter);
        }

        public IEnumerable<SearchResult> GetSourcesAuthorByLetter(string letter)
        {
            return m_searchClient.GetBooksByAuthorFirstLetter(letter);
        }

        public string GetContentByBookId(string id)
        {
            return m_searchClient.GetContentByBookId(id);
        }

        public SearchResult GetBookById(string id)
        {
            return m_searchClient.GetBookById(id);
        }
    }
}