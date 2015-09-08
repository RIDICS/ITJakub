using System;
using System.IO;
using System.Net.Http;
using System.ServiceModel;
using ITJakub.SearchService.Core.Exist.Attributes;
using ITJakub.Shared.Contracts;

namespace ITJakub.SearchService.Core.Exist
{
    [ServiceContract]
    public interface IExistClient
    {
        [OperationContract]
        [ExistQuery(Method = HttpMethodType.Get, XqueryName = "get-pages.xquery")]
        string GetPageByPositionFromStart(string bookId, string versionId, int page, string outputFormat);

        [OperationContract]
        [ExistQuery(Method = HttpMethodType.Get, XqueryName = "get-pages.xquery")]
        string GetPageByName(string bookId, string versionId, string start, string outputFormat);

        [OperationContract]
        [ExistQuery(Method = HttpMethodType.Get, XqueryName = "get-pages.xquery")]
        string GetPageByXmlId(string bookId, string versionId, string pageXmlId, string outputFormat);

        [OperationContract]
        [ExistQuery(Method = HttpMethodType.Get, XqueryName = "get-pages.xquery")]
        string GetPagesByName(string bookId, string versionId, string start, string end, string outputFormat);

        [OperationContract]
        [ExistResource(Method = HttpMethodType.Put, Type = ResourceLevelEnumContract.Version)]
        void UploadVersionFile(string bookId, string versionId, string fileName, Stream dataStream);

        [OperationContract]
        [ExistResource(Method = HttpMethodType.Put, Type = ResourceLevelEnumContract.Book)]
        void UploadBookFile(string bookId, string fileName, Stream dataStream);

        [OperationContract]
        [ExistResource(Method = HttpMethodType.Put, Type = ResourceLevelEnumContract.Shared)]
        void UploadSharedFile(string fileName, Stream dataStream);

        [OperationContract]
        [ExistResource(Method = "PUT", Type = ResourceLevelEnumContract.Bibliography)]
        void UploadBibliographyFile(string bookId, string bookVersionId, string fileName, Stream dataStream);

        [OperationContract]        
        [ExistQuery(Method = HttpMethodType.Get, XqueryName = "get-entry.xquery")]        string GetDictionaryEntryByXmlId(string bookId, string versionId, string xmlEntryId, string outputFormat);

        [OperationContract]
        [ExistQuery(Method = HttpMethodType.Get, XqueryName = "get-entry-from-search.xquery")]
        string GetDictionaryEntryFromSearch(string serializedSearchCriteria, string bookId, string versionId,
            string xmlEntryId, string outputFormat);

        [OperationContract]
        [ExistQuery(Method = HttpMethodType.Get, XqueryName = "list-search-editions.xquery")]
        string ListSearchEditionsResults(string serializedSearchCriteria);

        [OperationContract]
        [ExistQuery(Method = HttpMethodType.Get, XqueryName = "list-search-dictionaries.xquery")]
        string ListSearchDictionariesResults(string serializedSearchCriteria);

        [OperationContract]
        [ExistQuery(Method = HttpMethodType.Get, XqueryName = "get-search-dictionaries-count.xquery")]
        int ListSearchDictionariesResultsCount(string serializedSearchCriteria);

        [OperationContract]
        [ExistQuery(Method = HttpMethodType.Get, XqueryName = "get-search-editions-count.xquery")]
        int GetSearchCriteriaResultsCount(string serializedSearchCriteria);


        [OperationContract]
        [ExistQuery(Method = HttpMethodType.Get, XqueryName = "get-search-edition-page-list.xquery")]
        string GetSearchEditionsPageList(string serializedSearchCriteria);

        [OperationContract]
        [ExistQuery(Method = HttpMethodType.Get, XqueryName = "get-page-from-search.xquery")]
        string GetEditionPageFromSearch(string serializedSearchCriteria, string bookId, string versionId, string pageXmlId, string outputFormat);

        [OperationContract]
        [ExistQuery(Method = HttpMethodType.Get, XqueryName = "get-search-corpus.xquery")]
        string GetSearchCorpus(string serializedSearchCriteria);

        [OperationContract]
        [ExistQuery(Method = HttpMethodType.Get, XqueryName = "get-search-corpus-count.xquery")]
        int GetSearchCorpusCount(string serializedSearchCriteria);

    }
}