using System.Collections.Generic;
using Castle.MicroKernel;
using ITJakub.FileProcessing.Core.XMLProcessing.XSLT;

namespace ITJakub.FileProcessing.Core.XMLProcessing.Processors.Header
{
    public class FileDescProcessor : ProcessorBase
    {

        public FileDescProcessor( XsltTransformationManager xsltTransformationManager,
            IKernel container)
            : base(xsltTransformationManager, container)
        {
        }

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
                    Container.Resolve<TitleStmtProcessor>(),
                    Container.Resolve<EditionStmtProcessor>(),
                    Container.Resolve<PublicationStmtProcessor>(),
                    Container.Resolve<SourceDescProcessor>(),
                };
            }
        }
    }
}