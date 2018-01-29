using System.Collections.Generic;
using Castle.MicroKernel;
using ITJakub.FileProcessing.Core.Data;
using ITJakub.FileProcessing.Core.XMLProcessing.XSLT;

namespace ITJakub.FileProcessing.Core.XMLProcessing.Processors.Header
{
    public class SupportDescProcessor : ConcreteInstanceProcessorBase<ManuscriptDescriptionData>
    {
        public SupportDescProcessor(XsltTransformationManager xsltTransformationManager, IKernel container)
            : base(xsltTransformationManager, container)
        {
        }

        protected override string NodeName
        {
            get { return "supportDesc"; }
        }

        protected override IEnumerable<ConcreteInstanceProcessorBase<ManuscriptDescriptionData>> ConcreteSubProcessors
        {
            get
            {
                return new List<ConcreteInstanceProcessorBase<ManuscriptDescriptionData>>
                {
                    Container.Resolve<ExtentProcessor>(),
                };
            }
        }
    }
}