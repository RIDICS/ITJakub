using System;
using System.Collections.Generic;
using System.Xml;
using Castle.MicroKernel;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.DataEntities.Database.Entities.Enums;
using ITJakub.DataEntities.Database.Repositories;
using ITJakub.FileProcessing.Core.XMLProcessing.Processors.BookContent;
using ITJakub.FileProcessing.Core.XMLProcessing.Processors.Header;
using ITJakub.FileProcessing.Core.XMLProcessing.XSLT;

namespace ITJakub.FileProcessing.Core.XMLProcessing.Processors
{
    public class DocumentProcessor : ProcessorBase
    {
        private readonly BookRepository m_bookRepository;

        public DocumentProcessor(BookRepository bookRepository, XsltTransformationManager xsltTransformationManager,
            IKernel container)
            : base(xsltTransformationManager, container)
        {
            m_bookRepository = bookRepository;
        }

        protected override string NodeName
        {
            get { return "DocDescription"; }
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
                    Container.Resolve<BookContentProcessor>(),
                };
            }
        }

        protected override void ProcessAttributes(BookVersion bookVersion, XmlReader xmlReader)
        {
            string bookGuid = xmlReader.GetAttribute("n");
            Book book = m_bookRepository.GetBookByGuid(bookGuid) ?? new Book {Guid = bookGuid};

            string docType = xmlReader.GetAttribute("doctype");
            BookTypeEnum bookTypeEnum;
            Enum.TryParse(docType, true, out bookTypeEnum);
            var bookType = m_bookRepository.FindBookType(bookTypeEnum) ?? new BookType {Type = bookTypeEnum};

            book.BookType = bookType;
            bookVersion.Book = book;

            string versionId = xmlReader.GetAttribute("versionId");
            bookVersion.VersionId = versionId;
        }
    }
}