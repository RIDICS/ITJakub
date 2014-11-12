using System.Collections.Generic;
using Castle.MicroKernel;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.FileProcessing.Core.XSLT;

namespace ITJakub.FileProcessing.Core.Processors.Header
{
    public class MsDescProcessor : ProcessorBase
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
            if (bookVersion.ManuscriptDescription == null)
            {
                bookVersion.ManuscriptDescription = new ManuscriptDescription();
            }   
        }

        protected override IEnumerable<ProcessorBase> SubProcessors
        {
            get
            {
                return new List<ProcessorBase>
                {
                    Container.Resolve<MsIdentifierProcessor>(),
                    Container.Resolve<MsContentsProcessor>(),
                    Container.Resolve<HistoryProcessor>(),
                };
            }
        }
    }
}