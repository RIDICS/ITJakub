using System.Collections.Generic;
using System.ServiceModel;
using Castle.Windsor;
using ITJakub.Contracts;
using ITJakub.Contracts.Categories;
using ITJakub.Contracts.Searching;
using ITJakub.Core;

namespace ITJakub.ITJakubService
{
    public class ItJakubService : IItJakubServiceLocal
    {

        private readonly WindsorContainer m_container;
        private readonly ItJakubServiceManager m_serviceManager;

        public ItJakubService()
        {
            m_container = Container.Current;
            m_serviceManager = m_container.Resolve<ItJakubServiceManager>();
        }

        public List<string> GetAllExtendedTermsForKey(string key)
        {
            return m_serviceManager.GetAllExtendedTermsForKey(key);
        }

        public SearchResult[] GetContextForKeyWord(string keyWord)
        {
            return m_serviceManager.GetContextForKeyWord(keyWord);
        }

        public SearchResult[] GetResultsByBooks(string book, string keyWord)
        {
            //TODO return m_serviceManager.GetResultsFromBook(book, keyWord);
            return null;
        }

        public SelectionBase[] GetCategoryChildrenById(string categoryId)
        {
            return m_serviceManager.GetCategoryChildrenById(categoryId);
        }
    }

    [ServiceContract]
    public interface IItJakubServiceLocal:IItJakubService
    {
    }
}
