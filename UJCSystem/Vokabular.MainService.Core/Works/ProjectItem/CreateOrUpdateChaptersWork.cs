using System;
using System.Collections.Generic;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.DataContracts;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.MainService.Core.Works.ProjectItem
{
    public class CreateOrUpdateChaptersWork : UnitOfWorkBase
    {
        private readonly ResourceRepository m_resourceRepository;
        private readonly IList<CreateOrUpdateChapterContract> m_chapterData;
        private readonly long m_projectId;
        private readonly int m_userId;
        
        public CreateOrUpdateChaptersWork(ResourceRepository resourceRepository, IList<CreateOrUpdateChapterContract> chapterData, long projectId, int userId) : base(resourceRepository)
        {
            m_resourceRepository = resourceRepository;
            m_chapterData = chapterData;
            m_projectId = projectId;
            m_userId = userId;
        }

        protected override void ExecuteWorkImplementation()
        {
            var now = DateTime.UtcNow;
            var user = m_resourceRepository.Load<User>(m_userId);
            var dbChapters = m_resourceRepository.GetProjectLatestChapters(m_projectId);
            var updatedChapterIds = new List<long>();

            foreach (var chapter in m_chapterData)
            {
                var resourceBeginningPage = m_resourceRepository.Load<Resource>(chapter.BeginningPageId);
                var resourceParentChapter = chapter.ParentChapterId != null
                    ? m_resourceRepository.Load<Resource>(chapter.ParentChapterId.Value)
                    : null;

                ChapterResource chapterResource;
                if (chapter.Id == null)
                {
                    chapterResource = new ChapterResource
                    {
                        VersionNumber = 0,
                        Resource = new Resource
                        {
                            ContentType = ContentTypeEnum.Chapter,
                            ResourceType = ResourceTypeEnum.Chapter,
                            Project = m_resourceRepository.Load<Project>(m_projectId),
                        }
                    };
                }
                else
                {
                    chapterResource = m_resourceRepository.GetLatestResourceVersion<ChapterResource>(chapter.Id.Value);
                    if (chapterResource == null)
                    {
                        throw new MainServiceException(MainServiceErrorCode.EntityNotFound, "The entity was not found.");
                    }
                    updatedChapterIds.Add(chapter.Id.Value);
                }

                chapterResource.Name = chapter.Name;
                chapterResource.Comment = chapter.Comment;
                chapterResource.Position = chapter.Position;
                chapterResource.ParentResource = resourceParentChapter;
                chapterResource.ResourceBeginningPage = resourceBeginningPage;
                chapterResource.Resource.Name = chapter.Name;
                chapterResource.Resource.LatestVersion = chapterResource;
                chapterResource.VersionNumber++;
                chapterResource.CreateTime = now;
                chapterResource.CreatedByUser = user;

                m_resourceRepository.Save(chapterResource);
            }

            foreach (var dbChapter in dbChapters)
            {
                if (!updatedChapterIds.Contains(dbChapter.Id))
                {
                    //TODO remove chapter
                    //m_resourceRepository.Delete(dbChapter);
                }
            }
        }

    }
}