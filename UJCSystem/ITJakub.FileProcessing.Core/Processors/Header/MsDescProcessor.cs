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

        protected override void ProcessElement(BookVersion bookVersion, XmlReader xmlReader)
        {
            var manuscriptDesc = new ManuscriptDescription();
            Process(manuscriptDesc, xmlReader);
            bookVersion.ManuscriptDescriptions.Add(manuscriptDesc);
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