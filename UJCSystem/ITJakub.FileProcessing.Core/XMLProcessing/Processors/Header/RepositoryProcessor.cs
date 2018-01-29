using System.Xml;
using Castle.MicroKernel;
using ITJakub.FileProcessing.Core.Data;
using ITJakub.FileProcessing.Core.XMLProcessing.XSLT;

namespace ITJakub.FileProcessing.Core.XMLProcessing.Processors.Header
{
    public class RepositoryProcessor : ConcreteInstanceListProcessorBase<ManuscriptDescriptionData>
    {
        public RepositoryProcessor(XsltTransformationManager xsltTransformationManager, IKernel container)
            : base(xsltTransformationManager, container)
        {
        }

        protected override string NodeName
        {
            get { return "repository"; }
        }

        protected override void ProcessElement(BookData bookData, ManuscriptDescriptionData msDesc, XmlReader xmlReader)
        {
            xmlReader.Read();                           //read text value
            string value = xmlReader.Value;
            msDesc.Repository = value;
        }
    
    }
}