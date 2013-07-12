using System.Collections.Generic;
using ITJakub.Contracts.Categories;
using ITJakub.Contracts.Searching;
using ITJakub.Core;
using ITJakub.MVCWebLayer.Enums;

namespace ITJakub.MVCWebLayer.Services
{
    public class ItJakubSourcesProvider :ISourcesProvider
    {
        private readonly ItJakubServiceClient m_serviceClient;

        public ItJakubSourcesProvider()
        {
            m_serviceClient = Container.Current.Resolve<ItJakubServiceClient>();
        }

        public IEnumerable<SearchResult> GetSearchResult(string searchTerm)
        {
            return m_serviceClient.GetBooksBySearchTerm(searchTerm);
        }

        public SearchResult GetDetail(string id)
        {
            return m_serviceClient.GetBookById(id);
        }

        public IEnumerable<SearchResult> GetSourcesTitleByLetter(string letter)
        {
            return m_serviceClient.GetBooksTitleByLetter(letter);
        }

        public IEnumerable<SearchResult> GetSourcesAuthorByLetter(string letter)
        {
            return m_serviceClient.GetSourcesAuthorByLetter(letter);
        }

        public string GetContentByBookId(string id)
        {
            return m_serviceClient.GetContentByBookId(id);
        }

        public List<SearchResultWithHtmlContext> GetHtmlContextForKeyWord(string searchTerm, string booksIds)
        {
            return m_serviceClient.GetHtmlContextForKeyWord(searchTerm, new List<string>(), new List<string> {booksIds});
        }
    }
}