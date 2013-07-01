using System.Collections.Generic;
using ITJakub.Contracts.Categories;
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

        public IEnumerable<Book> GetSearchResult()
        {
            return new List<Book>();
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