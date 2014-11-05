using System.Collections.Generic;

namespace ITJakub.FileProcessing.Core.Processors.Header
{
    public class EncodingDescProcessor : ProcessorBase
    {
        protected override string NodeName
        {
            get { return "encodingDesc"; }
        }

        protected override IEnumerable<ProcessorBase> SubProcessors
        {
            get { throw new System.NotImplementedException(); }
        }
    }
}