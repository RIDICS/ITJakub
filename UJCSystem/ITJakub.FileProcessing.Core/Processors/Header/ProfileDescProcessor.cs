using System.Collections.Generic;
using Castle.MicroKernel;

namespace ITJakub.FileProcessing.Core.Processors.Header
{
    public class ProfileDescProcessor : ProcessorBase
    {
        public ProfileDescProcessor(IKernel container) : base(container)
        {
        }

        protected override string NodeName
        {
            get { return "profileDesc"; }
        }

        protected override IEnumerable<ProcessorBase> SubProcessors
        {
            get { return new List<ProcessorBase>(); } //TODO
        }
    }
}