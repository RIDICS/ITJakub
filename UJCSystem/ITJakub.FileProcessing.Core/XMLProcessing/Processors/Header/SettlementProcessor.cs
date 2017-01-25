using System.Xml;
using Castle.MicroKernel;
using ITJakub.FileProcessing.Core.Data;
using ITJakub.FileProcessing.Core.XMLProcessing.XSLT;

namespace ITJakub.FileProcessing.Core.XMLProcessing.Processors.Header
{
    public class SettlementProcessor : ConcreteInstanceListProcessorBase<ManuscriptDescriptionData>
    {
        public SettlementProcessor(XsltTransformationManager xsltTransformationManager, IKernel container)
            : base(xsltTransformationManager, container)
        {
        }

        protected override string NodeName
        {
            get { return "settlement"; }
        }

        protected override void ProcessElement(BookData bookData, ManuscriptDescriptionData msDesc, XmlReader xmlReader)
        {
            msDesc.Settlement = GetInnerContentAsString(xmlReader);
        }
    }
}