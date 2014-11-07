using System.Collections.Generic;
using Castle.MicroKernel;
using ITJakub.FileProcessing.Core.XSLT;

namespace ITJakub.FileProcessing.Core.Processors.Text
{
    public class TextProcessor : ProcessorBase
    {
        public TextProcessor(XsltTransformationManager xsltTransformationManager, IKernel container)
            : base(xsltTransformationManager, container)
        {
        }

        protected override string NodeName
        {
            get { return "text"; }
        }

        protected override IEnumerable<ProcessorBase> SubProcessors
        {
            get { return new List<ProcessorBase>(); }
        }
    }
}