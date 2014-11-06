using System.Collections.Generic;
using Castle.MicroKernel;

namespace ITJakub.FileProcessing.Core.Processors.Header
{
    public class TeiHeaderProcessor : ProcessorBase
    {
        public TeiHeaderProcessor(IKernel container) : base(container)
        {
        }

        protected override string NodeName
        {
            get { return "teiHeader"; }
        }

        protected override IEnumerable<ProcessorBase> SubProcessors
        {
            get
            {
                return new List<ProcessorBase>
                {
                    Container.Resolve<FileDescProcessor>(),
                    Container.Resolve<EncodingDescProcessor>(),
                    Container.Resolve<ProfileDescProcessor>(),
                };
            }
        }
    }
}