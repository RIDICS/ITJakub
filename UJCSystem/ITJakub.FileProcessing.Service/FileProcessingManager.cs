using System.IO;
using ITJakub.DataEntities.Database.Repositories;
using ITJakub.FileProcessing.Core;
using ITJakub.FileProcessing.Core.XMLProcessing;

namespace ITJakub.FileProcessing.Service
{
    public class FileProcessingManager
    {
        private readonly BookVersionRepository m_bookVersionRepository; 
        private readonly XmlProcessingManager m_xmlProcessingmanager;

        public FileProcessingManager(BookVersionRepository bookVersionRepository, XmlProcessingManager xmlProcessingManager)
        {
            m_bookVersionRepository = bookVersionRepository;
            m_xmlProcessingmanager = xmlProcessingManager;
        }

        public void TestXml()
        {
            FileStream xmlFileStream = File.Open("D:\\ITJakubTestXml\\CerKal.xml", FileMode.Open);
            var bookVersion = m_xmlProcessingmanager.GetXmlMetadata(xmlFileStream);
            var bookVersionId = m_bookVersionRepository.Create(bookVersion);
        }
    }
}