﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ITJakub.FileProcessing.Core.Data;
using ITJakub.FileProcessing.Core.Sessions.Works.Helpers;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.Repositories;

namespace ITJakub.FileProcessing.Core.Sessions.Works.SaveNewBook
{
    public class UpdatePagesSubtask
    {
        private readonly string DefaultImportResourceGroupName = "IMPORT";
        private readonly ResourceRepository m_resourceRepository;
        private List<long> m_allImportedResourceVersionIds;

        public UpdatePagesSubtask(ResourceRepository resourceRepository)
        {
            m_resourceRepository = resourceRepository;
        }

        public List<PageResource> ResultPageResourceList { get; private set; }

        public List<long> ImportedResourceVersionIds => m_allImportedResourceVersionIds;

        public void UpdatePages(long projectId, long bookVersionId, int userId, BookData bookData, Dictionary<string, Term> dbTermCache)
        {
            m_allImportedResourceVersionIds = new List<long>();
            if (bookData.Pages == null)
                return;

            var updatedPageResourceIds = new HashSet<long>();
            var newPageTextResources = new List<NewPageTextData>();
            var newPageImageResources = new List<ImageResource>();
            var resultPageResourceList = new List<PageResource>();

            var now = DateTime.UtcNow;
            var project = m_resourceRepository.Load<Project>(projectId);
            var user = m_resourceRepository.Load<User>(userId);
            var bookVersion = m_resourceRepository.Load<BookVersionResource>(bookVersionId);
            var dbPages = m_resourceRepository.GetProjectLatestPages(projectId);
            var dbPagesDict = dbPages.ToDictionaryMultipleValues(x => x.Name);

            // Update page list
            foreach (var page in bookData.Pages)
            {
                List<PageResource> dbPageResources;
                PageResource dbPageResource = null;
                if (dbPagesDict.TryGetValue(page.Text, out dbPageResources))
                {
                    dbPageResource = dbPageResources.Count == 1
                        ? dbPageResources.First()
                        : dbPageResources.FirstOrDefault(x => x.Position == page.Position); // If multiple pages have the same name, try identify page by Position
                }

                var newPageResource = new PageResource
                {
                    Resource = null,
                    Name = page.Text,
                    Comment = string.Empty,
                    CreateTime = now,
                    CreatedByUser = user,
                    Position = page.Position,
                    VersionNumber = 0,
                    Terms = PrepareTermList(page.TermXmlIds, dbTermCache),
                };

                if (dbPageResource == null)
                {
                    newPageResource.Resource = new Resource
                    {
                        Project = project,
                        Name = page.Text,
                        ContentType = ContentTypeEnum.Page,
                        ResourceType = ResourceTypeEnum.Page,
                    };
                    newPageResource.VersionNumber = 1;
                }
                else
                {
                    newPageResource.Resource = dbPageResource.Resource;
                    newPageResource.VersionNumber = dbPageResource.VersionNumber + 1;

                    updatedPageResourceIds.Add(dbPageResource.Id);
                }

                newPageResource.Resource.LatestVersion = newPageResource;
                
                m_resourceRepository.Create(newPageResource);

                
                // Create TextResource
                if (!string.IsNullOrEmpty(page.XmlId))
                {
                    var newTextResource = new TextResource
                    {
                        Resource = null,
                        Comment = string.Empty,
                        CreateTime = now,
                        CreatedByUser = user,
                        ExternalId = page.XmlId,
                        ResourcePage = newPageResource.Resource,
                        BookVersion = bookVersion,
                        VersionNumber = 0
                    };
                    newPageTextResources.Add(new NewPageTextData
                    {
                        NewTextResource = newTextResource,
                        BookPageData = page,
                    });
                }

                // Create ImageResource
                if (!string.IsNullOrEmpty(page.Image))
                {
                    var imageMimeType = MimeMapping.GetMimeMapping(page.Image);
                    bookData.FileNameMapping.TryGetValue(page.Image, out var fileInfo);
                    var newImageResource = new ImageResource
                    {
                        Resource = null,
                        Comment = string.Empty,
                        CreateTime = now,
                        CreatedByUser = user,
                        FileName = page.Image,
                        FileId = fileInfo?.NewNameInStorage,
                        MimeType = imageMimeType,
                        Size = fileInfo != null ? fileInfo.NewFileSize : 0L,
                        ResourcePage = newPageResource.Resource,
                        VersionNumber = 0
                    };
                    newPageImageResources.Add(newImageResource);
                }

                m_allImportedResourceVersionIds.Add(newPageResource.Id);
                resultPageResourceList.Add(newPageResource);
            }

            ResultPageResourceList = resultPageResourceList;

            // Remove unused pages
            var unusedDbPages = dbPages.Where(x => !updatedPageResourceIds.Contains(x.Id));
            foreach (var unusedDbPage in unusedDbPages)
            {
                var resourceToRemove = unusedDbPage.Resource;
                resourceToRemove.IsRemoved = true;
                m_resourceRepository.Update(resourceToRemove);

                // Related resources are remove in own subtask
            }

            UpdateTextResources(project, newPageTextResources);

            UpdateImageResources(project, newPageImageResources);
        }
        
        private IList<Term> PrepareTermList(List<string> pageTermXmlIds, Dictionary<string, Term> dbTermCache)
        {
            return pageTermXmlIds?.Select(termXmlId => dbTermCache[termXmlId]).ToList();
        }

