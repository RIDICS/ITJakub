using System.Collections.Generic;
using Castle.MicroKernel;
using ITJakub.FileProcessing.Core.Data;
using ITJakub.FileProcessing.Core.XMLProcessing.XSLT;

namespace ITJakub.FileProcessing.Core.XMLProcessing.Processors.Header
{
    public class HistoryProcessor : ConcreteInstanceProcessorBase<ManuscriptDescriptionData>
    {
        public HistoryProcessor(XsltTransformationManager xsltTransformationManager, IKernel container)
            : base(xsltTransformationManager, container)
        {
        }

        protected override string NodeName
        {
            get { return "history"; }
        }

        protected override IEnumerable<ConcreteInstanceProcessorBase<ManuscriptDescriptionData>> ConcreteSubProcessors
        {
            get
            {
                return new List<ConcreteInstanceProcessorBase<ManuscriptDescriptionData>>
                {
                  Container.Resolve<OriginProcessor>(),
                };
            }
        }
    }
}