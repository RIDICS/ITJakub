using System;
using System.IO;
using System.Linq;
using System.Reflection;
using ITJakub.FileProcessing.Core.Data;
using ITJakub.FileProcessing.Core.XMLProcessing;
using log4net;
using Vokabular.Core.Storage.Resources;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.Shared.DataContracts.Types;

namespace ITJakub.FileProcessing.Core.Sessions.Processors
{
    public class MetadataProcessor : IResourceProcessor
    {
        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly XmlMetadataProcessingManager m_xmlMetadataProcessingManager;

        public MetadataProcessor(XmlMetadataProcessingManager xmlMetadataProcessingManager)
        {
            m_xmlMetadataProcessingManager = xmlMetadataProcessingManager;
        }

        public void Process(ResourceSessionDirector resourceSessionDirector)
        {
            var metaData = GetMetadataForProcessing(resourceSessionDirector);

            var xmlFileStream = File.Open(metaData.FullPath, FileMode.Open);

            BookData bookData = m_xmlMetadataProcessingManager.GetXmlMetadata(xmlFileStream);

            bookData.VersionDescription = resourceSessionDirector.GetSessionInfoValue<string>(SessionInfo.Message);
            bookData.CreateTime = resourceSessionDirector.GetSessionInfoValue<DateTime>(SessionInfo.CreateTime);

            resourceSessionDirector.SetSessionInfoValue(SessionInfo.BookData, bookData);
            resourceSessionDirector.SetSessionInfoValue(SessionInfo.BookXmlId, bookData.BookXmlId);
            resourceSessionDirector.SetSessionInfoValue(SessionInfo.VersionXmlId, bookData.VersionXmlId);

            AddBookAccessories(resourceSessionDirector, bookData);
            AddBookPages(resourceSessionDirector, bookData);
            
        }

        private void AddBookAccessories(ResourceSessionDirector resourceSessionDirector, BookData bookData)
        {
            if (bookData.Accessories != null)
            {
                foreach (var file in bookData.Accessories)
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

        private ResourceType GetResourceTypeByBookAccessory(BookAccessoryData file)
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

        private static void AddBookPages(ResourceSessionDirector resourceSessionDirector, BookData bookData)
        {
            if (bookData.Pages != null)
            {
                foreach (var page in bookData.Pages)
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