using System.Collections.Generic;
using Castle.MicroKernel;
using ITJakub.FileProcessing.Core.Data;
using ITJakub.FileProcessing.Core.XMLProcessing.XSLT;

namespace ITJakub.FileProcessing.Core.XMLProcessing.Processors.Pages
{
    public class PagesProcessor : ProcessorBase
    {
        public PagesProcessor(XsltTransformationManager xsltTransformationManager, IKernel container)
            : base(xsltTransformationManager, container)
        {
        }

        protected override string NodeName
        {
            get { return "pages"; }
        }

        protected override IEnumerable<ProcessorBase> SubProcessors
        {
            get
            {
                return new List<ProcessorBase>
                {
                    Container.Resolve<PageProcessor>()
                };
            }
        }

        protected override void PreprocessSetup(BookData bookData)
        {
            if (bookData.Pages == null)
            {
                bookData.Pages = new List<BookPageData>();
            }
        }
    }
}
