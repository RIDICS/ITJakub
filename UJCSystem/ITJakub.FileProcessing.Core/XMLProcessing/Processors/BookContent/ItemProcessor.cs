using System.Collections.Generic;
using System.Xml;
using Castle.MicroKernel;
using ITJakub.FileProcessing.Core.Data;
using ITJakub.FileProcessing.Core.XMLProcessing.XSLT;

namespace ITJakub.FileProcessing.Core.XMLProcessing.Processors.BookContent
{
    public class ItemProcessor : ConcreteInstanceProcessorBase<BookContentItemData>
    {
        public ItemProcessor(XsltTransformationManager xsltTransformationManager, IKernel container)
            : base(xsltTransformationManager, container)
        {
        }

        protected override string NodeName
        {
            get { return "item"; }
        }

        protected override IEnumerable<ConcreteInstanceProcessorBase<BookContentItemData>> ConcreteSubProcessors
        {
            get
            {
                return new List<ConcreteInstanceProcessorBase<BookContentItemData>>
                {
                    Container.Resolve<HeadProcessor>(),
                    Container.Resolve<RefProcessor>(),
                    Container.Resolve<ListProcessor>()
                };
            }
        }

        protected override void ProcessElement(BookData bookData, BookContentItemData parentBookContentItem, XmlReader xmlReader)
        {
            if (bookData.BookContentItems == null)
            {
                bookData.BookContentItems = new List<BookContentItemData>();
            }

            var concreteBookItem = new BookContentItemData
            {
                ParentBookContentItem = parentBookContentItem,
                ItemOrder = bookData.BookContentItems.Count
            };

            bookData.BookContentItems.Add(concreteBookItem);

            base.ProcessElement(bookData, concreteBookItem, xmlReader);
        }
    }
}