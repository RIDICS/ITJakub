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
            m_xmlProcessingmanager.TextXml();
        }
    }
}