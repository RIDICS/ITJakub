using System.Collections.Generic;
using Castle.MicroKernel;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.FileProcessing.Core.XMLProcessing.XSLT;

namespace ITJakub.FileProcessing.Core.XMLProcessing.Processors.BookContent
{
    public class ListProcessor : ConcreteInstanceProcessorBase<BookContentItem>
    {

        public ListProcessor(XsltTransformationManager xsltTransformationManager, IKernel container)
            : base(xsltTransformationManager, container)
        {
        }
            
        protected override string NodeName
        {
            get { return "list"; }
        }

        protected override IEnumerable<ConcreteInstanceProcessorBase<BookContentItem>> ConcreteSubProcessors
        {
            get
            {
                return new List<ConcreteInstanceProcessorBase<BookContentItem>>
                {
                    Container.Resolve<ItemProcessor>()
                };
            }
        }
    }
}