using System.Reflection;
using System.Xml;
using Castle.MicroKernel;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.DataEntities.Database.Repositories;
using ITJakub.FileProcessing.Core.XMLProcessing.XSLT;
using log4net;

namespace ITJakub.FileProcessing.Core.XMLProcessing.Processors.Terms
{
    public class TermProcessor : ListProcessorBase
    {
        private readonly TermRepository m_termRepository;
        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public TermProcessor(TermRepository termsRepository, 
            XsltTransformationManager xsltTransformationManager, 
            IKernel container) : base(xsltTransformationManager, container)
        {
            m_termRepository = termsRepository;
        }

        protected override string NodeName
        {
            get { return "term"; }
        }

        protected override void ProcessAttributes(BookVersion bookVersion, XmlReader xmlReader)
        {
            var termXmlId = xmlReader.GetAttribute("id");
            var position = xmlReader.GetAttribute("n");
            string termCategoryName = xmlReader.GetAttribute("subtype");

            string text = GetInnerContentAsString(xmlReader);


            TermCategory termCategory = null;
            if (!string.IsNullOrWhiteSpace(termCategoryName))
            {
                termCategory = m_termRepository.GetTermCategoryByName(termCategoryName) ?? new TermCategory {Name = termCategoryName};
            }
                
            

            var term = new Term
            {
                XmlId = termXmlId,
                Position = long.Parse(position),
                Text = text,
                TermCategory = termCategory,
            };

            m_termRepository.SaveOrUpdate(term);

        }
    }
}
