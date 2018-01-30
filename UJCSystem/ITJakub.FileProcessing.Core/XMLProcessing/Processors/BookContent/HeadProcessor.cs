using System.Xml;
using Castle.MicroKernel;
using ITJakub.FileProcessing.Core.Data;
using ITJakub.FileProcessing.Core.XMLProcessing.XSLT;

namespace ITJakub.FileProcessing.Core.XMLProcessing.Processors.BookContent
{
    public class HeadProcessor : ConcreteInstanceListProcessorBase<BookContentItemData>
    {

        public HeadProcessor(XsltTransformationManager xsltTransformationManager, IKernel container)
            : base(xsltTransformationManager, container)
        {
        }

        protected override string NodeName
        {
            get { return "head"; }
        }

        protected override void ProcessElement(BookData bookData, BookContentItemData contentItem, XmlReader xmlReader)
        {
            contentItem.Text = GetInnerContentAsString(xmlReader);
        }
    }
}