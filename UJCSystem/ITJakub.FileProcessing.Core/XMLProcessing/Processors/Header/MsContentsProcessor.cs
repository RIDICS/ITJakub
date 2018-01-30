using System.Collections.Generic;
using Castle.MicroKernel;
using ITJakub.FileProcessing.Core.Data;
using ITJakub.FileProcessing.Core.XMLProcessing.XSLT;

namespace ITJakub.FileProcessing.Core.XMLProcessing.Processors.Header
{
    public class MsContentsProcessor : ConcreteInstanceProcessorBase<ManuscriptDescriptionData>
    {
        public MsContentsProcessor(XsltTransformationManager xsltTransformationManager, IKernel container)
            : base(xsltTransformationManager, container)
        {
        }

        protected override string NodeName
        {
            get { return "msContents"; }
        }

        protected override IEnumerable<ConcreteInstanceProcessorBase<ManuscriptDescriptionData>> ConcreteSubProcessors
        {
            get
            {
                return new List<ConcreteInstanceProcessorBase<ManuscriptDescriptionData>>
                {
                  Container.Resolve<MsItemProcessor>(),
                };
            }
        }
    }
}