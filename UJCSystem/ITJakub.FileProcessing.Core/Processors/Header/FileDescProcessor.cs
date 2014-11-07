using System.Collections.Generic;
using Castle.MicroKernel;
using ITJakub.FileProcessing.Core.XSLT;

namespace ITJakub.FileProcessing.Core.Processors.Header
{
    public class FileDescProcessor : ProcessorBase
    {
        public FileDescProcessor(XsltTransformationManager xsltTransformationManager, IKernel container)
            : base(xsltTransformationManager, container)
        {
        }

        protected override string NodeName
        {
            get { return "fileDesc"; }
        }

        protected override IEnumerable<ProcessorBase> SubProcessors
        {
            get
            {
                return new List<ProcessorBase>
                {
                    Container.Resolve<TitleStmtProcessor>()
                };
            }
        }
    }
}