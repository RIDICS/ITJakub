using System.Collections.Generic;

namespace ITJakub.FileProcessing.Core.Processors.Header
{
    public class FileDescProcessor : ProcessorBase
    {
        protected override string NodeName
        {
            get { return "fileDesc"; }
        }

        protected override IEnumerable<ProcessorBase> SubProcessors
        {
            get
            {
                return new List<ProcessorBase>
                {
                    new TitleStmtProcessor()
                };
            }
        }
    }
}