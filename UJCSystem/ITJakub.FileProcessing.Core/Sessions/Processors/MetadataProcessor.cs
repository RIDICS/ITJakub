using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
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

            AddBookAccessories(resourceSessionDirector, bookVersion);
            AddBookPages(resourceSessionDirector, bookVersion);
            
        }

        private void AddBookAccessories(ResourceSessionDirector resourceSessionDirector, BookVersion bookVersion)
        {
            if (bookVersion.Accessories != null)
            {
                foreach (var file in bookVersion.Accessories)
                {
                    if (!string.IsNullOrWhiteSpace(file.FileName))
                    {
                        
                        var accessory = new Resource
                        {
                            FileName = file.FileName,
                            FullPath = Path.Combine(resourceSessionDirector.SessionPath, file.FileName),
                            ResourceType = GetResourceTypeByBookAccessory(file)
                        };

                        if (m_log.IsInfoEnabled)
                            m_log.InfoFormat($"Adding resource: '{accessory.FileName}' as type '{accessory.ResourceType}'");


                        resourceSessionDirector.Resources.Add(accessory);
                    }
                }
            }
        }

        private ResourceType GetResourceTypeByBookAccessory(BookAccessory file)
        {
            switch (file.Type)
            {
                case AccessoryType.Content:
                    return ResourceType.Book;

                case AccessoryType.Cover:
                    return ResourceType.Image;

                case AccessoryType.Bibliography:
                    return ResourceType.BibliographyDocument;

                default:
                    return ResourceType.UnknownResourceFile;
            }
        }

        private static void AddBookPages(ResourceSessionDirector resourceSessionDirector, BookVersion bookVersion)
        {
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