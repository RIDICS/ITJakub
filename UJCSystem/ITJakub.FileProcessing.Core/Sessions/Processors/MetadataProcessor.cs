using System.IO;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.FileProcessing.Core.XMLProcessing;

namespace ITJakub.FileProcessing.Core.Sessions.Processors
{
    public class MetadataProcessor
    {
        private readonly XmlMetadataProcessingManager m_xmlMetadataProcessingManager;

        public MetadataProcessor(XmlMetadataProcessingManager xmlMetadataProcessingManager)
        {
            m_xmlMetadataProcessingManager = xmlMetadataProcessingManager;
        }

        public BookVersion Process(Resource metaData)
        {
            FileStream xmlFileStream = File.Open(metaData.FullPath, FileMode.Open);
            return m_xmlMetadataProcessingManager.GetXmlMetadata(xmlFileStream);
        }
    }
}