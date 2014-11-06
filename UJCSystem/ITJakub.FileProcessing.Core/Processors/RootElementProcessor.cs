using System.Collections.Generic;
using System.Xml;
using Castle.MicroKernel;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.DataEntities.Database.Repositories;
using ITJakub.FileProcessing.Core.Processors.Header;
using ITJakub.FileProcessing.Core.Processors.Text;

namespace ITJakub.FileProcessing.Core.Processors
{
    public class RootElementProcessor : ProcessorBase
    {
        private readonly BookRepository m_bookRepository;

        public RootElementProcessor(BookRepository bookRepository, IKernel container) : base(container)
        {
            m_bookRepository = bookRepository;
        }

        protected override string NodeName
        {
            get { return "TEI"; }
        }

        protected override void ProcessAttributes(BookVersion bookVersion, XmlReader xmlReader)
        {
            var bookGuid = xmlReader.GetAttribute("n");
            bookVersion.Book = m_bookRepository.GetBookByGuid(bookGuid);
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