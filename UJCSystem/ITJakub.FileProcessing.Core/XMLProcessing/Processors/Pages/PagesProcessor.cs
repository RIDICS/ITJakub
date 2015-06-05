using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using Castle.MicroKernel;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.FileProcessing.Core.XMLProcessing.XSLT;
using log4net;

namespace ITJakub.FileProcessing.Core.XMLProcessing.Processors.Pages
{
    public class PagesProcessor : ListProcessorBase
    {
        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

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
            var pagesElements =
                XDocument.Load(xmlReader).Descendants().Where(element => element.Name.LocalName == "page"); //TODO make pageProcessor and dont load all pages to memory
            var position = 0;
            if (bookVersion.BookPages == null)
            {
                bookVersion.BookPages = new List<BookPage>();
            }

            foreach (var pageElement in pagesElements)
            {
                ++position;
                var faxValue = GetAttributeValue(pageElement, "fax");
                var pageNameValue = GetAttributeValue(pageElement, "n") ?? Convert.ToString(position);
                var pageIdValue = GetAttributeValue(pageElement, XmlNamespace + "id") ?? Convert.ToString(Guid.NewGuid());
                var xmlResourceValue = GetAttributeValue(pageElement, "resource");

                if (string.IsNullOrEmpty(xmlResourceValue) && m_log.IsFatalEnabled)
                    m_log.ErrorFormat("Metadata_processor : Page in position {0} does not have resource attribute",
                        position);

                bookVersion.BookPages.Add(new BookPage
                {
                    Position = position,
                    Text = pageNameValue,
                    BookVersion = bookVersion,
                    Image = faxValue,
                    XmlId = pageIdValue,
                    XmlResource = xmlResourceValue
                });
            }
        }


    }
}