using System.Xml;
using Castle.MicroKernel;
using ITJakub.FileProcessing.Core.Data;
using ITJakub.FileProcessing.Core.XMLProcessing.XSLT;

namespace ITJakub.FileProcessing.Core.XMLProcessing.Processors.Header
{
    public class EditionProcessor: ListProcessorBase
    {
        public EditionProcessor(XsltTransformationManager xsltTransformationManager, IKernel container)
            : base(xsltTransformationManager, container)
        {
        }

        protected override string NodeName
        {
            get { return "edition"; }
        }

        protected override void ProcessElement(BookData bookData, XmlReader xmlReader)
        {
            //TODO load edition element content somewhere
        }
    }
}