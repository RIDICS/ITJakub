using System.Xml;
using Castle.MicroKernel;
using ITJakub.FileProcessing.Core.Data;
using ITJakub.FileProcessing.Core.XMLProcessing.XSLT;

namespace ITJakub.FileProcessing.Core.XMLProcessing.Processors.Header
{
    public class ExtentProcessor : ConcreteInstanceListProcessorBase<ManuscriptDescriptionData>
    {
        public ExtentProcessor(XsltTransformationManager xsltTransformationManager, IKernel container)
            : base(xsltTransformationManager, container)
        {
        }

        protected override string NodeName
        {
            get { return "extent"; }
        }
        
        protected override void ProcessElement(BookData bookData, ManuscriptDescriptionData msDesc, XmlReader xmlReader)
        {
            msDesc.Extent = GetInnerContentAsString(xmlReader);
        }
    }
}