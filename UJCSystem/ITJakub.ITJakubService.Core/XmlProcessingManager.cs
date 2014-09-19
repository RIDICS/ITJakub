using System;
using System.IO;
using System.Xml;
using ITJakub.ITJakubService.DataContracts;

namespace ITJakub.ITJakubService.Core
{
    public class XmlProcessingManager
    {
        private const string HeaderElement = "teiHeader";

        public ProcessedFileInfoContract GetInfoFromHeader(XmlDocument header)
        {
            throw new NotImplementedException();
        }

        public XmlDocument ParseHeader(FileStream fileStream)
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
                    var header = new XmlDocument();
                    header.LoadXml(xmlTextReader.ReadOuterXml());
                    return header;
                }
            }
            return null;
        }
    }
}