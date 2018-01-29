using System;
using System.Xml;
using Castle.MicroKernel;
using ITJakub.FileProcessing.Core.Data;
using ITJakub.FileProcessing.Core.XMLProcessing.XSLT;

namespace ITJakub.FileProcessing.Core.XMLProcessing.Processors.Header
{
    public class OrigDateProcessor : ConcreteInstanceListProcessorBase<ManuscriptDescriptionData>
    {
        public OrigDateProcessor(XsltTransformationManager xsltTransformationManager, IKernel container)
            : base(xsltTransformationManager, container)
        {
        }

        protected override string NodeName
        {
            get { return "origDate"; }
        }

        protected override void ProcessElement(BookData bookData, ManuscriptDescriptionData msDesc, XmlReader xmlReader)
        {
            var notBefore = xmlReader.GetAttribute("notBefore");
            var notAfter = xmlReader.GetAttribute("notAfter");
            msDesc.NotBefore = new DateTime(Convert.ToInt32(notBefore), 1, 1);
            msDesc.NotAfter = new DateTime(Convert.ToInt32(notAfter), 1, 1);
            msDesc.OriginDate = GetInnerContentAsString(xmlReader);
        }
    
    }
}