using System.Collections.Generic;
using Castle.MicroKernel;
using ITJakub.FileProcessing.Core.XSLT;

namespace ITJakub.FileProcessing.Core.Processors
{
    public abstract class ListProcessorBase : ProcessorBase
    {
        protected ListProcessorBase(XsltTransformationManager xsltTransformationManager, IKernel container)
            : base(xsltTransformationManager, container)
        {
        }

        protected override sealed IEnumerable<ProcessorBase> SubProcessors
        {
            get { return new List<ProcessorBase>(); }
        }
    }
}