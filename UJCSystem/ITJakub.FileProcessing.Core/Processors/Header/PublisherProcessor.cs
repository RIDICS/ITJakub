using System.Xml;
using Castle.MicroKernel;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.FileProcessing.Core.XSLT;

namespace ITJakub.FileProcessing.Core.Processors.Header
{
    public class PublisherProcessor : ListProcessorBase
    {
        public PublisherProcessor(XsltTransformationManager xsltTransformationManager, IKernel container)
            : base(xsltTransformationManager, container)
        {
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


            bookVersion.Publisher = publisher;
        }
    }
}