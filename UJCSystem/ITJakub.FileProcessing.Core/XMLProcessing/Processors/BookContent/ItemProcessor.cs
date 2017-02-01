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
                bookData.BookContentItemsCount = 0;
            }

            bookData.BookContentItemsCount++;
            var concreteBookItem = new BookContentItemData
            {
                SubContentItems = new List<BookContentItemData>(),
                ItemOrder = bookData.BookContentItemsCount
            };

            if (parentBookContentItem == null)
            {
                bookData.BookContentItems.Add(concreteBookItem);
            }
            else
            {
                parentBookContentItem.SubContentItems.Add(concreteBookItem);
            }
            
            base.ProcessElement(bookData, concreteBookItem, xmlReader);
        }
    }
}