using System.Xml;
using Castle.MicroKernel;
using ITJakub.DataEntities.Database.Entities;

namespace ITJakub.FileProcessing.Core.Processors.Header
{
    public class AuthorProcessor : ListProcessorBase
    {
        public AuthorProcessor(IKernel container) : base(container)
        {
        }

        protected override string NodeName
        {
            get { return "author"; }
        }

        protected override void ProcessElement(BookVersion bookVersion, XmlReader xmlReader)
        {
            bookVersion.Authors.Add(new Author {Name = GetInnerContentAsString(xmlReader.ReadSubtree())});
        }
    }
}