using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using Castle.MicroKernel;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.DataEntities.Database.Repositories;
using ITJakub.FileProcessing.Core.XMLProcessing.Processors.BookContent;
using ITJakub.FileProcessing.Core.XMLProcessing.XSLT;
using log4net;

namespace ITJakub.FileProcessing.Core.XMLProcessing.Processors.Pages
{
    public class TermRefProcessor : ConcreteInstanceListProcessorBase<BookPage>
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

        protected override void ProcessElement(BookVersion bookVersion, BookPage page, XmlReader xmlReader)
        {
            if (page.Terms == null)
                page.Terms = new List<Term>();

            var refTermXmlId = xmlReader.GetAttribute("n");
            var term = m_termRepository.FindByXmlId(refTermXmlId);

            page.Terms.Add(term);
        }
    }
}