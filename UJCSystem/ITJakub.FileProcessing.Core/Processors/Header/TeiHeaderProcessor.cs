using System.Collections.Generic;

namespace ITJakub.FileProcessing.Core.Processors.Header
{
    public class TeiHeaderProcessor : ProcessorBase
    {
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
                    new FileDescProcessor(),
                    new EncodingDescProcessor(),
                    new ProfileDescProcessor(),

                };
            }
        }
    }
}