using System.Collections.Generic;
using System.Xml;
using Castle.MicroKernel;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.FileProcessing.Core.XSLT;

namespace ITJakub.FileProcessing.Core.Processors.Header
{
    public class MsDescProcessor : ConcreteInstanceProcessorBase<ManuscriptDescription>
    {
        public MsDescProcessor(XsltTransformationManager xsltTransformationManager, IKernel container)
            : base(xsltTransformationManager, container)
        {
        }

        protected override string NodeName
        {
            get { return "msDesc"; }
        }

        protected override void PreprocessSetup(BookVersion bookVersion)
        {
            if (bookVersion.ManuscriptDescriptions == null)
            {
                bookVersion.ManuscriptDescriptions = new List<ManuscriptDescription>();
            }   
        }

        protected override ManuscriptDescription LoadInstance(BookVersion bookVersion)
        {
            return new ManuscriptDescription();
        }

        protected override void SaveInstance(ManuscriptDescription instance, BookVersion bookVersion)
        {
            bookVersion.ManuscriptDescriptions.Add(instance);
        }

        protected override IEnumerable<ConcreteInstanceProcessorBase<ManuscriptDescription>> ConcreteSubProcessors
        {
            get
            {
                return new List<ConcreteInstanceProcessorBase<ManuscriptDescription>>
                {
                    Container.Resolve<MsIdentifierProcessor>(),
                    Container.Resolve<MsContentsProcessor>(),
                    Container.Resolve<HistoryProcessor>(),
                };
            }
        }
    }
}