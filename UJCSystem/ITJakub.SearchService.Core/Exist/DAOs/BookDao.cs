using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using ITJakub.Shared.Contracts;

namespace ITJakub.SearchService.Core.Exist.DAOs
{
    public class BookDao : ExistDao
    {
        public BookDao(ExistConnectionSettingsSkeleton connectionSettings) : base(connectionSettings)
        {
        }

        public string GetPagesByName(string documentId, string start, string end, string transformationName)
        {
            var parameters = new Dictionary<string, object> {{"document", documentId}, {"start", start}, {"end", end}};
            return RunStoredQuery("get-pages.xquery", transformationName, parameters);
        }

        public string GetPageByName(string documentId, string pageName, string transformationName)
        {
            var parameters = new Dictionary<string, object> { { "document", documentId }, { "start", pageName }};
            return RunStoredQuery("get-pages.xquery", transformationName, parameters);
            
        }

        public string GetPageByPositionFromStart(string documentId, int pagePosition, string transformationName)
        {
            var parameters = new Dictionary<string, object> { { "document", documentId }, { "page", pagePosition } };
            return RunStoredQuery("get-pages.xquery", transformationName, parameters);
        }

        public IList<BookPage> GetBookPageList(string documentId)
        {
            var parameters = new Dictionary<string, object> { { "document", documentId } };
            string pagesXmlAsString = RunStoredQuery("get-page-list.xquery", parameters);
            XDocument xmlDoc = XDocument.Parse(pagesXmlAsString);
            IEnumerable<XElement> pageBreakElements = xmlDoc.Root.Elements().Where<XElement>(element => element.Name.LocalName == "pb");
            var pageList = new List<BookPage>();
            int position = 0;
            foreach (XElement pageBreakElement in pageBreakElements)
            {
                ++position;
                pageList.Add(new BookPage { Position = position, Text = pageBreakElement.Attribute("n").Value });
            }

            return pageList;
        }
    }
}