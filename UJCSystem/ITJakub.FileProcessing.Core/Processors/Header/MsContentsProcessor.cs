using System.Collections.Generic;
using Castle.MicroKernel;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.FileProcessing.Core.XSLT;

namespace ITJakub.FileProcessing.Core.Processors.Header
{
    public class MsContentsProcessor : ConcreteInstanceProcessorBase<ManuscriptDescription>
    {
        public MsContentsProcessor(XsltTransformationManager xsltTransformationManager, IKernel container)
            : base(xsltTransformationManager, container)
        {
        }

        protected override string NodeName
        {
            get { return "msContents"; }
        }

        protected override IEnumerable<ConcreteInstanceProcessorBase<ManuscriptDescription>> ConcreteSubProcessors
        {
            get
            {
                return new List<ConcreteInstanceProcessorBase<ManuscriptDescription>>
                {
                  Container.Resolve<MsItemProcessor>(),
                };
            }
        }
    }
}