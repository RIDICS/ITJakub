using System.Xml;
using Castle.MicroKernel;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.FileProcessing.Core.XMLProcessing.XSLT;

namespace ITJakub.FileProcessing.Core.XMLProcessing.Processors.Header
{
    public class SettlementProcessor : ConcreteInstanceListProcessorBase<ManuscriptDescription>
    {
        public SettlementProcessor(XsltTransformationManager xsltTransformationManager, IKernel container)
            : base(xsltTransformationManager, container)
        {
        }

        protected override string NodeName
        {
            get { return "settlement"; }
        }

        protected override void ProcessElement(ManuscriptDescription msDesc, XmlReader xmlReader)
        {
            msDesc.Settlement = GetInnerContentAsString(xmlReader);
        }
    
    }
}