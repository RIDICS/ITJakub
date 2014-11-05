using System.Collections.Generic;

namespace ITJakub.FileProcessing.Core.Processors.Header
{
    public class ProfileDescProcessor : ProcessorBase
    {
        protected override string NodeName
        {
            get { return "profileDesc"; }
        }

        protected override IEnumerable<ProcessorBase> SubProcessors
        {
            get { throw new System.NotImplementedException(); }
        }
    }
}