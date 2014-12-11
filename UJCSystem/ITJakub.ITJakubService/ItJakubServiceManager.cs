using System.Collections.Generic;
using System.IO;
using ITJakub.ITJakubService.Core;
using ITJakub.ITJakubService.Core.Resources;
using ITJakub.ITJakubService.DataContracts;
using ITJakub.Shared.Contracts;

namespace ITJakub.ITJakubService
{
    public class ItJakubServiceManager : IItJakubServiceLocal
    {
        private readonly UserManager m_userManager;
        private readonly UploadManager m_uploadManager;
        private readonly BookManager m_bookManager;
        private readonly AuthorManager m_authorManager;
        private readonly ResourceManager m_resourceManager;

        public ItJakubServiceManager(UserManager userManager, UploadManager uploadManager, BookManager bookManager, AuthorManager authorManager, ResourceManager resourceManager)
        {
            m_userManager = userManager;
            m_uploadManager = uploadManager;
            m_bookManager = bookManager;
            m_authorManager = authorManager;
            m_resourceManager = resourceManager;
        }

        public CreateUserResultContract CreateUser(CreateUserContract createUserContract)
        {
            return m_userManager.CreateUser(createUserContract);
        }

        public LoginUserResultContract LoginUser(LoginUserContract loginUserContract)
        {
            return m_userManager.LoginUser(loginUserContract);
        }

        public ProcessedFileInfoContract SaveUploadedFile(UploadFileContract uploadFileContract)
        {
            return m_uploadManager.ProcessUploadedFile(uploadFileContract);
        }

        public void SaveFileMetadata(string fileGuid, string name, string author)
        {
            m_bookManager.CreateBook(fileGuid, name, author);
        }

        public IEnumerable<AuthorDetailContract> GetAllAuthors()
        {
            return m_authorManager.GetAllAuthors();
        }

        public int CreateAuthor(string name)
        {
            return m_authorManager.CreateAuthor(name);
        }

        public void AssignAuthorsToBook(string bookGuid, string bookVersionGuid, IEnumerable<int> authorIds)
        {
            m_bookManager.AssignAuthorsToBook(bookGuid, bookVersionGuid, authorIds);
        }

        public string GetBookPageByName(string documentId, string pageName, string resultFormat)
        {
            return m_bookManager.GetBookPageByName(documentId, pageName, resultFormat);
        }

        public string GetBookPagesByName(string documentId, string startPageName, string endPageName, string resultFormat)
        {
            return m_bookManager.GetBookPagesByName(documentId, startPageName, endPageName, resultFormat);
        }

        public string GetBookPageByPosition(string documentId, int position, string resultFormat)
        {
            return m_bookManager.GetBoookPagesByPosition(documentId, position, resultFormat);
        }

        public IList<BookPage> GetBookPageList(string documentId)
        {
            return m_bookManager.GetBookPagesList(documentId);
        }

        public void AddResource(string resourceSessionId, string fileName, Stream dataStream)
        {
            m_resourceManager.AddResource(resourceSessionId, fileName, dataStream);
        }

        public bool ProcessSession(string resourceSessionId)
        {
            return m_resourceManager.ProcessSession(resourceSessionId);
        }
    }
}