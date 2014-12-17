using System;
using System.IO;
using System.Linq;
using ITJakub.Core.Resources;
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
            var metaData = resourceSessionDirector.Resources.FirstOrDefault( resource => resource.ResourceType == ResourceTypeEnum.Metadata);
            if (metaData == null)
                throw new ResourceMissingException("Metada not found in resources");
            var xmlFileStream = File.Open(metaData.FullPath, FileMode.Open);

            var bookVersion = m_xmlMetadataProcessingManager.GetXmlMetadata(xmlFileStream);

            bookVersion.Description = resourceSessionDirector.GetSessionInfoValue<string>(SessionInfo.Message);
            bookVersion.CreateTime = resourceSessionDirector.GetSessionInfoValue<DateTime>(SessionInfo.CreateTime);

            resourceSessionDirector.SetSessionInfoValue(SessionInfo.BookVersionEntity, bookVersion);
            resourceSessionDirector.SetSessionInfoValue(SessionInfo.BookId, bookVersion.Book.Guid);
            resourceSessionDirector.SetSessionInfoValue(SessionInfo.VersionId, bookVersion.VersionId);

            foreach (var page in bookVersion.BookPages)
            {
                var pageResource = new Resource
                {
                    FileName = page.XmlResource,
                    FullPath = Path.Combine(resourceSessionDirector.SessionPath, page.XmlResource),
                    ResourceType = ResourceTypeEnum.Page
                };
                resourceSessionDirector.Resources.Add(pageResource);
            }
        }
    }
}