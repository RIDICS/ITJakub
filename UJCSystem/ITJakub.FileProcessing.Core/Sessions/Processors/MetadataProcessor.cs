using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using ITJakub.Core.Resources;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.DataEntities.Database.Entities.Enums;
using ITJakub.DataEntities.Database.Repositories;
using ITJakub.FileProcessing.Core.XMLProcessing;
using ITJakub.Shared.Contracts.Resources;
using log4net;

namespace ITJakub.FileProcessing.Core.Sessions.Processors
{
    public class MetadataProcessor : IResourceProcessor
    {
        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly CategoryRepository m_categoryRepository;
        private readonly XmlMetadataProcessingManager m_xmlMetadataProcessingManager;

        public MetadataProcessor(XmlMetadataProcessingManager xmlMetadataProcessingManager,
            CategoryRepository categoryRepository)
        {
            m_xmlMetadataProcessingManager = xmlMetadataProcessingManager;
            m_categoryRepository = categoryRepository;
        }

        public void Process(ResourceSessionDirector resourceSessionDirector)
        {

            var metaData = GetMetadataForProcessing(resourceSessionDirector);
            
            var xmlFileStream = File.Open(metaData.FullPath, FileMode.Open);

            BookVersion bookVersion = m_xmlMetadataProcessingManager.GetXmlMetadata(xmlFileStream);

            bookVersion.Description = resourceSessionDirector.GetSessionInfoValue<string>(SessionInfo.Message);
            bookVersion.CreateTime = resourceSessionDirector.GetSessionInfoValue<DateTime>(SessionInfo.CreateTime);

            resourceSessionDirector.SetSessionInfoValue(SessionInfo.BookVersionEntity, bookVersion);
            resourceSessionDirector.SetSessionInfoValue(SessionInfo.BookId, bookVersion.Book.Guid);
            resourceSessionDirector.SetSessionInfoValue(SessionInfo.VersionId, bookVersion.VersionId);

            if (bookVersion.BookPages != null)
            {
                foreach (var page in bookVersion.BookPages)
                {
                    if (!string.IsNullOrWhiteSpace(page.XmlResource))
                    {
                        var pageResource = new Resource
                        {
                            FileName = page.XmlResource,
                            FullPath = Path.Combine(resourceSessionDirector.SessionPath, page.XmlResource),
                            ResourceType = ResourceType.Page
                        };
                        resourceSessionDirector.Resources.Add(pageResource);
                    }
                }
            }
                        
        }

        private Resource GetMetadataForProcessing(ResourceSessionDirector resourceSessionDirector)
        {
            var metaData = resourceSessionDirector.Resources.FirstOrDefault(resource => resource.ResourceType == ResourceType.UploadedMetadata) ??
                           resourceSessionDirector.Resources.FirstOrDefault(resource => resource.ResourceType == ResourceType.ConvertedMetadata);

            if (metaData == null)
                throw new ResourceMissingException("Metadata not found in resources");

            return metaData;
        }

        
    }

 
}