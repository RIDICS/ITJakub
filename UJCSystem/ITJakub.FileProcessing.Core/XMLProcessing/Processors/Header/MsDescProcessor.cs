using System.Collections.Generic;
using Castle.MicroKernel;
using ITJakub.FileProcessing.Core.Data;
using ITJakub.FileProcessing.Core.XMLProcessing.XSLT;

namespace ITJakub.FileProcessing.Core.XMLProcessing.Processors.Header
{
    public class MsDescProcessor : ConcreteInstanceProcessorBase<ManuscriptDescriptionData>
    {
        public MsDescProcessor(XsltTransformationManager xsltTransformationManager, IKernel container)
            : base(xsltTransformationManager, container)
        {
        }

        protected override string NodeName
        {
            get { return "msDesc"; }
        }

        protected override void PreprocessSetup(BookData bookData)
        {
            if (bookData.ManuscriptDescriptions == null)
            {
                bookData.ManuscriptDescriptions = new List<ManuscriptDescriptionData>();
            }   
        }

        protected override ManuscriptDescriptionData LoadInstance(BookData bookData)
        {
            return new ManuscriptDescriptionData();
        }

        protected override void SaveInstance(ManuscriptDescriptionData instance, BookData bookData)
        {
            bookData.ManuscriptDescriptions.Add(instance);
        }

        protected override IEnumerable<ConcreteInstanceProcessorBase<ManuscriptDescriptionData>> ConcreteSubProcessors
        {
            get
            {
                return new List<ConcreteInstanceProcessorBase<ManuscriptDescriptionData>>
                {
                    Container.Resolve<MsIdentifierProcessor>(),
                    Container.Resolve<MsContentsProcessor>(),
                    Container.Resolve<PhysDescProcessor>(),
                    Container.Resolve<HistoryProcessor>(),
                };
            }
        }
    }
}