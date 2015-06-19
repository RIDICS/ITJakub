using System.Collections.Generic;
using System.Xml;
using Castle.MicroKernel;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.DataEntities.Database.Repositories;
using ITJakub.FileProcessing.Core.XMLProcessing.XSLT;

namespace ITJakub.FileProcessing.Core.XMLProcessing.Processors.BookContent
{
    public class ItemProcessor : ConcreteInstanceProcessorBase<BookContentItem>
    {
        public ItemProcessor(XsltTransformationManager xsltTransformationManager, IKernel container)
            : base(xsltTransformationManager, container)
        {
        }

        protected override string NodeName
        {
            get { return "item"; }
        }

        protected override IEnumerable<ConcreteInstanceProcessorBase<BookContentItem>> ConcreteSubProcessors
        {
            get
            {
                return new List<ConcreteInstanceProcessorBase<BookContentItem>>
                {
                    Container.Resolve<HeadProcessor>(),
                    Container.Resolve<RefProcessor>(),
                    Container.Resolve<ListProcessor>()
                };
            }
        }

        protected override void ProcessElement(BookVersion bookVersion, BookContentItem parentBookContentItem, XmlReader xmlReader)
        {
            if (bookVersion.BookContentItems == null)
            {
                bookVersion.BookContentItems = new List<BookContentItem>();
            }

            var concreteBookItem = new BookContentItem
            {
                ParentBookContentItem = parentBookContentItem,
                BookVersion = bookVersion,
                ItemOrder = bookVersion.BookContentItems.Count
            };

            bookVersion.BookContentItems.Add(concreteBookItem);

            base.ProcessElement(bookVersion, concreteBookItem, xmlReader);
        }
    }
}