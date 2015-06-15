using System.Xml;
using Castle.MicroKernel;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.FileProcessing.Core.XMLProcessing.XSLT;

namespace ITJakub.FileProcessing.Core.XMLProcessing.Processors.BookContent
{
    public class HeadProcessor : ConcreteInstanceListProcessorBase<BookContentItem>
    {

        public HeadProcessor(XsltTransformationManager xsltTransformationManager, IKernel container)
            : base(xsltTransformationManager, container)
        {
        }

        protected override string NodeName
        {
            get { return "head"; }
        }

        protected override void ProcessElement(BookVersion bookVersion, BookContentItem contentItem, XmlReader xmlReader)
        {
            contentItem.Text = GetInnerContentAsString(xmlReader);
        }
    }
}