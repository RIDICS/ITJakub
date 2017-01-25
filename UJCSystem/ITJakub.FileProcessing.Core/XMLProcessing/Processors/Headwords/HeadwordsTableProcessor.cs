using System.Collections.Generic;
using Castle.MicroKernel;
using ITJakub.FileProcessing.Core.Data;
using ITJakub.FileProcessing.Core.XMLProcessing.XSLT;

namespace ITJakub.FileProcessing.Core.XMLProcessing.Processors.Headwords
{
    public class HeadwordsTableProcessor : ProcessorBase
    {
        public HeadwordsTableProcessor(XsltTransformationManager xsltTransformationManager, IKernel container) : base(xsltTransformationManager, container)
        {
        }

        protected override string NodeName
        {
            get { return "headwordsTable"; }
        }

        protected override IEnumerable<ProcessorBase> SubProcessors
        {
            get
            {
                return new List<ProcessorBase>
                {
                    Container.Resolve<HeadwordProcessor>()
                };
            }
        }

        protected override void PreprocessSetup(BookData bookData)
        {
            if (bookData.BookHeadwords == null)
            {
                bookData.BookHeadwords = new List<BookHeadwordData>();
            }
        }
    }
}
