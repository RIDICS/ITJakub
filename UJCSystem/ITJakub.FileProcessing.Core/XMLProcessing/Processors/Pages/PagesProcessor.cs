using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Castle.MicroKernel;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.FileProcessing.Core.XMLProcessing.XSLT;

namespace ITJakub.FileProcessing.Core.XMLProcessing.Processors.Pages
{
    public class PagesProcessor : ListProcessorBase
    {
        public PagesProcessor(XsltTransformationManager xsltTransformationManager, IKernel container)
            : base(xsltTransformationManager, container)
        {
        }

        protected override string NodeName
        {
            get { return "pages"; }
        }

        protected override void ProcessElement(BookVersion bookVersion, XmlReader xmlReader)
        {
            IEnumerable<XElement> pagesElements = XDocument.Load(xmlReader).Elements().Where(element => element.Name.LocalName == "page");
            int position = 0;
            foreach (XElement pageElement in pagesElements)
            {
                ++position;
                bookVersion.BookPages.Add(new BookPage
                {
                    Position = position,
                    Text = pageElement.Attribute("n").Value,
                    BookVersion = bookVersion,
                    Image = pageElement.Attribute("fax").Value,
                    XmlId = pageElement.Attribute("xml:id").Value,
                    XmlResource = pageElement.Attribute("resource").Value
                });
            }
        }
    }
}