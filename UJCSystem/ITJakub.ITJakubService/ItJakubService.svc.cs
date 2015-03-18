using System.Collections.Generic;
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

        public ProcessedFileInfoContract SaveUploadedFile(UploadResourceContract uploadResourceContract)
        {
            return m_serviceManager.SaveUploadedFile(uploadResourceContract);
        }

        public IEnumerable<AuthorDetailContract> GetAllAuthors()
        {
            return m_serviceManager.GetAllAuthors();
        }

        public int CreateAuthor(string name)
        {
            return m_serviceManager.CreateAuthor(name);
        }

        public BookInfoContract GetBookInfo(string bookId)
        {
            return m_serviceManager.GetBookInfo(bookId);
        }

        public async Task<string> GetBookPageByNameAsync(string documentId, string pageName, string resultFormat)
        {
            return await m_serviceManager.GetBookPageByNameAsync(documentId, pageName, resultFormat);
        }

        public async Task<string> GetBookPagesByNameAsync(string documentId, string startPageName, string endPageName, string resultFormat)
        {
            return await m_serviceManager.GetBookPagesByNameAsync(documentId, startPageName, endPageName, resultFormat);
        }

        public async Task<string> GetBookPageByPositionAsync(string documentId, int position, string resultFormat)
        {
            return await m_serviceManager.GetBookPageByPositionAsync(documentId, position, resultFormat);
        }

        public async Task<IList<BookPageContract>> GetBookPageListAsync(string documentId)
        {
            return await m_serviceManager.GetBookPageListAsync(documentId);
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
    }

    [ServiceContract]
    public interface IItJakubServiceLocal : IItJakubService
    {
    }
}