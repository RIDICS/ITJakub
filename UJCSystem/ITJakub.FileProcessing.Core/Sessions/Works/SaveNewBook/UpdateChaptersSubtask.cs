using System;
using System.Collections.Generic;
using System.Linq;
using ITJakub.FileProcessing.Core.Data;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.Repositories;

namespace ITJakub.FileProcessing.Core.Sessions.Works.SaveNewBook
{
    public class UpdateChaptersSubtask
    {
        private readonly ResourceRepository m_resourceRepository;

        public UpdateChaptersSubtask(ResourceRepository resourceRepository)
        {
            m_resourceRepository = resourceRepository;
        }

        public void UpdateChapters(long projectId, int userId, string comment, BookData bookData, List<PageResource> dbPageResources)
        {
            if (bookData.BookContentItems == null)
                return;

            var now = DateTime.UtcNow;
            var newChapterNames = new HashSet<string>();
            var user = m_resourceRepository.Load<User>(userId);
            var project = m_resourceRepository.Load<Project>(projectId);

            var dbChapters = m_resourceRepository.GetProjectChapters(projectId);
            var dbChaptersByName = dbChapters.ToDictionary(x => x.Name);
            //var dbPages = m_resourceRepository.GetProjectPages(projectId);
            var dbPagesByName = dbPageResources?.ToDictionary(x => x.Name);

            var chapterRecursionData = new ChapterRecursionData
            {
                NewChapterNames = newChapterNames,
                DbChaptersByName = dbChaptersByName,
                DbPagesByName = dbPagesByName,
                Project = project,
                User = user,
                Now = now,
                Comment = comment
            };

            UpdateChapterList(bookData.BookContentItems, null, chapterRecursionData);
            
            var unusedDbChapters = dbChapters.Where(x => !newChapterNames.Contains(x.Name));
            foreach (var unusedDbChapter in unusedDbChapters)
            {
                unusedDbChapter.Position = 0;
                m_resourceRepository.Update(unusedDbChapter);
            }
        }

        //TODO dbPagesByName can be null
        private void UpdateChapterList(IList<BookContentItemData> bookContentItems, ChapterResource parentChapter, ChapterRecursionData data)
        {
            var parentChapterResourceResource = parentChapter?.Resource;
            var parentChapterResourceResourceId = parentChapterResourceResource?.Id;

            var newChapterNames = data.NewChapterNames;
            var dbPagesByName = data.DbPagesByName;
            var dbChaptersByName = data.DbChaptersByName;
            var project = data.Project;
            var user = data.User;
            var now = data.Now;
            var comment = data.Comment;

            foreach (var bookContentItem in bookContentItems)
            {
                newChapterNames.Add(bookContentItem.Text);

                PageResource dbPage;
                if (!dbPagesByName.TryGetValue(bookContentItem.Page.Text, out dbPage))
                {
                    throw new ArgumentException($"Trying assign Chapter to non-existent Page {bookContentItem.Page.Text}");
                }

                ChapterResource dbChapter;
                if (!dbChaptersByName.TryGetValue(bookContentItem.Text, out dbChapter))
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

                        m_resourceRepository.Update(dbChapter);
                    }
                }

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
            public HashSet<string> NewChapterNames { get; set; }
            public Dictionary<string, ChapterResource> DbChaptersByName { get; set; }
            public Dictionary<string, PageResource> DbPagesByName { get; set; }
            public Project Project { get; set; }
            public User User { get; set; }
            public DateTime Now { get; set; }
            public string Comment { get; set; }
        }
    }
}