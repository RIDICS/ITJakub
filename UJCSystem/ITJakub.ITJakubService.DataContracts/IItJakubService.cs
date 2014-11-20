using System.Collections.Generic;
using System.ServiceModel;
using ITJakub.Shared.Contracts;

namespace ITJakub.ITJakubService.DataContracts
{
    [ServiceContract]
    public interface IItJakubService
    {
        
        [OperationContract]
        void CreateUser(AuthProvidersContract providerContract, string providerToken, UserDetailContract userDetail);

        [OperationContract]
        void LoginUser(AuthProvidersContract providerContract, string providerToken, string email);

        [OperationContract]
        ProcessedFileInfoContract ProcessUploadedFile(UploadFileContract dataStream);

        [OperationContract]
        void SaveFrontImageForFile(UploadImageContract uploadImageContract);

        [OperationContract]
        void SavePageImageForFile(UploadImageContract uploadImageContract);

        [OperationContract]
        void SaveFileMetadata(string fileGuid, string name, string author);

        [OperationContract]
        IEnumerable<AuthorDetailContract> GetAllAuthors();

        [OperationContract]
        int CreateAuthor(string name);

        [OperationContract]
        void AssignAuthorsToBook(string bookGuid, string bookVersionGuid, IEnumerable<int> authorIds);

        [OperationContract]
        string GetBookPageByName(string documentId, string pageName);

        [OperationContract]
        string GetBookPagesByName(string documentId, string startPageName, string endPageName);

        [OperationContract]
        string GetBookPageByPosition(string documentId, int position);

        [OperationContract]
        IList<BookPage> GetBookPageList(string documentId);
    }
}