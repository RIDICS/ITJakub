using System.Xml;
using Castle.MicroKernel;
using ITJakub.FileProcessing.Core.Data;
using ITJakub.FileProcessing.Core.XMLProcessing.XSLT;

namespace ITJakub.FileProcessing.Core.XMLProcessing.Processors.Header
{
    public class TitleProcessor : ListProcessorBase
    {
        public TitleProcessor(XsltTransformationManager xsltTransformationManager, IKernel container)
            : base(xsltTransformationManager, container)
        {
        }

        protected override string NodeName
        {
            get { return "title"; }
        }

        protected override void ProcessElement(BookData bookData, XmlReader xmlReader)
        {
            string titleType = xmlReader.GetAttribute("type");
            if (string.IsNullOrEmpty(titleType) || titleType.Equals("main"))
            {
                bookData.Title = GetInnerContentAsString(xmlReader);
            }
            else if (titleType.Equals("sub"))
            {
                bookData.SubTitle = GetInnerContentAsString(xmlReader);
            } 
        }
    }
}