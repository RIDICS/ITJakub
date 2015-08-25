using System;
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
            var notBefore = xmlReader.GetAttribute("notBefore");
            var notAfter = xmlReader.GetAttribute("notAfter");
            msDesc.NotBefore = new DateTime(Convert.ToInt32(notBefore), 1, 1);
            msDesc.NotAfter = new DateTime(Convert.ToInt32(notAfter), 1, 1);
            msDesc.OriginDate = GetInnerContentAsString(xmlReader);
        }
    
    }
}