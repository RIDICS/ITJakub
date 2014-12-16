using System.Collections.Generic;
using System.ServiceModel;
using ITJakub.Shared.Contracts;

namespace ITJakub.Core.SearchService
{
    public class SearchServiceClient : ClientBase<ISearchService>, ISearchService
    {
        public string GetBookPageByPosition(string bookId,string versionId, int pagePosition, string transformationName)
        {
            return Channel.GetBookPageByPosition(bookId, versionId, pagePosition, transformationName);
        }

        public string GetBookPageByName(string bookId, string versionId, string pageName, string transformationName)
        {
            return Channel.GetBookPageByName(bookId, versionId, pageName, transformationName);
        }

        public string GetBookPagesByName(string bookId, string versionId, string startPageName, string endPageName, string transformationName)
        {
            return Channel.GetBookPagesByName(bookId, versionId, startPageName, endPageName, transformationName);
        }

        public IList<BookPage> GetBookPageList(string bookId,string versionId)
        {
            return Channel.GetBookPageList(bookId, versionId);
        }

        public void UploadFile(FileUploadContract fileUploadContract)
        {
            Channel.UploadFile(fileUploadContract);
        }
    }
}
