using System.Linq;
using System.Xml;
using Castle.MicroKernel;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.FileProcessing.Core.XMLProcessing.XSLT;

namespace ITJakub.FileProcessing.Core.XMLProcessing.Processors.BookContent
{
    public class RefProcessor : ConcreteInstanceListProcessorBase<BookContentItem>
    {
        public RefProcessor(XsltTransformationManager xsltTransformationManager, IKernel container)
            : base(xsltTransformationManager, container)
        {
        }

        protected override string NodeName
        {
            get { return "ref"; }
        }

        protected override void ProcessElement(BookVersion bookVersion, BookContentItem contentItem, XmlReader xmlReader)
        {
            var pageXmlIdRefference = xmlReader.GetAttribute("target");
            if (pageXmlIdRefference != null && bookVersion.BookPages != null)
            {
                var pageXmlId = pageXmlIdRefference.Substring(1);
                var bookPage = bookVersion.BookPages.SingleOrDefault(x => x.XmlId == pageXmlId);
                contentItem.Page = bookPage;
            }
        }
    }
}