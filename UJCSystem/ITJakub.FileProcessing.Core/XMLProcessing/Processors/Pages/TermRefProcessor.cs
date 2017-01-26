using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using Castle.MicroKernel;
using ITJakub.FileProcessing.Core.Data;
using ITJakub.FileProcessing.Core.XMLProcessing.XSLT;
using log4net;

namespace ITJakub.FileProcessing.Core.XMLProcessing.Processors.Pages
{
    public class TermRefProcessor : ConcreteInstanceListProcessorBase<BookPageData>
    {
        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public TermRefProcessor(XsltTransformationManager xsltTransformationManager, IKernel container) : base(xsltTransformationManager, container)
        {
        }

        protected override string NodeName
        {
            get { return "termRef"; }
        }

        protected override void ProcessElement(BookData bookData, BookPageData page, XmlReader xmlReader)
        {
            if (page.TermXmlIds == null)
                page.TermXmlIds = new List<string>();

            var refTermXmlId = xmlReader.GetAttribute("n");
            
            page.TermXmlIds.Add(refTermXmlId);
        }
    }
}