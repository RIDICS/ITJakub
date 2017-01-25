using System.Xml;
using Castle.MicroKernel;
using ITJakub.FileProcessing.Core.Data;
using ITJakub.FileProcessing.Core.XMLProcessing.XSLT;

namespace ITJakub.FileProcessing.Core.XMLProcessing.Processors.Header
{
    public class CountryProcessor : ConcreteInstanceListProcessorBase<ManuscriptDescriptionData>
    {
        public CountryProcessor(XsltTransformationManager xsltTransformationManager, IKernel container)
            : base(xsltTransformationManager, container)
        {
        }

        protected override string NodeName
        {
            get { return "country"; }
        }

        protected override void ProcessElement(BookData bookData, ManuscriptDescriptionData msDesc, XmlReader xmlReader)
        {
            msDesc.Country = GetInnerContentAsString(xmlReader);
        }
    
    }
}