using System.Collections.Generic;
using System.ServiceModel;
using ITJakub.Contracts.Searching;

namespace ITJakub.Contracts
{
    [ServiceContract]
    public interface ISearchService
    {
        [OperationContract]
        void Search(List<SearchCriteriumBase> criteria);

        [OperationContract]
        SearchResult[] GetContextForKeyWord(string keyWord);

        [OperationContract]
        string GetTitleById(string id);

        [OperationContract]
        List<string> AllExtendedTermsForKey(string key);

        [OperationContract]
        List<string> AllExtendedTermsForKeyWithBooksRestriction(string key, List<string> booksIds);
    }
}
