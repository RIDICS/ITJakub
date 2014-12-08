using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using ITJakub.Shared.Contracts;

namespace ITJakub.SearchService.Core.Exist.DAOs
{
    public class BookDao
    {
        private readonly ExistManager m_existManager;

        public BookDao(ExistManager existManager)
        {
            m_existManager = existManager;
        }

        public string GetPagesByName(string documentId, string start, string end, string transformationName)
        {
            return m_existManager.GetPagesByName(documentId, start, end);
        }

        public string GetPageByName(string documentId, string pageName, string transformationName)
        {
            return m_existManager.GetPageByName(documentId, pageName);
            
        }

        public string GetPageByPositionFromStart(string documentId, int pagePosition, string transformationName)
        {
            return m_existManager.GetPageByPositionFromStart(documentId, pagePosition);
        }

        public IList<BookPage> GetBookPageList(string documentId)
        {
            string pagesXmlAsString = m_existManager.GetPageList(documentId);
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

        public void Test()
        {
            var res = m_existManager.GetPageList("{8688926F-9106-4A70-9440-673779415D07}");

        }
    }
}