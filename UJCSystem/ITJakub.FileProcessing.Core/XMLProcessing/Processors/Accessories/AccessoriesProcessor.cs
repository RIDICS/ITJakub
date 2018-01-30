using System.Collections.Generic;
using Castle.MicroKernel;
using ITJakub.FileProcessing.Core.Data;
using ITJakub.FileProcessing.Core.XMLProcessing.XSLT;

namespace ITJakub.FileProcessing.Core.XMLProcessing.Processors.Accessories
{
    public class AccessoriesProcessor : ProcessorBase
    {
        public AccessoriesProcessor(XsltTransformationManager xsltTransformationManager, IKernel container) : base(xsltTransformationManager, container)
        {
        }

        protected override string NodeName
        {
            get { return "accessories"; }
        }

        protected override IEnumerable<ProcessorBase> SubProcessors
        {
            get
            {
                return new List<ProcessorBase>
                {
                    Container.Resolve<FileProcessor>()
                };
            }
        }

        protected override void PreprocessSetup(BookData bookData)
        {
            if (bookData.Accessories == null)
            {
                bookData.Accessories = new List<BookAccessoryData>();
            }
        }
    }
}