using System.Xml;
using Castle.MicroKernel;
using ITJakub.FileProcessing.Core.Data;
using ITJakub.FileProcessing.Core.XMLProcessing.XSLT;

namespace ITJakub.FileProcessing.Core.XMLProcessing.Processors.Header
{
    public class PubPlaceProcessor : ListProcessorBase
    {
        public PubPlaceProcessor(XsltTransformationManager xsltTransformationManager, IKernel container) : base(xsltTransformationManager, container)
        {
        }

        protected override string NodeName
        {
            get { return "pubPlace"; }
        }

        protected override void ProcessElement(BookData bookData, XmlReader xmlReader)
        {
            bookData.PublishPlace = GetInnerContentAsString(xmlReader);
        }
    }
}