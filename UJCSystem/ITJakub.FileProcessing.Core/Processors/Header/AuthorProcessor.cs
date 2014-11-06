using System.Collections.Generic;
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
            if (bookVersion.Authors == null)
            {
                bookVersion.Authors = new List<Author>();
            }
            bookVersion.Authors.Add(new Author {Name = GetInnerContentAsString(xmlReader)});
        }
    }
}