using System.Collections.Generic;
using System.ServiceModel;
using ITJakub.Contracts.Categories;
using ITJakub.Contracts.Searching;

namespace ITJakub.Contracts
{
    [ServiceContract]
    public interface IItJakubService
    {
        [OperationContract]
        KeyWordsResponse GetAllExtendedTermsForKey(string key, List<string> categorieIds, List<string> booksIds);

        [OperationContract]
        List<SearchResultWithKwicContext> GetContextForKeyWord(string keyWord);

        [OperationContract]
        SearchResult[] GetResultsByBooks(string book, string keyWord);

        [OperationContract]
        SelectionBase[] GetCategoryChildrenById(string categoryId);

        [OperationContract]
        SelectionBase[] GetRootCategories();
    }
}
