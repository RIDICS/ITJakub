using System.IO;
using System.Xml;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.FileProcessing.Core.Processors;

namespace ITJakub.FileProcessing.Core
{
    public class XmlProcessingManager
    {
        private readonly DocumentProcessor m_documentProcessor;

        public XmlProcessingManager(DocumentProcessor documentProcessor)
        {
            m_documentProcessor = documentProcessor;
        }

        public void TextXml()
        {
            var bookVersion = new BookVersion();
            FileStream xmlFileStream = File.Open("D:\\ITJakubTestXml\\Albrecht.xml", FileMode.Open);
            using (var xmlReader = new XmlTextReader(xmlFileStream) {WhitespaceHandling = WhitespaceHandling.None})
            {
                while (xmlReader.Read())
                {
                    if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.IsStartElement() &&
                        m_documentProcessor.XmlRootName.Equals(xmlReader.LocalName))
                    {
                        m_documentProcessor.Process(bookVersion, xmlReader);
                    }
                }
            }
        }
    }
}