using System.Collections.Generic;

namespace ITJakub.SearchService.Core.Exist.DAOs
{
    public class BookDao : ExistDao
    {
        public BookDao(ExistConnectionSettingsSkeleton connectionSettings) : base(connectionSettings)
        {
        }

        public string GetPagesByName(string documentId, string start, string end)
        {
            var parameters = new Dictionary<string, object> {{"document", documentId}, {"start", start}, {"end", end}};
            return RunStoredQuery("get-pages.xquery", parameters);
        }

        public string GetPageByName(string documentId, string pageName)
        {
            var parameters = new Dictionary<string, object> { { "document", documentId }, { "start", pageName }};
            return RunStoredQuery("get-pages.xquery", parameters);
            
        }

        public string GetPageByPositionFromStart(string documentId, int pagePosition)
        {
            var parameters = new Dictionary<string, object> { { "document", documentId }, { "page", pagePosition } };
            return RunStoredQuery("get-pages.xquery", parameters);
        }

        public string GetBookPageList(string documentId)
        {
            var parameters = new Dictionary<string, object> { { "document", documentId } };
            return RunStoredQuery("get-page-list.xquery", parameters);
        }
    }
}