using System.Collections.Generic;

namespace ITJakub.FileProcessing.Core.Processors.Header
{
    public class TitleStmtProcessor : ProcessorBase
    {
        protected override string NodeName
        {
            get { return "titleStmt"; }
        }

        protected override IEnumerable<ProcessorBase> SubProcessors
        {
            get
            {
                return new List<ProcessorBase>
                {
                    new TitleProcessor(),
                    new AuthorProcessor()
                };
            }
        }
    }
}