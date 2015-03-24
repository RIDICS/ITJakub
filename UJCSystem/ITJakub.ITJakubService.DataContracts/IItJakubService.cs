using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
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
        Task<string> GetBookPageByNameAsync(string documentId, string pageName, string resultFormat);

        [OperationContract]
        Task<string> GetBookPagesByNameAsync(string documentId, string startPageName, string endPageName, string resultFormat);

        [OperationContract]
        Task<string> GetBookPageByPositionAsync(string documentId, int position, string resultFormat);

        [OperationContract]
        Task<IList<BookPageContract>> GetBookPageListAsync(string documentId);

        #region Resource Import
        [OperationContract]       
        void AddResource(UploadResourceContract uploadFileInfoSkeleton);

        [OperationContract]
        bool ProcessSession(string resourceSessionId, string uploadMessage);
        #endregion

        [OperationContract]
        List<SearchResultContract> Search(string term);

        [OperationContract]
        BookInfoContract GetBookInfo(string bookId);

        [OperationContract]
        BookTypeSearchResultContract GetBooksWithCategoriesByBookType(BookTypeEnumContract bookType);
    }
}