using System.Collections.Generic;
using ITJakub.ITJakubService.Core;
using ITJakub.ITJakubService.DataContracts;
using ITJakub.Shared.Contracts;

namespace ITJakub.ITJakubService
{
    public class ItJakubServiceManager
    {
        private readonly AccountManager m_accountManager;
        private readonly UploadManager m_uploadManager;
        private readonly BookManager m_bookManager;
        private readonly AuthorManager m_authorManager;

        public ItJakubServiceManager(AccountManager accountManager, UploadManager uploadManager, BookManager bookManager, AuthorManager authorManager)
        {
            m_accountManager = accountManager;
            m_uploadManager = uploadManager;
            m_bookManager = bookManager;
            m_authorManager = authorManager;
        }

        public void CreateUser(AuthProvidersContract providerContract, string providerToken, UserDetailContract userDetail)
        {
            m_accountManager.CreateUser(providerContract, providerToken, userDetail);
        }

        public void LoginUser(AuthProvidersContract providerContract, string providerToken, string email)
        {
            m_accountManager.LoiginUser(providerContract, providerToken, email);
        }

        public ProcessedFileInfoContract ProcessUploadedFile(UploadFileContract uploadFileContract)
        {
            return m_uploadManager.ProcessUploadedFile(uploadFileContract);
        }

        public void SaveFrontImageForFile(UploadImageContract uploadImageContract)
        {
            m_uploadManager.SaveFrontImageForFile(uploadImageContract);
        }

        public void SavePageImageForFile(UploadImageContract uploadImageContract)
        {
            m_uploadManager.SavePageImageForFile(uploadImageContract);
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
    }
}