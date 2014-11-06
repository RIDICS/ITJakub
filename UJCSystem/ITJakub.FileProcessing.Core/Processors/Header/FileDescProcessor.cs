using System.Collections.Generic;
using Castle.MicroKernel;

namespace ITJakub.FileProcessing.Core.Processors.Header
{
    public class FileDescProcessor : ProcessorBase
    {
        public FileDescProcessor(IKernel container) : base(container)
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