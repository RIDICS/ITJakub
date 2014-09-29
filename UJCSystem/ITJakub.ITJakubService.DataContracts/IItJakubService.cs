using System.Collections.Generic;
using System.ServiceModel;
using ITJakub.Contracts.Categories;
using ITJakub.Contracts.Searching;

namespace ITJakub.ITJakubService.DataContracts
{
    [ServiceContract]
    public interface IItJakubService
    {
        #region Prototype1 Operations

        [OperationContract]
        KeyWordsResponse GetAllExtendedTermsForKey(string key, List<string> categorieIds, List<string> booksIds);

        [OperationContract]
        List<SearchResultWithHtmlContext> GetHtmlContextForKeyWord(string keyWord, List<string> categorieIds,
            List<string> booksIds);

        [OperationContract]
        List<SearchResultWithHtmlContext> GetResultsByBooks(string book, string keyWord);

        [OperationContract]
        List<SelectionBase> GetCategoryChildrenById(string categoryId);

        [OperationContract]
        List<SelectionBase> GetRootCategories();

        [OperationContract]
        IEnumerable<SearchResult> GetBooksBySearchTerm(string searchTerm);

        [OperationContract]
        IEnumerable<SearchResult> GetBooksTitleByLetter(string letter);

        [OperationContract]
        IEnumerable<SearchResult> GetSourcesAuthorByLetter(string letter);

        [OperationContract]
        string GetContentByBookId(string id);

        [OperationContract]
        SearchResult GetBookById(string id);

        #endregion

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
        int CreateAuthor(IEnumerable<AuthorInfoContract> authorInfos);

        [OperationContract]
        void AssignAuthorsToBook(string bookGuid, string bookVersionGuid, IEnumerable<int> authorIds);
    }
}