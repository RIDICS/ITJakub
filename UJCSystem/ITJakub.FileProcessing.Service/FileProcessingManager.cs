using System.IO;
using ITJakub.DataEntities.Database.Repositories;
using ITJakub.FileProcessing.Core;

namespace ITJakub.FileProcessing.Service
{
    public class FileProcessingManager
    {
        private readonly BookVersionRepository m_bookVersionRepository; //TODO just for testing purpose
        private readonly XmlProcessingManager m_xmlProcessingmanager;

        public FileProcessingManager(BookVersionRepository bookVersionRepository, XmlProcessingManager xmlProcessingManager)
        {
            m_bookVersionRepository = bookVersionRepository;
            m_xmlProcessingmanager = xmlProcessingManager;
        }

        public void TestXml()
        {
            FileStream xmlFileStream = File.Open("D:\\ITJakubTestXml\\LekJadroBrn.xml", FileMode.Open);
            var bookVersion = m_xmlProcessingmanager.GetXmlMetadata(xmlFileStream);
            bookVersion.VersionId = "verId"; //TODO get from xml in GetXmlMetadata
            var bookVersionId = m_bookVersionRepository.Create(bookVersion);
        }
    }
}