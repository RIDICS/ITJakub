using System.Xml;
using Castle.MicroKernel;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.DataEntities.Database.Repositories;
using ITJakub.FileProcessing.Core.XSLT;

namespace ITJakub.FileProcessing.Core.Processors.Header
{
    public class PublisherProcessor : ListProcessorBase
    {
        private readonly BookVersionRepository m_bookVersionRepository;

        public PublisherProcessor(BookVersionRepository bookVersionRepository, XsltTransformationManager xsltTransformationManager, IKernel container)
            : base(xsltTransformationManager, container)
        {
            m_bookVersionRepository = bookVersionRepository;
        }

        protected override string NodeName
        {
            get { return "publisher"; }
        }

        protected override void ProcessElement(BookVersion bookVersion, XmlReader xmlReader)
        {
            var xmlFragment = new XmlDocument();
            xmlFragment.Load(xmlReader);

            var publisher = new Publisher();

            using (var tempReader = new XmlNodeReader(xmlFragment))
            {
                while (tempReader.Read())
                {
                    if (tempReader.NodeType == XmlNodeType.Element && tempReader.IsStartElement() &&
                        tempReader.LocalName.Equals("email"))
                    {
                        publisher.Email = GetInnerContentAsString(tempReader);
                    }
                }
            }

            using (var tempReader = new XmlNodeReader(xmlFragment))
            {
                publisher.Text = GetInnerContentAsString(tempReader);
            }

            publisher = m_bookVersionRepository.FindPublisherByText(publisher.Text) ?? publisher;
            bookVersion.Publisher = publisher;
        }
    }
}