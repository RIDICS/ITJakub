using System.Xml;
using Castle.MicroKernel;
using ITJakub.FileProcessing.Core.Data;
using ITJakub.FileProcessing.Core.XMLProcessing.XSLT;

namespace ITJakub.FileProcessing.Core.XMLProcessing.Processors.Header
{
    public class AvailabilityProcessor : ListProcessorBase
    {
        public AvailabilityProcessor(XsltTransformationManager xsltTransformationManager, IKernel container)
            : base(xsltTransformationManager, container)
        {
        }

        protected override string NodeName
        {
            get { return "availability"; }
        }

        protected override void ProcessElement(BookData bookData, XmlReader xmlReader)
        {
            bookData.Copyright = GetInnerContentAsString(xmlReader);
        }

        protected override void ProcessAttributes(BookData bookData, XmlReader xmlReader)
        {
            string status = xmlReader.GetAttribute("status");
            bookData.AvailabilityStatus = ParseEnum<AvailabilityStatusEnum>(status);
        }
    }
}