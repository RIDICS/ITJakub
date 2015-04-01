using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using System.Threading.Tasks;
using Castle.Windsor;
using ITJakub.ITJakubService.DataContracts;
using ITJakub.Shared.Contracts;
using ITJakub.Shared.Contracts.Resources;

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
       
        public CreateUserResultContract CreateUser(CreateUserContract createUserContract)
        {
            return m_serviceManager.CreateUser(createUserContract);
        }

        public LoginUserResultContract LoginUser(LoginUserContract loginUserContract)
        {
            return m_serviceManager.LoginUser(loginUserContract);
        }

        public IEnumerable<AuthorDetailContract> GetAllAuthors()
        {
            return m_serviceManager.GetAllAuthors();
        }

        public int CreateAuthor(string name)
        {
            return m_serviceManager.CreateAuthor(name);
        }

        public BookInfoContract GetBookInfo(string bookGuid)
        {
            return m_serviceManager.GetBookInfo(bookGuid);
        }

        public BookTypeSearchResultContract GetBooksWithCategoriesByBookType(BookTypeEnumContract bookType)
        {
            return m_serviceManager.GetBooksWithCategoriesByBookType(bookType);
        }

        public async Task<string> GetBookPageByNameAsync(string bookGuid, string pageName, string resultFormat)
        {
            return await m_serviceManager.GetBookPageByNameAsync(bookGuid, pageName, resultFormat);
        }

        public async Task<string> GetBookPagesByNameAsync(string bookGuid, string startPageName, string endPageName, string resultFormat)
        {
            return await m_serviceManager.GetBookPagesByNameAsync(bookGuid, startPageName, endPageName, resultFormat);
        }

        public async Task<string> GetBookPageByPositionAsync(string bookGuid, int position, string resultFormat)
        {
            return await m_serviceManager.GetBookPageByPositionAsync(bookGuid, position, resultFormat);
        }

        public async Task<IList<BookPageContract>> GetBookPageListAsync(string bookGuid)
        {
            return await m_serviceManager.GetBookPageListAsync(bookGuid);
        }

        public void AddResource(UploadResourceContract resourceInfoSkeleton)
        {
            m_serviceManager.AddResource(resourceInfoSkeleton);
        }

        public bool ProcessSession(string resourceSessionId, string uploadMessage)
        {
            return m_serviceManager.ProcessSession(resourceSessionId, uploadMessage);
        }

        public List<SearchResultContract> Search(string term)
        {
            return m_serviceManager.Search(term);
        }

        public Stream GetBookPageImage(BookPageImageContract bookPageImageContract)
        {
            return m_serviceManager.GetBookPageImage(bookPageImageContract);
        }
    }

    [ServiceContract]
    public interface IItJakubServiceLocal : IItJakubService
    {
    }
}