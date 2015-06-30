using System.Collections.Generic;
using Castle.MicroKernel;
using ITJakub.DataEntities.Database.Entities;
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

        protected override void PreprocessSetup(BookVersion bookVersion)
        {
            if (bookVersion.BookHeadwords == null)
            {
                bookVersion.BookHeadwords = new List<BookHeadword>();
            }
        }
    }
}
