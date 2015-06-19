using System.Xml;
using Castle.MicroKernel;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.FileProcessing.Core.XMLProcessing.XSLT;

namespace ITJakub.FileProcessing.Core.XMLProcessing.Processors.Header
{
    public class CountryProcessor : ConcreteInstanceListProcessorBase<ManuscriptDescription>
    {
        public CountryProcessor(XsltTransformationManager xsltTransformationManager, IKernel container)
            : base(xsltTransformationManager, container)
        {
        }

        protected override string NodeName
        {
            get { return "contry"; }
        }

        protected override void ProcessElement(BookVersion bookVersion, ManuscriptDescription msDesc, XmlReader xmlReader)
        {
            msDesc.Country = GetInnerContentAsString(xmlReader);
        }
    
    }
}