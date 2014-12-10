using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using ITJakub.Shared.Contracts;

namespace ITJakub.SearchService.Core.Exist
{
    public class ExistManager
    {
        private readonly ExistClient m_client;

        public ExistManager(ExistClient existClient)
        {
            m_client = existClient;
        }

        public List<BookPage> GetPageList(string documentId, string xslPath = null)
        {
            XDocument xmlDoc;
            using (Stream pageStream = m_client.GetPageList(documentId, xslPath))
            {
                xmlDoc = XDocument.Load(pageStream);
            }
            IEnumerable<XElement> pageBreakElements = xmlDoc.Root.Elements().Where(element => element.Name.LocalName == "pb");
            var pageList = new List<BookPage>();
            int position = 0;
            foreach (XElement pageBreakElement in pageBreakElements)
            {
                ++position;
                pageList.Add(new BookPage {Position = position, Text = pageBreakElement.Attribute("n").Value});
            }

            return pageList;
        }

        public void UploadFile(string bookId, string bookVersionid, string fileName, Stream filStream)
        {
            m_client.UploadFile(bookId, bookVersionid, fileName, filStream);
        }

        public string GetPageByPositionFromStart(string documentId, int pagePosition)
        {
            return m_client.GetPageByPositionFromStart(documentId, pagePosition);
        }

        public string GetPageByName(string documentId, string pageName)
        {
            return m_client.GetPageByName(documentId, pageName);
        }

        public string GetPagesByName(string documentId, string start, string end)
        {
            return m_client.GetPagesByName(documentId, start, end);
        }
    }
}