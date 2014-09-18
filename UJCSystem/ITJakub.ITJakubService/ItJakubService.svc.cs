using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using Castle.Windsor;
using ITJakub.Contracts;
using ITJakub.Contracts.Categories;
using ITJakub.Contracts.Searching;
using ITJakub.Core;
using ITJakub.ITJakubService.DataContracts;

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


        public List<SearchResultWithHtmlContext> GetHtmlContextForKeyWord(string keyWord, List<string> categorieIds, List<string> booksIds)
        {
            return m_serviceManager.GetContextForKeyWord(keyWord, categorieIds, booksIds);
        }

        public List<SearchResultWithHtmlContext> GetResultsByBooks(string book, string keyWord)
        {
            //TODO return m_serviceManager.GetResultsFromBook(book, keyWord);
            return null;
        }

        public List<SelectionBase> GetCategoryChildrenById(string categoryId)
        {
            return m_serviceManager.GetCategoryChildrenById(categoryId);
        }

        public List<SelectionBase> GetRootCategories()
        {
            return m_serviceManager.GetRootCategories();
        }

        public IEnumerable<SearchResult> GetBooksBySearchTerm(string searchTerm)
        {
            return m_serviceManager.GetBookBySearchTerm(searchTerm);
        }

        public IEnumerable<SearchResult> GetBooksTitleByLetter(string letter)
        {
            return m_serviceManager.GetBooksTitleByLetter(letter);
        }

        public IEnumerable<SearchResult> GetSourcesAuthorByLetter(string letter)
        {
            return m_serviceManager.GetSourcesAuthorByLetter(letter);
        }

        public string GetContentByBookId(string id)
        {
            return m_serviceManager.GetContentByBookId(id);
        }

        public SearchResult GetBookById(string id)
        {
            return m_serviceManager.GetBookById(id);
        }

        public void CreateUser(AuthProvidersContract providerContract, string providerToken, UserDetailContract userDetail)
        {
            m_serviceManager.CreateUser(providerContract, providerToken, userDetail);
        }

        public void LoginUser(AuthProvidersContract providerContract, string providerToken, string email)
        {
            m_serviceManager.LoginUser(providerContract, providerToken, email);
        }

        public ProcessedFileInfoContract ProcessUploadedFile(string filename, Stream dataStream)
        {
            return m_serviceManager.ProcessUploadedFile(filename, dataStream);
        }

        public void SaveFrontImageForFile(string fileGuid, Stream dataStream)
        {
            m_serviceManager.SaveFrontImageForFile(fileGuid, dataStream);
        }

        public void SaveImagesForFile(string fileGuid, Stream dataStream)
        {
            m_serviceManager.SaveImagesForFile(fileGuid, dataStream);
        }
    }

    [ServiceContract]
    public interface IItJakubServiceLocal:IItJakubService
    {
    }
}
