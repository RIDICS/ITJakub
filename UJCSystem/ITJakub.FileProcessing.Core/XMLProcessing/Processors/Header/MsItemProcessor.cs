using System.Collections.Generic;
using Castle.MicroKernel;
using ITJakub.FileProcessing.Core.Data;
using ITJakub.FileProcessing.Core.XMLProcessing.XSLT;

namespace ITJakub.FileProcessing.Core.XMLProcessing.Processors.Header
{
    public class MsItemProcessor : ConcreteInstanceProcessorBase<ManuscriptDescriptionData>
    {
        public MsItemProcessor(XsltTransformationManager xsltTransformationManager, IKernel container)
            : base(xsltTransformationManager, container)
        {
        }

        protected override string NodeName
        {
            get { return "msItem"; }
        }

        protected override IEnumerable<ConcreteInstanceProcessorBase<ManuscriptDescriptionData>> ConcreteSubProcessors
        {
            get
            {
                return new List<ConcreteInstanceProcessorBase<ManuscriptDescriptionData>>
                {
                  Container.Resolve<MsTitleProcessor>(),
                };
            }
        }
    }
}