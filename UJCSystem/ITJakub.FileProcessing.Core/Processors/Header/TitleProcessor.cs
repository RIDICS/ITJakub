using System.Xml;
using Castle.MicroKernel;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.FileProcessing.Core.XSLT;

namespace ITJakub.FileProcessing.Core.Processors.Header
{
    public class TitleProcessor : ListProcessorBase
    {
        public TitleProcessor(XsltTransformationManager xsltTransformationManager, IKernel container)
            : base(xsltTransformationManager, container)
        {
        }

        protected override string NodeName
        {
            get { return "title"; }
        }

        protected override void ProcessElement(BookVersion bookVersion, XmlReader xmlReader)
        {
            bookVersion.Name = GetInnerContentAsString(xmlReader);
        }
    }
}