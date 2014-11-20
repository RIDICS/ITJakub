using System.Collections.Generic;
using System.ServiceModel;
using ITJakub.Shared.Contracts;

namespace ITJakub.ITJakubService.Core.SearchService
{
    public class SearchServiceClient : ClientBase<ISearchService>, ISearchService
    {
        public string GetBookPageByPosition(string documentId, int pagePosition)
        {
            return Channel.GetBookPageByPosition(documentId, pagePosition);
        }

        public string GetBookPageByName(string documentId, string pageName)
        {
            return Channel.GetBookPageByName(documentId, pageName);
        }

        public string GetBookPagesByName(string documentId, string startPageName, string endPageName)
        {
            return Channel.GetBookPagesByName(documentId, startPageName, endPageName);
        }

        public IList<BookPage> GetBookPageList(string documentId)
        {
            return Channel.GetBookPageList(documentId);
        }
    }
}
