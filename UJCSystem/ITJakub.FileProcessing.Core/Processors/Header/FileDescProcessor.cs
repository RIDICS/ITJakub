using System.Collections.Generic;
using System.Xml;
using Castle.MicroKernel;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.DataEntities.Database.Repositories;
using ITJakub.FileProcessing.Core.XSLT;

namespace ITJakub.FileProcessing.Core.Processors.Header
{
    public class FileDescProcessor : ProcessorBase
    {
        private readonly BookRepository m_bookRepository;

        public FileDescProcessor(BookRepository bookRepository, XsltTransformationManager xsltTransformationManager,
            IKernel container)
            : base(xsltTransformationManager, container)
        {
            m_bookRepository = bookRepository;
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

        protected override void ProcessAttributes(BookVersion bookVersion, XmlReader xmlReader)
        {
            string bookGuid = xmlReader.GetAttribute("n");
            Book book = m_bookRepository.GetBookByGuid(bookGuid) ?? new Book {Guid = bookGuid};
            bookVersion.Book = book;

            string versionId = xmlReader.GetAttribute("version");
            bookVersion.VersionId = versionId;
        }
    }
}