using System.Collections.Generic;
using Castle.MicroKernel;
using ITJakub.FileProcessing.Core.Data;
using ITJakub.FileProcessing.Core.XMLProcessing.XSLT;

namespace ITJakub.FileProcessing.Core.XMLProcessing.Processors.BookContent
{
    public class ListProcessor : ConcreteInstanceProcessorBase<BookContentItemData>
    {
        private readonly ItemProcessor m_itemProcessor;

        public ListProcessor(XsltTransformationManager xsltTransformationManager, IKernel container, ItemProcessor itemProcessor)
            : base(xsltTransformationManager, container)
        {
            m_itemProcessor = itemProcessor;
        }

        protected override string NodeName
        {
            get { return "list"; }
        }

        protected override IEnumerable<ConcreteInstanceProcessorBase<BookContentItemData>> ConcreteSubProcessors
        {
            get
            {
                return new List<ConcreteInstanceProcessorBase<BookContentItemData>>
                {
                    m_itemProcessor
                };
            }
        }
    }
}