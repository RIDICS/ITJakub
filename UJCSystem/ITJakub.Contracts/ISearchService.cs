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
        string GetTitleById(string id);

        [OperationContract]
        SearchTermPossibleResult AllExtendedTermsForKey(string key);

        [OperationContract]
        SearchTermPossibleResult AllExtendedTermsForKeyWithBooksRestriction(string key, List<string> booksIds);

        [OperationContract]
        List<SearchResultWithKwicContext> GetKwicContextForKeyWord(string keyWord);

        [OperationContract]
        List<SearchResultWithXmlContext> GetXmlContextForKeyWord(string keyWord);

        [OperationContract]
        List<SearchResultWithHtmlContext> GetHtmlContextForKeyWord(string keyWord);

        [OperationContract]
        List<SearchResultWithHtmlContext> GetHtmlContextForKeyWordWithBooksRestriction(string keyWord, List<string> bookIds);
    }
}
