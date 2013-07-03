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

            return new List<SearchResult>();
        }

        public Book GetDetail(string id)
        {
            return new Book();
        }

        public IEnumerable<Book> GetSources(string query, SourcesViewMode mode)
        {
            return new List<Book>();
        }
    }
}