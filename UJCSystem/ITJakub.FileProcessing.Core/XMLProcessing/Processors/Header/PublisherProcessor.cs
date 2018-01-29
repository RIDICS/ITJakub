using System.Xml;
using Castle.MicroKernel;
using ITJakub.FileProcessing.Core.Data;
using ITJakub.FileProcessing.Core.XMLProcessing.XSLT;

namespace ITJakub.FileProcessing.Core.XMLProcessing.Processors.Header
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

        protected override void ProcessElement(BookData bookData, XmlReader xmlReader)
        {
            var xmlFragment = new XmlDocument();
            xmlFragment.Load(xmlReader);

            var publisher = new PublisherData();

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

            bookData.Publisher = publisher;
        }
    }
}