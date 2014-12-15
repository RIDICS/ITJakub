using System;
using System.IO;
using System.Linq;
using ITJakub.Core.Resources;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.FileProcessing.Core.XMLProcessing;
using ITJakub.Shared.Contracts.Resources;

namespace ITJakub.FileProcessing.Core.Sessions.Processors
{
    public class MetadataProcessor : IResourceProcessor
    {
        private readonly XmlMetadataProcessingManager m_xmlMetadataProcessingManager;

        public MetadataProcessor(XmlMetadataProcessingManager xmlMetadataProcessingManager)
        {
            m_xmlMetadataProcessingManager = xmlMetadataProcessingManager;
        }

        public void Process(ResourceSessionDirector resourceSessionDirector)
        {
            Resource metaData =
                resourceSessionDirector.Resources.FirstOrDefault(
                    resource => resource.ResourceType == ResourceTypeEnum.Metadata);
            if (metaData == null)
                throw new ResourceMissingException("Metada not found in resources");
            FileStream xmlFileStream = File.Open(metaData.FullPath, FileMode.Open);

            BookVersion bookVersion = m_xmlMetadataProcessingManager.GetXmlMetadata(xmlFileStream);
            
            bookVersion.Description = resourceSessionDirector.GetSessionInfoValue<string>(SessionInfo.Message);
            bookVersion.CreateTime = resourceSessionDirector.GetSessionInfoValue<DateTime>(SessionInfo.CreateTime);
            
            resourceSessionDirector.SetSessionInfoValue(SessionInfo.BookVersionEntity, bookVersion);
        }
    }
}