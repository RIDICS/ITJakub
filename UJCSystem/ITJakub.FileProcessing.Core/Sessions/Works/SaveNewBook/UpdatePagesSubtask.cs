using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ITJakub.FileProcessing.Core.Data;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.Repositories;

namespace ITJakub.FileProcessing.Core.Sessions.Works.SaveNewBook
{
    public class UpdatePagesSubtask
    {
        private readonly string DefaultImportResourceGroupName = "IMPORT";
        private readonly ResourceRepository m_resourceRepository;

        public UpdatePagesSubtask(ResourceRepository resourceRepository)
        {
            m_resourceRepository = resourceRepository;
        }

        public List<PageResource> ResultPageResourceList { get; private set; }

        public void UpdatePages(long projectId, long bookVersionId, int userId, string comment, BookData bookData, Dictionary<string, Term> dbTermCache)
        {
            if (bookData.Pages == null)
                return;

            var importedPageResourceIds = new HashSet<long>();
            var newPageTextResources = new List<NewPageTextData>();
            var newPageImageResources = new List<ImageResource>();
            var resultPageResourceList = new List<PageResource>();

            var now = DateTime.UtcNow;
            var project = m_resourceRepository.Load<Project>(projectId);
            var user = m_resourceRepository.Load<User>(userId);
            var bookVersion = m_resourceRepository.Load<BookVersionResource>(bookVersionId);
            var dbPages = m_resourceRepository.GetProjectPages(projectId);
            var dbPagesDict = dbPages.ToDictionary(x => x.Name);

            // Update page list
            foreach (var page in bookData.Pages)
            {
                PageResource dbPageResource;
                
                if (!dbPagesDict.TryGetValue(page.Text, out dbPageResource))
                {
                    var newResource = new Resource
                    {
                        Project = project,
                        Name = page.Text,
                        ContentType = ContentTypeEnum.Page,
                        ResourceType = ResourceTypeEnum.Page
                    };
                    var newPageResource = new PageResource
                    {
                        Resource = newResource,
                        Name = page.Text,
                        Comment = comment,
                        CreateTime = now,
                        CreatedByUser = user,
                        Position = page.Position,
                        VersionNumber = 1,
                        Terms = PrepareTermList(page.TermXmlIds, dbTermCache),
                    };
                    newResource.LatestVersion = newPageResource;

                    m_resourceRepository.Create(newPageResource);
                    dbPageResource = newPageResource;
                }
                else
                {
                    //if (IsPageUpdated(dbPageResource, page))
                    //{
                    // Always update page data
                        dbPageResource.Position = page.Position;
                        dbPageResource.CreateTime = now;
                        dbPageResource.CreatedByUser = user;
                        dbPageResource.Comment = comment;
                        dbPageResource.Terms = PrepareTermList(page.TermXmlIds, dbTermCache);
                        // Update resource name is not required (PageResources are distinguish by name)

                        m_resourceRepository.Update(dbPageResource);
                        importedPageResourceIds.Add(dbPageResource.Id);
                    //}
                }

                if (!string.IsNullOrEmpty(page.XmlId))
                {
                    var newTextResource = new TextResource
                    {
                        Resource = null,
                        Comment = comment,
                        CreateTime = now,
                        CreatedByUser = user,
                        ExternalId = page.XmlId,
                        ParentResource = dbPageResource.Resource,
                        BookVersion = bookVersion,
                        VersionNumber = 0
                    };
                    newPageTextResources.Add(new NewPageTextData
                    {
                        NewTextResource = newTextResource,
                        BookPageData = page,
                    });
                }

                if (!string.IsNullOrEmpty(page.Image))
                {
                    var imageMimeType = MimeMapping.GetMimeMapping(page.Image);
                    var newImageResource = new ImageResource
                    {
                        Resource = null,
                        Comment = comment,
                        CreateTime = now,
                        CreatedByUser = user,
                        FileName = page.Image,
                        MimeType = imageMimeType,
                        Size = 0,
                        ParentResource = dbPageResource.Resource,
                        VersionNumber = 0
                    };
                    newPageImageResources.Add(newImageResource);
                }

                resultPageResourceList.Add(dbPageResource);
            }

            ResultPageResourceList = resultPageResourceList;

            // Update positions to unused pages
            var unusedDbPages = dbPages.Where(x => !importedPageResourceIds.Contains(x.Id));
            foreach (var unusedDbPage in unusedDbPages)
            {
                unusedDbPage.Position = 0;
                m_resourceRepository.Update(unusedDbPage);
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
            var dbTexts = m_resourceRepository.GetProjectTexts(projectId, resourceGroup.Id);
            var dbTextsByPageResId = new Dictionary<long, List<TextResource>>();
            foreach (var textResourceByPageGroup in dbTexts.GroupBy(x => x.ParentResource.Id))
            {
                dbTextsByPageResId.Add(textResourceByPageGroup.Key, textResourceByPageGroup.ToList());
            }

            foreach (var pageTextData in newPageTextResources)
            {
                var newTextResource = pageTextData.NewTextResource;
                var pageResourceId = newTextResource.ParentResource.Id;
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
                }
                m_resourceRepository.Create(newTextResource);
            }
        }

        private void UpdateImageResources(Project project, IList<ImageResource> newPageImageResources)
        {
            if (newPageImageResources.Count == 0)
            {
                return;
            }

            var projectId = project.Id;
            var imageResourceGroup = GetOrCreateNamedResourceGroup(projectId, TextTypeEnum.Original, DefaultImportResourceGroupName);
            var dbImages = m_resourceRepository.GetProjectImages(projectId, imageResourceGroup.Id);
            var dbImagesByPageResId = new Dictionary<long, List<ImageResource>>();
            foreach (var imageResourceByPageGroup in dbImages.GroupBy(x => x.ParentResource.Id))
            {
                dbImagesByPageResId.Add(imageResourceByPageGroup.Key, imageResourceByPageGroup.ToList());
            }

            foreach (var newImageResource in newPageImageResources)
            {
                var pageResourceId = newImageResource.ParentResource.Id;
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
                }
                m_resourceRepository.Create(newImageResource);
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