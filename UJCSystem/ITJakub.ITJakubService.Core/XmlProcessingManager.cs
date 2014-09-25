using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using ITJakub.ITJakubService.DataContracts;

namespace ITJakub.ITJakubService.Core
{
    public class XmlProcessingManager
    {
        private const string HeaderElement = "teiHeader";
        private readonly XNamespace m_teiNamespace = "http://www.tei-c.org/ns/1.0";

        public ProcessedFileInfoContract GetInfoFromHeader(XDocument header)
        {
            return new ProcessedFileInfoContract
            {
                FileGuid = header.Root.Element(m_teiNamespace + "fileDesc").Attribute("n").Value,
                Name = string.Join(" ", header.Root.Descendants(m_teiNamespace + "title").First()
                    .Descendants(m_teiNamespace + "w")
                    .Select(x => x.Value)
                    .ToArray()),
                Author = string.Join(" ", header.Root.Descendants(m_teiNamespace + "author").First()
                    .Descendants(m_teiNamespace + "w")
                    .Select(x => x.Value)
                    .ToArray()) //TODO rewrite for list of authors
            };
        }

        public XDocument ParseHeader(FileStream fileStream)
        {
            var xmlTextReader = new XmlTextReader(fileStream)
            {
                WhitespaceHandling = WhitespaceHandling.None
            };

            while (xmlTextReader.Read())
            {
                if (xmlTextReader.NodeType == XmlNodeType.Element &&
                    xmlTextReader.LocalName == HeaderElement &&
                    xmlTextReader.IsStartElement())
                {
                    return XDocument.Parse(xmlTextReader.ReadOuterXml());
                }
            }
            return null;
        }
    }
}