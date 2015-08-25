using System.Reflection;
using System.Xml;
using Castle.MicroKernel;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.FileProcessing.Core.XMLProcessing.XSLT;
using log4net;

namespace ITJakub.FileProcessing.Core.XMLProcessing.Processors.Terms
{
    public class TermProcessor : ListProcessorBase
    {
        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public TermProcessor(XsltTransformationManager xsltTransformationManager, IKernel container) : base(xsltTransformationManager, container)
        {
        }

        protected override string NodeName
        {
            get { return "term"; }
        }

        protected override void ProcessAttributes(BookVersion bookVersion, XmlReader xmlReader)
        {
            var termXmlId = xmlReader.GetAttribute("id");
            var position = xmlReader.GetAttribute("n");
            string text = GetInnerContentAsString(xmlReader);

            //TODO create term
        }
    }
}
