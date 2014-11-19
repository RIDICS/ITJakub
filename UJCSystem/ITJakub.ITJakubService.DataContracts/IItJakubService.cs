using System.Collections.Generic;
using System.ServiceModel;
using ITJakub.Shared.Contracts.Categories;
using ITJakub.Shared.Contracts.Searching;

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
    }
}