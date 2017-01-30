using System.Collections.Generic;
using Castle.MicroKernel;
using ITJakub.FileProcessing.Core.Data;
using ITJakub.FileProcessing.Core.XMLProcessing.XSLT;

namespace ITJakub.FileProcessing.Core.XMLProcessing.Processors.Header
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

        protected override void PreprocessSetup(BookData bookData)
        {
            if (bookData.Keywords == null)
            {
                bookData.Keywords = new List<string>();
            }

            if (bookData.LiteraryOriginals == null)
            {
                bookData.LiteraryOriginals = new List<string>();
            }

            if (bookData.LiteraryKinds == null)
            {
                bookData.LiteraryKinds = new List<string>();
            }

            if (bookData.LiteraryGenres == null)
            {
                bookData.LiteraryGenres = new List<string>();
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