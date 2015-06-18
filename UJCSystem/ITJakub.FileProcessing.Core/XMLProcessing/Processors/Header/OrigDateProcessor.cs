using System.Xml;
using Castle.MicroKernel;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.FileProcessing.Core.XMLProcessing.XSLT;

namespace ITJakub.FileProcessing.Core.XMLProcessing.Processors.Header
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

        protected override void ProcessElement(BookVersion bookVersion, ManuscriptDescription msDesc, XmlReader xmlReader)
        {
            msDesc.NotBefore = xmlReader.GetAttribute("notBefore");
            msDesc.NotAfter = xmlReader.GetAttribute("notAfter");
            msDesc.OriginDate = GetInnerContentAsString(xmlReader);
        }
    
    }
}