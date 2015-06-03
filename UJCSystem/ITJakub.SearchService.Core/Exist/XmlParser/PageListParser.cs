using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using ITJakub.Shared.Contracts;

namespace ITJakub.SearchService.Core.Exist.XmlParser
{
    public class PageListParser : IXmlParser <List<BookPageContract>>
    {
        public List<BookPageContract> Parse(Stream dataStream)
        {
            var xmlDoc = XDocument.Load(dataStream);
            IEnumerable<XElement> pageBreakElements = xmlDoc.Root.Elements().Where(element => element.Name.LocalName == "pb");
            var pageList = new List<BookPageContract>();
            int position = 0;

            foreach (XElement pageBreakElement in pageBreakElements)
            {
                ++position;
                pageList.Add(new BookPageContract { Position = position, Text = pageBreakElement.Attribute("n").Value });
            }

            return pageList;
        }
    }
}