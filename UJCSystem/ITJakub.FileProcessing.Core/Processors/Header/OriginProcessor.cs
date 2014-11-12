using System.Collections.Generic;
using Castle.MicroKernel;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.FileProcessing.Core.XSLT;

namespace ITJakub.FileProcessing.Core.Processors.Header
{
    public class OriginProcessor : ConcreteInstanceProcessorBase<ManuscriptDescription>
    {
        public OriginProcessor(XsltTransformationManager xsltTransformationManager, IKernel container)
            : base(xsltTransformationManager, container)
        {
        }

        protected override string NodeName
        {
            get { return "origin"; }
        }

        protected override IEnumerable<ConcreteInstanceProcessorBase<ManuscriptDescription>> ConcreteSubProcessors
        {
            get
            {
                return new List<ConcreteInstanceProcessorBase<ManuscriptDescription>>
                {
                      Container.Resolve<OrigDateProcessor>(),
                };
            }
        }
    }
}