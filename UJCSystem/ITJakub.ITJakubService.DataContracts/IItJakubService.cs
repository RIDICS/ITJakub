using System.Collections.Generic;
using System.ServiceModel;
using ITJakub.Shared.Contracts;

namespace ITJakub.ITJakubService.DataContracts
{
    [ServiceContract]
    public interface IItJakubService
    {
        
        [OperationContract]
        CreateUserResultContract CreateUser(CreateUserContract createUserContract);

        [OperationContract]
        LoginUserResultContract LoginUser(LoginUserContract loginUserContract);

        [OperationContract]
        ProcessedFileInfoContract SaveUploadedFile(UploadFileContract dataStream);

        [OperationContract]
        void SaveFileMetadata(string fileGuid, string name, string author);

        [OperationContract]
        IEnumerable<AuthorDetailContract> GetAllAuthors();

        [OperationContract]
        int CreateAuthor(string name);

        [OperationContract]
        void AssignAuthorsToBook(string bookGuid, string bookVersionGuid, IEnumerable<int> authorIds);

        [OperationContract]
        string GetBookPageByName(string documentId, string pageName, string resultFormat);

        [OperationContract]
        string GetBookPagesByName(string documentId, string startPageName, string endPageName, string resultFormat);

        [OperationContract]
        string GetBookPageByPosition(string documentId, int position, string resultFormat);

        [OperationContract]
        IList<BookPage> GetBookPageList(string documentId);
    }
}