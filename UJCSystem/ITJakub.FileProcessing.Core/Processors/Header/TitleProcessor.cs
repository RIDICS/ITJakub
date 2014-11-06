using System.Xml;
using Castle.MicroKernel;
using ITJakub.DataEntities.Database.Entities;

namespace ITJakub.FileProcessing.Core.Processors.Header
{
    public class TitleProcessor : ListProcessorBase
    {
        public TitleProcessor(IKernel container) : base(container)
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