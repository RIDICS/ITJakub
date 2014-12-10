using System.Collections.Generic;
using System.Xml;
using Castle.MicroKernel;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.DataEntities.Database.Repositories;
using ITJakub.FileProcessing.Core.XMLProcessing.XSLT;

namespace ITJakub.FileProcessing.Core.XMLProcessing.Processors.Header
{
    public class AuthorProcessor : ListProcessorBase
    {
        private readonly AuthorRepository m_authorRepository;

        public AuthorProcessor(AuthorRepository authorRepository, XsltTransformationManager xsltTransformationManager,
            IKernel container)
            : base(xsltTransformationManager, container)
        {
            m_authorRepository = authorRepository;
        }

        protected override string NodeName
        {
            get { return "author"; }
        }

        protected override void PreprocessSetup(BookVersion bookVersion)
        {
            if (bookVersion.Authors == null)
            {
                bookVersion.Authors = new List<Author>();
            }
        }

        protected override void ProcessElement(BookVersion bookVersion, XmlReader xmlReader)
        {
            string name = GetInnerContentAsString(xmlReader);
            Author author = m_authorRepository.FindByName(name) ?? new Author {Name = name};
            bookVersion.Authors.Add(author);
        }
    }
}