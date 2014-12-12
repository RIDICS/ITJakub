using System.Collections.Generic;
using System.IO;
using System.Linq;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.FileProcessing.Core.Sessions.Resources;
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

        public BookVersion Process(IEnumerable<Resource> resources)
        {
            var metaData = resources.FirstOrDefault(resource => resource.ResourceType == ResourceTypeEnum.Metadata);
            if (metaData == null)
                throw new ResourceMissingException("Metada not found in resources");
            FileStream xmlFileStream = File.Open(metaData.FullPath, FileMode.Open);
            return m_xmlMetadataProcessingManager.GetXmlMetadata(xmlFileStream);
        }
    }
}