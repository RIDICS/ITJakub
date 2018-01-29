using System.Collections.Generic;
using Castle.MicroKernel;
using ITJakub.FileProcessing.Core.Data;
using ITJakub.FileProcessing.Core.XMLProcessing.XSLT;

namespace ITJakub.FileProcessing.Core.XMLProcessing.Processors.Terms
{
    public class TermsProcessor : ProcessorBase
    {
        public TermsProcessor(XsltTransformationManager xsltTransformationManager, IKernel container)
            : base(xsltTransformationManager, container)
        {
        }

        protected override string NodeName
        {
            get { return "terms"; }
        }

        protected override IEnumerable<ProcessorBase> SubProcessors
        {
            get
            {
                return new List<ProcessorBase>
                {
                    Container.Resolve<TermProcessor>()
                };
            }
        }

        protected override void PreprocessSetup(BookData bookData)
        {
            if (bookData.Terms == null)
            {
                bookData.Terms = new List<TermData>();
            }
        }
    }
}
