using System.Reflection;
using System.Xml;
using Castle.MicroKernel;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.FileProcessing.Core.XMLProcessing.Processors.BookContent;
using ITJakub.FileProcessing.Core.XMLProcessing.XSLT;
using log4net;

namespace ITJakub.FileProcessing.Core.XMLProcessing.Processors.Pages
{
    public class TermRefProcessor : ListProcessorBase
    {
        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public TermRefProcessor(XsltTransformationManager xsltTransformationManager, IKernel container) : base(xsltTransformationManager, container)
        {
        }

        protected override string NodeName
        {
            get { return "termRef"; }
        }

        protected override void ProcessAttributes(BookVersion bookVersion, XmlReader xmlReader)
        {
            var refTermXmlId = xmlReader.GetAttribute("n");
        }
    }
}