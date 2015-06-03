using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using ITJakub.Shared.Contracts;

namespace ITJakub.SearchService.Core.Exist.XmlParser
{
    public class BookContentParser : IXmlParser <List<BookContentItemContract>>
    {
        public List<BookContentItemContract> Parse(Stream dataStream)
        {
            var xmlDoc = XDocument.Load(dataStream);
            var itemElements = xmlDoc.Root.Element("list").Elements().Where(element => element.Name.LocalName == "item");
            var contentItemList = new List<BookContentItemContract>();
            foreach (var contentItemElement in itemElements)
            {
                var referecceElemenet = contentItemElement.Element("ref");
                var headElemenet = contentItemElement.Element("head");
                if(referecceElemenet == null || headElemenet == null) continue;

                var bookContract = new BookContentItemContract
                {
                    Text = headElemenet.Value,
                    ReferredPageXmlId = referecceElemenet.Attribute("target").Value.Substring(1),
                    ReferredPageName = referecceElemenet.Value
                };

                ParseChilds(contentItemElement, bookContract);

                contentItemList.Add(bookContract);
            }

            return contentItemList;
        }

        private BookContentItemContract ParseChilds(XElement bookContentItemElement, BookContentItemContract bookContract)
        {
            var listElement = bookContentItemElement.Element("list");
            if (listElement != null)
            {
                var childItemsElements = listElement.Elements().Where(element => element.Name.LocalName == "item");

                foreach (var contentItemElement in childItemsElements)
                {
                    var referecceElemenet = contentItemElement.Element("ref");
                    var headElemenet = contentItemElement.Element("head");
                    if (referecceElemenet == null || headElemenet == null) continue;

                    var childBookContract = new BookContentItemContract
                    {
                        Text = headElemenet.Value,
                        ReferredPageXmlId = referecceElemenet.Attribute("target").Value.Substring(1),
                        ReferredPageName = referecceElemenet.Value
                    };

                    ParseChilds(contentItemElement, childBookContract);

                    if(bookContract.ChildContentItems == null) bookContract.ChildContentItems = new List<BookContentItemContract>();
                    bookContract.ChildContentItems.Add(childBookContract);
                }
            }

            return bookContract;
        }
    }
}