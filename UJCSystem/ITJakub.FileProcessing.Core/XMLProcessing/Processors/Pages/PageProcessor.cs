using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using Castle.MicroKernel;
using ITJakub.FileProcessing.Core.Data;
using ITJakub.FileProcessing.Core.XMLProcessing.XSLT;
using log4net;

namespace ITJakub.FileProcessing.Core.XMLProcessing.Processors.Pages
{
    public class PageProcessor : ConcreteInstanceProcessorBase<BookPageData>
    {
        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public PageProcessor(XsltTransformationManager xsltTransformationManager, IKernel container) : base(xsltTransformationManager, container)
        {
        }

        protected override string NodeName
        {
            get { return "page"; }
        }

        protected override void ProcessAttributes(BookData bookData, XmlReader xmlReader)
        {
            var position = bookData.Pages.Count + 1;
            var facsValue = xmlReader.GetAttribute("facs");
            var pageNameValue = xmlReader.GetAttribute("n") ?? Convert.ToString(position);
            var pageIdValue = xmlReader.GetAttribute("id", XmlNamespace.NamespaceName);
            var xmlResourceValue = xmlReader.GetAttribute("resource");

            if (string.IsNullOrEmpty(xmlResourceValue) && m_log.IsFatalEnabled)
                m_log.ErrorFormat("Metadata_processor : Page in position {0} does not have resource attribute",
                    position);

            var page = new BookPageData
            {
                Position = position,
                Text = pageNameValue,
                Image = facsValue,
                XmlId = pageIdValue,
                XmlResource = xmlResourceValue
            };

            bookData.Pages.Add(page);

            base.ProcessElement(bookData, page, xmlReader);
        }

        protected override IEnumerable<ConcreteInstanceProcessorBase<BookPageData>> ConcreteSubProcessors
        {
            get { return new List<ConcreteInstanceProcessorBase<BookPageData>> { Container.Resolve<TermRefProcessor>() }; }
        }
    }
}
