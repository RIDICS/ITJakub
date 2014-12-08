using System.Collections.Generic;
using System.ServiceModel;
using ITJakub.Shared.Contracts;

namespace ITJakub.SearchService
{
    public class SearchService : ISearchServiceLocal
    {
        private readonly SearchServiceManager m_searchServiceManager;

        public SearchService()
        {
            m_searchServiceManager = Container.Current.Resolve<SearchServiceManager>();
        }

        public string GetBookPageByPosition(string documentId, int pagePosition, string transformationName)
        {
            return m_searchServiceManager.GetBookPageByPosition(documentId, pagePosition, transformationName);
        }

        public string GetBookPageByName(string documentId, string pageName, string transformationName)
        {
            return m_searchServiceManager.GetBookPageByName(documentId, pageName, transformationName);
        }

        public string GetBookPagesByName(string documentId, string startPageName, string endPageName, string transformationName)
        {
            return m_searchServiceManager.GetBookPagesByName(documentId, startPageName, endPageName, transformationName);
        }

        public IList<BookPage> GetBookPageList(string documentId)
        {
            return m_searchServiceManager.GetBookPageList(documentId);
        }

        public void Test()
        {
            m_searchServiceManager.Test();
        }
    }

    [ServiceContract]
    public interface ISearchServiceLocal : ISearchService
    {
    }
}