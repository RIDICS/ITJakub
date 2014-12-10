using System.Collections.Generic;
using Castle.MicroKernel;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.FileProcessing.Core.XMLProcessing.XSLT;

namespace ITJakub.FileProcessing.Core.XMLProcessing.Processors.Header
{
    public class MsIdentifierProcessor : ConcreteInstanceProcessorBase<ManuscriptDescription>
    {
        public MsIdentifierProcessor(XsltTransformationManager xsltTransformationManager, IKernel container)
            : base(xsltTransformationManager, container)
        {
        }

        protected override string NodeName
        {
            get { return "msIdentifier"; }
        }


        protected override IEnumerable<ConcreteInstanceProcessorBase<ManuscriptDescription>> ConcreteSubProcessors
        {
            get
            {
                return new List<ConcreteInstanceProcessorBase<ManuscriptDescription>>
                {
                  Container.Resolve<CountryProcessor>(),
                    Container.Resolve<SettlementProcessor>(),
                    Container.Resolve<RepositoryProcessor>(),
                    Container.Resolve<IdnoProcessor>(),
                };
            }
        }
    }
}