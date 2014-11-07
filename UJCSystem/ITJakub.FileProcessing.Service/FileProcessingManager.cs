using System.IO;
using ITJakub.FileProcessing.Core;

namespace ITJakub.FileProcessing.Service
{
    public class FileProcessingManager
    {
        private readonly XmlProcessingManager m_xmlProcessingmanager;

        public FileProcessingManager(XmlProcessingManager xmlProcessingManager)
        {
            m_xmlProcessingmanager = xmlProcessingManager;
        }

        public void TestXml()
        {
            FileStream xmlFileStream = File.Open("D:\\ITJakubTestXml\\LekChir.xml", FileMode.Open);
            var bookVersion = m_xmlProcessingmanager.GetXmlMetadata(xmlFileStream);
        }
    }
}