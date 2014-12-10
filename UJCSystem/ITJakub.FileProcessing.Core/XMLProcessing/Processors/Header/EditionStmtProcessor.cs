using System.Collections.Generic;
using Castle.MicroKernel;
using ITJakub.FileProcessing.Core.XMLProcessing.XSLT;

namespace ITJakub.FileProcessing.Core.XMLProcessing.Processors.Header
{
    public class EditionStmtProcessor : ProcessorBase
    {
        public EditionStmtProcessor(XsltTransformationManager xsltTransformationManager, IKernel container)
            : base(xsltTransformationManager, container)
        {
        }

        protected override string NodeName
        {
            get { return "editionStmt"; }
        }

        protected override IEnumerable<ProcessorBase> SubProcessors
        {
            get
            {
                return new List<ProcessorBase>
                {
                    Container.Resolve<EditionProcessor>(),
                    Container.Resolve<RespStmtProcessor>(),
                };
            }
        }
    }
}