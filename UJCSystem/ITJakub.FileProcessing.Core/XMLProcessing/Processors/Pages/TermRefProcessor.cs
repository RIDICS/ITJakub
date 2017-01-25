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
        private readonly TermRepository m_termRepository;
        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public TermRefProcessor(TermRepository termRepository, XsltTransformationManager xsltTransformationManager, IKernel container) : base(xsltTransformationManager, container)
        {
            m_termRepository = termRepository;
        }

        protected override string NodeName
        {
            get { return "termRef"; }
        }

        protected override void ProcessElement(BookData bookData, BookPageData page, XmlReader xmlReader)
        {
            if (page.Terms == null)
                page.Terms = new List<TermData>();

            var refTermXmlId = xmlReader.GetAttribute("n");
            var term = m_termRepository.FindByXmlId(refTermXmlId);

            page.Terms.Add(term);
        }
    }
}