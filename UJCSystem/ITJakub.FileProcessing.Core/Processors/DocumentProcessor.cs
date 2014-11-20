using System.Collections.Generic;
using Castle.MicroKernel;
using ITJakub.FileProcessing.Core.Processors.Header;
using ITJakub.FileProcessing.Core.Processors.Text;
using ITJakub.FileProcessing.Core.XSLT;

namespace ITJakub.FileProcessing.Core.Processors
{
    public class DocumentProcessor : ProcessorBase
    {
        public DocumentProcessor(XsltTransformationManager xsltTransformationManager, IKernel container)
            : base(xsltTransformationManager, container)
        {
        }

        protected override string NodeName
        {
            get { return "TEI"; }
        }

        public string XmlRootName
        {
            get { return NodeName; }
        }


        protected override IEnumerable<ProcessorBase> SubProcessors
        {
            get
            {
                return new List<ProcessorBase>
                {
                    Container.Resolve<TeiHeaderProcessor>(),
                    Container.Resolve<TextProcessor>(),
                };
            }
        }
    }
}