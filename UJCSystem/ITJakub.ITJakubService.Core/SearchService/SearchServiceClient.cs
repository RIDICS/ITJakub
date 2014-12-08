using System.Collections.Generic;
using System.ServiceModel;
using ITJakub.Shared.Contracts;

namespace ITJakub.ITJakubService.Core.SearchService
{
    public class SearchServiceClient : ClientBase<ISearchService>, ISearchService
    {
        public string GetBookPageByPosition(string documentId, int pagePosition, string transformationName)
        {
            return Channel.GetBookPageByPosition(documentId, pagePosition, transformationName);
        }

        public string GetBookPageByName(string documentId, string pageName, string transformationName)
        {
            return Channel.GetBookPageByName(documentId, pageName,transformationName);
        }

        public string GetBookPagesByName(string documentId, string startPageName, string endPageName, string transformationName)
        {
            return Channel.GetBookPagesByName(documentId, startPageName, endPageName,transformationName);
        }

        public IList<BookPage> GetBookPageList(string documentId)
        {
            return Channel.GetBookPageList(documentId);
        }

        public void Test()
        {
            Channel.Test();
        }
    }
}
