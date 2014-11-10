using System.Collections.Generic;
using Castle.MicroKernel;
using ITJakub.FileProcessing.Core.XSLT;

namespace ITJakub.FileProcessing.Core.Processors.Header
{
    public class PublicationStmtProcessor : ProcessorBase
    {
        public PublicationStmtProcessor(XsltTransformationManager xsltTransformationManager, IKernel container)
            : base(xsltTransformationManager, container)
        {
        }

        protected override string NodeName
        {
            get { return "publicationStmt"; }
        }

        protected override IEnumerable<ProcessorBase> SubProcessors
        {
            get
            {
                return new List<ProcessorBase>
                {
                    Container.Resolve<PubPlaceProcessor>(),
                    Container.Resolve<DateProcessor>(),
                    Container.Resolve<AvailabilityProcessor>(),
                    Container.Resolve<PublisherProcessor>(),
                };
            }
        }
    }
}