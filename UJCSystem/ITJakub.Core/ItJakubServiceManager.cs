using System.Collections.Generic;
using ITJakub.Contracts.Categories;
using ITJakub.Contracts.Searching;
using ITJakub.Core.Database;
using ITJakub.Core.XMLOperations;

namespace ITJakub.Core
{
    public class ItJakubServiceManager
    {
        private readonly ReleationDatabaseMock m_releationDatabaseMock;
        private readonly XslTransformDirector m_xsltTransformDirector;
        private readonly SearchServiceClient m_searchClient;


        public ItJakubServiceManager(SearchServiceClient searchClient, ReleationDatabaseMock releationDatabaseMock, XslTransformDirector xsltTransformDirector)
        {
            m_searchClient = searchClient;
            m_releationDatabaseMock = releationDatabaseMock;
            m_xsltTransformDirector = xsltTransformDirector;
        }

        public List<SearchResultWithHtmlContext> GetContextForKeyWord(string keyWord)
        {
            List<SearchResultWithXmlContext> results = m_searchClient.GetXmlContextForKeyWord(keyWord);
            //TODO transform here
            List<SearchResultWithHtmlContext> resultWithHtml = new List<SearchResultWithHtmlContext>();
            foreach (var result in results)
            {
                var item = new SearchResultWithHtmlContext();
                item.Id = result.Id;
                item.Author = result.Author;
                item.OriginalXml = result.OriginalXml;
                item.Title = result.Title;
                item.Categories = result.Categories;
                item.HtmlContext = m_xsltTransformDirector.TransformResult(result.XmlContext);

                resultWithHtml.Add(item);
            }

            return resultWithHtml;
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