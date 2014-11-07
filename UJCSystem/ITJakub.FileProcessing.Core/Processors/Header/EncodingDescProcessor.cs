using System.Collections.Generic;
using Castle.MicroKernel;
using ITJakub.FileProcessing.Core.XSLT;

namespace ITJakub.FileProcessing.Core.Processors.Header
{
    public class EncodingDescProcessor : ProcessorBase
    {
        public EncodingDescProcessor(XsltTransformationManager xsltTransformationManager, IKernel container)
            : base(xsltTransformationManager, container)
        {
        }

        protected override string NodeName
        {
            get { return "encodingDesc"; }
        }

        protected override IEnumerable<ProcessorBase> SubProcessors
        {
            get { return new List<ProcessorBase>(); } //TODO
        }
    }
}