using System.Xml;
using Castle.MicroKernel;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.FileProcessing.Core.XSLT;

namespace ITJakub.FileProcessing.Core.Processors.Header
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