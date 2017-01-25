using System.Xml;
using Castle.MicroKernel;
using ITJakub.FileProcessing.Core.Data;
using ITJakub.FileProcessing.Core.XMLProcessing.XSLT;

namespace ITJakub.FileProcessing.Core.XMLProcessing.Processors.Header
{
    public class MsTitleProcessor : ConcreteInstanceListProcessorBase<ManuscriptDescriptionData>
    {
        public MsTitleProcessor(XsltTransformationManager xsltTransformationManager, IKernel container)
            : base(xsltTransformationManager, container)
        {
        }

        protected override string NodeName
        {
            get { return "title"; }
        }

        protected override void ProcessElement(BookData bookData, ManuscriptDescriptionData msDesc, XmlReader xmlReader)
        {
            msDesc.Title = GetInnerContentAsString(xmlReader);
        }
    }
}