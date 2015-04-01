using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ITJakub.ITJakubService.Core;
using ITJakub.ITJakubService.Core.Resources;
using ITJakub.ITJakubService.DataContracts;
using ITJakub.Shared.Contracts;
using ITJakub.Shared.Contracts.Resources;

namespace ITJakub.ITJakubService
{
    public class ItJakubServiceManager : IItJakubServiceLocal
    {
        private readonly UserManager m_userManager;
        private readonly BookManager m_bookManager;
        private readonly AuthorManager m_authorManager;
        private readonly ResourceManager m_resourceManager;
        private readonly SearchManager m_searchManager;

        public ItJakubServiceManager(UserManager userManager, BookManager bookManager, AuthorManager authorManager, ResourceManager resourceManager, SearchManager searchManager)
        {
            m_userManager = userManager;
            m_bookManager = bookManager;
            m_authorManager = authorManager;
            m_resourceManager = resourceManager;
            m_searchManager = searchManager;
        }

        public CreateUserResultContract CreateUser(CreateUserContract createUserContract)
        {
            return m_userManager.CreateUser(createUserContract);
        }

        public LoginUserResultContract LoginUser(LoginUserContract loginUserContract)
        {
            return m_userManager.LoginUser(loginUserContract);
        }

        public IEnumerable<AuthorDetailContract> GetAllAuthors()
        {
            return m_authorManager.GetAllAuthors();
        }

        public int CreateAuthor(string name)
        {
            return m_authorManager.CreateAuthor(name);
        }

        public BookInfoContract GetBookInfo(string bookGuid)
        {
            return m_bookManager.GetBookInfo(bookGuid);
        }

        public BookTypeSearchResultContract GetBooksWithCategoriesByBookType(BookTypeEnumContract bookType)
        {
            return m_searchManager.GetBooksWithCategoriesByBookType(bookType);
        }

        public async Task<string> GetBookPageByNameAsync(string bookGuid, string pageName, string resultFormat)
        {
            return await m_bookManager.GetBookPageByNameAsync(bookGuid, pageName, resultFormat);
        }

        public async Task<string> GetBookPagesByNameAsync(string bookGuid, string startPageName, string endPageName, string resultFormat)
        {
            return await m_bookManager.GetBookPagesByNameAsync(bookGuid, startPageName, endPageName, resultFormat);
        }

        public async Task<string> GetBookPageByPositionAsync(string bookGuid, int position, string resultFormat)
        {
            return await m_bookManager.GetBookPagesByPositionAsync(bookGuid, position, resultFormat);
        }

        public async Task<IList<BookPageContract>> GetBookPageListAsync(string bookGuid)
        {
            return await m_bookManager.GetBookPagesListAsync(bookGuid);
        }

        public void AddResource(UploadResourceContract resourceInfoSkeleton)
        {
            m_resourceManager.AddResource(resourceInfoSkeleton);
        }

        public bool ProcessSession(string resourceSessionId, string uploadMessage)
        {
            return m_resourceManager.ProcessSession(resourceSessionId, uploadMessage);
        }

        public List<SearchResultContract> Search(string term)
        {
            return m_searchManager.Search(term);
        }

        public Stream GetBookPageImage(BookPageImageContract bookPageImageContract)
        {
            return m_bookManager.GetBookPageImage(bookPageImageContract);
        }

    }
}