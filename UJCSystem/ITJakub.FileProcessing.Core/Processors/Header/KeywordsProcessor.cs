using System.Collections.Generic;
using Castle.MicroKernel;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.FileProcessing.Core.XSLT;

namespace ITJakub.FileProcessing.Core.Processors.Header
{
    public class KeywordsProcessor : ProcessorBase
    {
        public KeywordsProcessor(XsltTransformationManager xsltTransformationManager, IKernel container)
            : base(xsltTransformationManager, container)
        {
        }

        protected override string NodeName
        {
            get { return "keywords"; }
        }

        protected override void PreprocessSetup(BookVersion bookVersion)
        {
            if (bookVersion.Keywords == null)
            {
                bookVersion.Keywords = new List<Keyword>();
            }
        }

        protected override IEnumerable<ProcessorBase> SubProcessors
        {
            get
            {
                return new List<ProcessorBase>
                {
                    Container.Resolve<TermProcessor>(),
                };
            }
        }
    }
}