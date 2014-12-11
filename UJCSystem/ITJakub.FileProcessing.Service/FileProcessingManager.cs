using ITJakub.DataEntities.Database.Repositories;
using ITJakub.FileProcessing.Core.XMLProcessing;

namespace ITJakub.FileProcessing.Service
{
    public class FileProcessingManager
    {
        private readonly BookVersionRepository m_bookVersionRepository; 
        private readonly XmlMetadataProcessingManager m_xmlMetadataProcessingmanager;

        public FileProcessingManager(BookVersionRepository bookVersionRepository, XmlMetadataProcessingManager xmlMetadataProcessingManager)
        {
            m_bookVersionRepository = bookVersionRepository;
            m_xmlMetadataProcessingmanager = xmlMetadataProcessingManager;
        }
    }
}