using System.Linq;
using System.Xml;
using Castle.MicroKernel;
using ITJakub.FileProcessing.Core.Data;
using ITJakub.FileProcessing.Core.XMLProcessing.XSLT;

namespace ITJakub.FileProcessing.Core.XMLProcessing.Processors.BookContent
{
    public class RefProcessor : ConcreteInstanceListProcessorBase<BookContentItemData>
    {
        public RefProcessor(XsltTransformationManager xsltTransformationManager, IKernel container)
            : base(xsltTransformationManager, container)
        {
        }

        protected override string NodeName
        {
            get { return "ref"; }
        }

        protected override void ProcessElement(BookData bookData, BookContentItemData contentItem, XmlReader xmlReader)
        {
            var pageXmlIdRefference = xmlReader.GetAttribute("target");
            if (pageXmlIdRefference != null && bookData.Pages != null)
            {
                var pageXmlId = pageXmlIdRefference.Substring(1);
                var bookPage = bookData.Pages.SingleOrDefault(x => x.XmlId == pageXmlId);
                contentItem.Page = bookPage;
            }
        }
    }
}