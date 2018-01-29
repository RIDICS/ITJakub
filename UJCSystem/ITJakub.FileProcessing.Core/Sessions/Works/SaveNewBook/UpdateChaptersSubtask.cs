using System;
using System.Collections.Generic;
using System.Linq;
using ITJakub.FileProcessing.Core.Data;
using ITJakub.FileProcessing.Core.Sessions.Works.Helpers;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.Repositories;

namespace ITJakub.FileProcessing.Core.Sessions.Works.SaveNewBook
{
    public class UpdateChaptersSubtask
    {
        private readonly ResourceRepository m_resourceRepository;
        private List<long> m_allImportedResourceVersionIds;

        public UpdateChaptersSubtask(ResourceRepository resourceRepository)
        {
            m_resourceRepository = resourceRepository;
        }

        public List<long> ImportedResourceVersionIds => m_allImportedResourceVersionIds;

        public void UpdateChapters(long projectId, int userId, string comment, BookData bookData, List<PageResource> dbPageResources)
        {
            m_allImportedResourceVersionIds = new List<long>();
            if (bookData.BookContentItems == null)
                return;

            var now = DateTime.UtcNow;
            var importedChapterResourceIds = new HashSet<long>();
            var user = m_resourceRepository.Load<User>(userId);
            var project = m_resourceRepository.Load<Project>(projectId);

            var dbChapters = m_resourceRepository.GetProjectChapters(projectId);
            var dbChaptersByName = dbChapters.ToDictionaryMultipleValues(x => x.Name);
            var dbPagesByPosition = dbPageResources != null
                ? dbPageResources.ToDictionary(x => x.Position)
                : new Dictionary<int, PageResource>();

            var chapterRecursionData = new ChapterRecursionData
            {
                ImportedChapterResourceIds = importedChapterResourceIds,
                DbChaptersByName = dbChaptersByName,
                DbPagesByPosition = dbPagesByPosition,
                Project = project,
                User = user,
                Now = now,
                Comment = comment
            };

            UpdateChapterList(bookData.BookContentItems, null, chapterRecursionData);
            
            var unusedDbChapters = dbChapters.Where(x => !importedChapterResourceIds.Contains(x.Id));
            foreach (var unusedDbChapter in unusedDbChapters)
            {
                unusedDbChapter.Position = 0;
                m_resourceRepository.Update(unusedDbChapter);
            }
        }

        private void UpdateChapterList(IList<BookContentItemData> bookContentItems, ChapterResource parentChapter, ChapterRecursionData data)
        {
            var parentChapterResourceResource = parentChapter?.Resource;
            var parentChapterResourceResourceId = parentChapterResourceResource?.Id;

            var importedChapterResourceIds = data.ImportedChapterResourceIds;
            var dbPagesByPosition = data.DbPagesByPosition;
            var dbChaptersByName = data.DbChaptersByName;
            var project = data.Project;
            var user = data.User;
            var now = data.Now;
            var comment = data.Comment;

            foreach (var bookContentItem in bookContentItems)
            {
                PageResource dbPage;
                if (!dbPagesByPosition.TryGetValue(bookContentItem.Page.Position, out dbPage))
                {
                    throw new ArgumentException($"Trying assign Chapter to non-existent Page with position {bookContentItem.Page.Position} (page name: {bookContentItem.Page.Text})");
                }

                ChapterResource dbChapter = null;
                List<ChapterResource> dbChapters;
                if (dbChaptersByName.TryGetValue(bookContentItem.Text, out dbChapters))
                {
                    dbChapter = dbChapters.Count == 1
                        ? dbChapters.First()
                        : dbChapters.FirstOrDefault(x => x.Position == bookContentItem.ItemOrder); // If multiple chapters have the same name, try identify chapter by Position
                }

                if (dbChapter == null)
                {
                    var newResource = new Resource
                    {
                        Project = project,
                        ContentType = ContentTypeEnum.Chapter,
                        Name = bookContentItem.Text,
                        LatestVersion = null,
                        ResourceType = ResourceTypeEnum.Chapter
                    };
                    dbChapter = new ChapterResource
                    {
                        Resource = newResource,
                        Name = bookContentItem.Text,
                        ResourceBeginningPage = dbPage.Resource,
                        Comment = comment,
                        CreateTime = now,
                        CreatedByUser = user,
                        Position = bookContentItem.ItemOrder,
                        VersionNumber = 1,
                        ParentResource = parentChapterResourceResource
                    };
                    newResource.LatestVersion = dbChapter;
                    m_resourceRepository.Create(dbChapter);
                }
                else
                {
                    if (IsChapterChanged(dbChapter, bookContentItem, dbPage.Resource.Id, parentChapterResourceResourceId))
                    {
                        dbChapter.Name = bookContentItem.Text;
                        dbChapter.Position = bookContentItem.ItemOrder;
                        dbChapter.ResourceBeginningPage = dbPage.Resource;
                        dbChapter.ParentResource = parentChapterResourceResource;
                        // Update resource name is not required (ChapterResources are distinguish by name)

                        m_resourceRepository.Update(dbChapter);
                    }
                    importedChapterResourceIds.Add(dbChapter.Id);
                }

                m_allImportedResourceVersionIds.Add(dbChapter.Id);
                UpdateChapterList(bookContentItem.SubContentItems, dbChapter, data);
            }
        }

        private bool IsChapterChanged(ChapterResource dbChapter, BookContentItemData bookContentItem, long newPageResourceResourceId, long? newParentResourceId)
        {
            return bookContentItem.ItemOrder != dbChapter.Position ||
                   newPageResourceResourceId != dbChapter.ResourceBeginningPage.Id ||
                   newParentResourceId != dbChapter.ParentResource?.Id ||
                   bookContentItem.Text != dbChapter.Name;
        }

        private class ChapterRecursionData
        {
            public HashSet<long> ImportedChapterResourceIds { get; set; }
            public Dictionary<string, List<ChapterResource>> DbChaptersByName { get; set; }
            public Dictionary<int, PageResource> DbPagesByPosition { get; set; }
            public Project Project { get; set; }
            public User User { get; set; }
            public DateTime Now { get; set; }
            public string Comment { get; set; }
        }
    }
}