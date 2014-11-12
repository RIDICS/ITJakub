using System.Xml;
using Castle.MicroKernel;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.FileProcessing.Core.XSLT;

namespace ITJakub.FileProcessing.Core.Processors.Header
{
    public class OrigDateProcessor : ConcreteInstanceListProcessorBase<ManuscriptDescription>
    {
        public OrigDateProcessor(XsltTransformationManager xsltTransformationManager, IKernel container)
            : base(xsltTransformationManager, container)
        {
        }

        protected override string NodeName
        {
            get { return "origDate"; }
        }

        protected override void ProcessElement(ManuscriptDescription msDesc, XmlReader xmlReader)
        {
            msDesc.OriginDate = GetInnerContentAsString(xmlReader);
        }
    
    }
}