using System.Xml;
using Castle.MicroKernel;
using ITJakub.FileProcessing.Core.Data;
using ITJakub.FileProcessing.Core.XMLProcessing.XSLT;

namespace ITJakub.FileProcessing.Core.XMLProcessing.Processors.Header
{
    public class IdnoProcessor : ConcreteInstanceListProcessorBase<ManuscriptDescriptionData>
    {
        public IdnoProcessor(XsltTransformationManager xsltTransformationManager, IKernel container)
            : base(xsltTransformationManager, container)
        {
        }

        protected override string NodeName
        {
            get { return "idno"; }
        }

        protected override void ProcessElement(BookData bookData, ManuscriptDescriptionData msDesc, XmlReader xmlReader)
        {
            xmlReader.Read();                           //read text value
            string value = xmlReader.Value;
            msDesc.Idno = value;
        }
    
    }
}