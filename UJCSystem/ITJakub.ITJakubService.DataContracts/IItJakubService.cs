using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using ITJakub.Shared.Contracts;
using ITJakub.Shared.Contracts.Resources;

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
        IEnumerable<AuthorDetailContract> GetAllAuthors();

        [OperationContract]
        string GetBookPageByName(string documentId, string pageName, string resultFormat);

        [OperationContract]
        string GetBookPagesByName(string documentId, string startPageName, string endPageName, string resultFormat);

        [OperationContract]
        string GetBookPageByPosition(string documentId, int position, string resultFormat);

        [OperationContract]
        IList<BookPage> GetBookPageList(string documentId);

        #region Resource Import
        [OperationContract]       
        void AddResource(UploadResourceContract uploadFileInfoSkeleton);

        [OperationContract]
        bool ProcessSession(string resourceSessionId, string uploadMessage);
        #endregion
    }
}