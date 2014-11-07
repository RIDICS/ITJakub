using System.Collections.Generic;
using System.Xml;
using Castle.MicroKernel;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.DataEntities.Database.Repositories;
using ITJakub.FileProcessing.Core.Processors.Header;
using ITJakub.FileProcessing.Core.Processors.Text;
using ITJakub.FileProcessing.Core.XSLT;

namespace ITJakub.FileProcessing.Core.Processors
{
    public class DocumentProcessor : ProcessorBase
    {
        private readonly BookRepository m_bookRepository;

        public DocumentProcessor(BookRepository bookRepository, XsltTransformationManager xsltTransformationManager, IKernel container)
            : base(xsltTransformationManager, container)
        {
            m_bookRepository = bookRepository;
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

        protected override void ProcessAttributes(BookVersion bookVersion, XmlReader xmlReader)
        {
            var bookGuid = xmlReader.GetAttribute("n");
            var book = m_bookRepository.GetBookByGuid(bookGuid);
            if (book == null)
            {
                book = new Book {Guid = bookGuid};
            }
            bookVersion.Book = book;
        }
    }
}