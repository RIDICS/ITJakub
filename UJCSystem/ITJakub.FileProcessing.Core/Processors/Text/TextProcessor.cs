using System.Collections.Generic;
using Castle.MicroKernel;

namespace ITJakub.FileProcessing.Core.Processors.Text
{
    public class TextProcessor : ProcessorBase
    {
        public TextProcessor(IKernel container) : base(container)
        {
        }

        protected override string NodeName
        {
            get { return "text"; }
        }

        protected override IEnumerable<ProcessorBase> SubProcessors
        {
            get
            {
                return new List<ProcessorBase>
                {
                };
            }
        }
    }
}