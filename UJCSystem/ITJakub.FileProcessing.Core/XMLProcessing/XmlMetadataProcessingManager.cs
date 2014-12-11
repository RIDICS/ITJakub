using System.IO;
using System.Xml;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.FileProcessing.Core.XMLProcessing.Processors;

namespace ITJakub.FileProcessing.Core.XMLProcessing
{
    public class XmlMetadataProcessingManager
    {
        private readonly DocumentProcessor m_documentProcessor;

        public XmlMetadataProcessingManager(DocumentProcessor documentProcessor)
        {
            m_documentProcessor = documentProcessor;
        }

        public BookVersion GetXmlMetadata(Stream xlmData)
        {
            var bookVersion = new BookVersion();
            using (var xmlReader = new XmlTextReader(xlmData) { WhitespaceHandling = WhitespaceHandling.None })
            {
                while (xmlReader.Read())
                {
                    if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.IsStartElement() &&
                        xmlReader.LocalName.Equals(m_documentProcessor.XmlRootName))
                    {
                        m_documentProcessor.Process(bookVersion, xmlReader);
                    }
                }
            }
            return bookVersion;
        }
    }
}