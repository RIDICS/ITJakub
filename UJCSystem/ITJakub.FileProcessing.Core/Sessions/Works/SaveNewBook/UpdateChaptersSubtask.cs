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

        public void UpdateChapters(long projectId, int userId, BookData bookData, List<PageResource> dbPageResources)
        {
            m_allImportedResourceVersionIds = new List<long>();
            if (bookData.BookContentItems == null)
                return;

            var now = DateTime.UtcNow;
            var importedChapterResourceIds = new HashSet<long>();
            var user = m_resourceRepository.Load<User>(userId);
            var project = m_resourceRepository.Load<Project>(projectId);

            var dbChapters = m_resourceRepository.GetProjectLatestChapters(projectId);
            var dbChaptersByName = dbChapters.ToDictionaryMultipleValues(x => x.Name);
            var dbPagesByPosition = dbPageResources != null
                ? dbPageResources.ToDictionary(x => x.Position)
                : new Dictionary<int, PageResource>();

            var chapterRecursionData = new ChapterRecursionData
            {
                UpdatedChapterResourceIds = importedChapterResourceIds,
                DbChaptersByName = dbChaptersByName,
                DbPagesByPosition = dbPagesByPosition,
                Project = project,
                User = user,
                Now = now,
            };

            UpdateChapterList(bookData.BookContentItems, null, chapterRecursionData);
            
            var unusedDbChapters = dbChapters.Where(x => !importedChapterResourceIds.Contains(x.Id));
            foreach (var unusedDbChapter in unusedDbChapters)
            {
                var resourceToRemove = unusedDbChapter.Resource;
                resourceToRemove.IsRemoved = true;
                m_resourceRepository.Update(resourceToRemove);
            }
        }

        private void UpdateChapterList(IList<BookContentItemData> bookContentItems, ChapterResource parentChapter, ChapterRecursionData data)
        {
            var parentChapterResourceResource = parentChapter?.Resource;

            var updatedChapterResourceIds = data.UpdatedChapterResourceIds;
            var dbPagesByPosition = data.DbPagesByPosition;
            var dbChaptersByName = data.DbChaptersByName;
            var project = data.Project;
            var user = data.User;
            var now = data.Now;

            foreach (var bookContentItem in bookContentItems)
            {
                PageResource dbPage;
                if (bookContentItem.Page == null)
                {
                    dbPage = null;
                }
                else if (!dbPagesByPosition.TryGetValue(bookContentItem.Page.Position, out dbPage))
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

                var newDbChapter = new ChapterResource
                {
                    Resource = null,
                    Name = bookContentItem.Text,
                    ResourceBeginningPage = dbPage?.Resource,
                    Comment = string.Empty,
                    CreateTime = now,
                    CreatedByUser = user,
                    Position = bookContentItem.ItemOrder,
                    VersionNumber = 0,
                    ParentResource = parentChapterResourceResource
                };

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

                    newDbChapter.Resource = newResource;
                    newDbChapter.VersionNumber = 1;
                }
                else
                {
                    newDbChapter.Resource = dbChapter.Resource;
                    newDbChapter.VersionNumber = dbChapter.VersionNumber + 1;

                    updatedChapterResourceIds.Add(dbChapter.Id);
                }

                newDbChapter.Resource.LatestVersion = newDbChapter;
                m_resourceRepository.Create(newDbChapter);

                m_allImportedResourceVersionIds.Add(newDbChapter.Id);
                UpdateChapterList(bookContentItem.SubContentItems, newDbChapter, data);
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
            public HashSet<long> UpdatedChapterResourceIds { get; set; }
            public Dictionary<string, List<ChapterResource>> DbChaptersByName { get; set; }
            public Dictionary<int, PageResource> DbPagesByPosition { get; set; }
            public Project Project { get; set; }
            public User User { get; set; }
            public DateTime Now { get; set; }
        }
    }
}