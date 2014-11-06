using System.Collections.Generic;
using Castle.MicroKernel;

namespace ITJakub.FileProcessing.Core.Processors.Header
{
    public class EncodingDescProcessor : ProcessorBase
    {
        public EncodingDescProcessor(IKernel container) : base(container)
        {
        }

        protected override string NodeName
        {
            get { return "encodingDesc"; }
        }

        protected override IEnumerable<ProcessorBase> SubProcessors
        {
            get { return new List<ProcessorBase>();  } //TODO
        }
    }
}