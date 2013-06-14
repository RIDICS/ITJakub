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

        public KeyWordsResponse GetAllExtendedTermsForKey(string key, List<string> categorieIds, List<string> booksIds)
        {
            return m_serviceManager.GetAllExtendedTermsForKey(key, categorieIds, booksIds);
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

        public SelectionBase[] GetRootCategories()
        {
            return m_serviceManager.GetRootCategories();
        }
    }

    [ServiceContract]
    public interface IItJakubServiceLocal:IItJakubService
    {
    }
}
