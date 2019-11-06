using System;
using System.Collections.Generic;
using System.Linq;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.Core.Works.Content;
using Vokabular.Shared.DataEntities.UnitOfWork;
using Vokabular.TextConverter.Markdown.Extensions;

namespace Vokabular.MainService.Core.Works.ProjectItem
{
    public class GenerateChaptersWork : UnitOfWorkBase
    {
        private readonly long m_projectId;
        private readonly int m_userId;
        private readonly IList<Tuple<PageResource, IList<MarkdownHeadingData>>> m_pageWithHeadingsList;
        private readonly ResourceRepository m_resourceRepository;

        public GenerateChaptersWork(long projectId, int userId, IList<Tuple<PageResource, IList<MarkdownHeadingData>>> pageWithHeadingsList,
            ResourceRepository resourceRepository) : base(resourceRepository)
        {
            m_projectId = projectId;
            m_userId = userId;
            m_pageWithHeadingsList = pageWithHeadingsList;
            m_resourceRepository = resourceRepository;
        }

        protected override void ExecuteWorkImplementation()
        {
            var now = DateTime.UtcNow;
            var chapters = m_resourceRepository.GetProjectLatestChapters(m_projectId);
            var project = m_resourceRepository.Load<Project>(m_projectId);
            var user = m_resourceRepository.Load<User>(m_userId);

            // Fix levels
            var currentLevel = 0; // min level is 1
            foreach (var heading in m_pageWithHeadingsList.SelectMany(x => x.Item2))
            {
                if (heading.Level > currentLevel + 1)
                {
                    heading.Level = currentLevel + 1;
                }
                else
                {
                    currentLevel = heading.Level;
                }
            }

            // Update chapters
            var updatedResourceChapters = new List<Resource>();
            var currentChapterByLevel = new ChapterResource[7];
            var position = 0;
            
            foreach (var pageWithHeadings in m_pageWithHeadingsList)
            {
                var headings = pageWithHeadings.Item2;
                var resourcePage = m_resourceRepository.Load<Resource>(pageWithHeadings.Item1.Resource.Id);

                foreach (var markdownHeadingData in headings)
                {
                    position++;

                    // Find original Resource
                    ChapterResource originalChapterResource = null;
                    var originalChapters = chapters.Where(x => x.Name == markdownHeadingData.Heading).ToList();
                    if (originalChapters.Count == 1)
                    {
                        originalChapterResource = originalChapters[0];
                    }
                    else if (originalChapters.Count > 1)
                    {
                        originalChapterResource = originalChapters.FirstOrDefault(x => x.Position == position);
                    }

                    if (updatedResourceChapters.Contains(originalChapterResource?.Resource))
                    {
                        originalChapterResource = null;
                    }

                    // Create ChapterResource
                    var chapterResource = new ChapterResource
                    {
                        Comment = null,
                        CreateTime = now,
                        CreatedByUser = user,
                        Name = markdownHeadingData.Heading,
                        ParentResource = currentChapterByLevel[markdownHeadingData.Level - 1]?.Resource,
                        Position = position,
                        Resource = null, // is updated below
                        ResourceBeginningPage = resourcePage,
                        VersionNumber = 1, // is updated below
                    };

                    if (originalChapterResource != null)
                    {
                        chapterResource.Resource = originalChapterResource.Resource;
                        chapterResource.VersionNumber = originalChapterResource.VersionNumber + 1;

                        updatedResourceChapters.Add(chapterResource.Resource);
                    }
                    else
                    {
                        chapterResource.Resource = new Resource
                        {
                            ContentType = ContentTypeEnum.Chapter,
                            IsRemoved = false,
                            Name = chapterResource.Name,
                            Project = project,
                            ResourceType = ResourceTypeEnum.Chapter,
                        };
                    }

                    chapterResource.Resource.LatestVersion = chapterResource;
                    currentChapterByLevel[markdownHeadingData.Level] = chapterResource;

                    m_resourceRepository.Create(chapterResource);
                }
            }

            // Remove unused chapters
            var removeResourceSubwork = new RemoveResourceSubwork(m_resourceRepository);
            foreach (var chapterResource in chapters)
            {
                var resource = chapterResource.Resource;
                if (!updatedResourceChapters.Contains(resource))
                {
                    removeResourceSubwork.RemoveResource(resource.Id);
                }
            }
        }
    }
}