        private void UpdateTextResources(Project project, IList<NewPageTextData> newPageTextResources)
        {
            if (newPageTextResources.Count == 0)
            {
                return;
            }

            var projectId = project.Id;
            var resourceGroup = GetOrCreateNamedResourceGroup(projectId, TextTypeEnum.Transcribed, DefaultImportResourceGroupName);
            var dbTexts = m_resourceRepository.GetProjectLatestTexts(projectId, resourceGroup.Id, false);
            var dbTextsByPageResId = new Dictionary<long, List<TextResource>>();
            foreach (var textResourceByPageGroup in dbTexts.GroupBy(x => x.ResourcePage.Id))
            {
                dbTextsByPageResId.Add(textResourceByPageGroup.Key, textResourceByPageGroup.ToList());
            }

            var updatedTextResourceIds = new HashSet<long>();
            foreach (var pageTextData in newPageTextResources)
            {
                var newTextResource = pageTextData.NewTextResource;
                var pageResourceId = newTextResource.ResourcePage.Id;
                List<TextResource> originDbTexts;
                TextResource originDbText;
                dbTextsByPageResId.TryGetValue(pageResourceId, out originDbTexts);
                originDbText = originDbTexts != null && originDbTexts.Count == 1
                    ? originDbTexts.Single()
                    : originDbTexts?.FirstOrDefault(x => x.ExternalId == newTextResource.ExternalId);
                if (originDbText == null)
                {
                    var newResource = new Resource
                    {
                        Project = project,
                        Name = pageTextData.BookPageData.XmlResource ?? string.Empty, // Name is required
                        ContentType = ContentTypeEnum.Page,
                        NamedResourceGroup = resourceGroup,
                        ResourceType = ResourceTypeEnum.Text
                    };
                    newTextResource.Resource = newResource;
                    newTextResource.VersionNumber = 1;
                    newResource.LatestVersion = newTextResource;
                }
                else
                {
                    newTextResource.Resource = originDbText.Resource;
                    newTextResource.VersionNumber = originDbText.VersionNumber + 1;
                    newTextResource.Resource.LatestVersion = newTextResource;
                    newTextResource.Resource.Name = pageTextData.BookPageData.XmlResource ?? string.Empty; // Name is required

                    updatedTextResourceIds.Add(originDbText.Id);
                }
                m_resourceRepository.Create(newTextResource);
                m_allImportedResourceVersionIds.Add(newTextResource.Id);
            }

            // Remove unused
            var unusedDbTexts = dbTexts.Where(x => !updatedTextResourceIds.Contains(x.Id));
            foreach (var unusedDbText in unusedDbTexts)
            {
                var resourceToRemove = unusedDbText.Resource;
                resourceToRemove.IsRemoved = true;
                m_resourceRepository.Update(resourceToRemove);
            }
        }

        private void UpdateImageResources(Project project, IList<ImageResource> newPageImageResources)
        {
            if (newPageImageResources.Count == 0)
            {
                return;
            }

            var projectId = project.Id;
            var imageResourceGroup = GetOrCreateNamedResourceGroup(projectId, TextTypeEnum.NoneOrOriginal, DefaultImportResourceGroupName);
            var dbImages = m_resourceRepository.GetProjectLatestImages(projectId, imageResourceGroup.Id, false);
            var dbImagesByPageResId = new Dictionary<long, List<ImageResource>>();
            foreach (var imageResourceByPageGroup in dbImages.GroupBy(x => x.ResourcePage.Id))
            {
                dbImagesByPageResId.Add(imageResourceByPageGroup.Key, imageResourceByPageGroup.ToList());
            }

            var updatedImageResourceIds = new HashSet<long>();
            foreach (var newImageResource in newPageImageResources)
            {
                var pageResourceId = newImageResource.ResourcePage.Id;
                List<ImageResource> originDbImages;
                ImageResource originDbImage;
                dbImagesByPageResId.TryGetValue(pageResourceId, out originDbImages);
                originDbImage = originDbImages?.FirstOrDefault(x => x.FileName == newImageResource.FileName);

                if (originDbImage == null)
                {
                    var newResource = new Resource
                    {
                        Project = project,
                        Name = newImageResource.FileName,
                        ContentType = ContentTypeEnum.Page,
                        NamedResourceGroup = imageResourceGroup,
                        ResourceType = ResourceTypeEnum.Image
                    };
                    newImageResource.Resource = newResource;
                    newImageResource.VersionNumber = 1;
                    newResource.LatestVersion = newImageResource;
                }
                else
                {
                    newImageResource.Resource = originDbImage.Resource;
                    newImageResource.VersionNumber = originDbImage.VersionNumber + 1;
                    newImageResource.Resource.LatestVersion = newImageResource;
                    newImageResource.Resource.Name = newImageResource.FileName;

                    updatedImageResourceIds.Add(originDbImage.Id);
                }
                m_resourceRepository.Create(newImageResource);
                m_allImportedResourceVersionIds.Add(newImageResource.Id);
            }

            // Remove unused
            var unusedDbImages = dbImages.Where(x => !updatedImageResourceIds.Contains(x.Id));
            foreach (var unusedDbImage in unusedDbImages)
            {
                var resourceToRemove = unusedDbImage.Resource;
                resourceToRemove.IsRemoved = true;
                m_resourceRepository.Update(resourceToRemove);
            }
        }

        private NamedResourceGroup GetOrCreateNamedResourceGroup(long projectId, TextTypeEnum textType, string resourceGroupName)
        {
            var resourceGroup = m_resourceRepository.GetNamedResourceGroup(projectId, resourceGroupName, textType);
            if (resourceGroup != null)
            {
                return resourceGroup;
            }

            var project = m_resourceRepository.Load<Project>(projectId);
            resourceGroup = new NamedResourceGroup
            {
                Project = project,
                Name = resourceGroupName,
                TextType = textType
            };

            m_resourceRepository.Create(resourceGroup);
            return resourceGroup;
        }

        private class NewPageTextData
        {
            public BookPageData BookPageData { get; set; }
            public TextResource NewTextResource { get; set; }
        }
    }
}