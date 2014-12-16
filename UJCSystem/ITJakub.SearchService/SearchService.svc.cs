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

        public string GetBookPageByPosition(string bookId, string versionId, int pagePosition, string transformationName)
        {
            return m_searchServiceManager.GetBookPageByPosition(bookId, versionId, pagePosition, transformationName);
        }

        public void UploadFile(FileUploadContract fileUploadContract)
        {
            m_searchServiceManager.UploadFile(fileUploadContract);
        }

        public string GetBookPageByName(string bookId, string versionId, string pageName, string transformationName)
        {
            return m_searchServiceManager.GetBookPageByName(bookId, versionId, pageName, transformationName);
        }

        public string GetBookPagesByName(string bookId, string versionId, string startPageName, string endPageName, string transformationName)
        {
            return m_searchServiceManager.GetBookPagesByName(bookId, versionId, startPageName, endPageName,transformationName);
        }

        public IList<BookPage> GetBookPageList(string bookId, string versionId)
        {
            return m_searchServiceManager.GetBookPageList(bookId, versionId);
        }
    }

    [ServiceContract]
    public interface ISearchServiceLocal : ISearchService
    {
    }
}