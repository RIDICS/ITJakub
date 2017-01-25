using System.Xml;
using Castle.MicroKernel;
using ITJakub.FileProcessing.Core.Data;
using ITJakub.FileProcessing.Core.XMLProcessing.XSLT;

namespace ITJakub.FileProcessing.Core.XMLProcessing.Processors.Header
{
    public class DateProcessor : ListProcessorBase
    {
        public DateProcessor(XsltTransformationManager xsltTransformationManager, IKernel container)
            : base(xsltTransformationManager, container)
        {
        }

        protected override string NodeName
        {
            get { return "date"; }
        }

        protected override void ProcessElement(BookData bookData, XmlReader xmlReader)
        {
            bookData.PublishDate = GetInnerContentAsString(xmlReader);
        }
    }
}