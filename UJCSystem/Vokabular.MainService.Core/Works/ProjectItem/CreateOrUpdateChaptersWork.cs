﻿using System;
using System.Collections.Generic;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.Core.Works.Content;
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
            var updatedResourceChapterIds = new List<long>();

            foreach (var chapter in m_chapterData)
            {
                var resourceBeginningPage = m_resourceRepository.Load<Resource>(chapter.BeginningPageId);
                var resourceParentChapter = chapter.ParentChapterId != null
                    ? m_resourceRepository.Load<Resource>(chapter.ParentChapterId.Value)
                    : null;

                var chapterResource = new ChapterResource
                {
                    Name = chapter.Name,
                    Comment = chapter.Comment,
                    Position = chapter.Position,
                    Resource = null,
                    ParentResource = resourceParentChapter,
                    ResourceBeginningPage = resourceBeginningPage,
                    VersionNumber = 0,
                    CreateTime = now,
                    CreatedByUser = user,
                };

                if (chapter.Id == null)
                {
                    var resource = new Resource
                    {
                        ContentType = ContentTypeEnum.Chapter,
                        ResourceType = ResourceTypeEnum.Chapter,
                        Project = m_resourceRepository.Load<Project>(m_projectId),
                    };

                    chapterResource.Resource = resource;
                    chapterResource.VersionNumber = 1;
                }
                else
                {
                    var latestChapterResource = m_resourceRepository.GetLatestResourceVersion<ChapterResource>(chapter.Id.Value);
                    if (latestChapterResource == null)
                    {
                        throw new MainServiceException(MainServiceErrorCode.EntityNotFound, "The entity was not found.");
                    }

                    chapterResource.Resource = latestChapterResource.Resource;
                    chapterResource.VersionNumber = latestChapterResource.VersionNumber + 1;

                    updatedResourceChapterIds.Add(chapter.Id.Value);
                }

                chapterResource.Resource.Name = chapter.Name;
                chapterResource.Resource.LatestVersion = chapterResource;

                m_resourceRepository.Create(chapterResource);
            }

            var removeResourceSubwork = new RemoveResourceSubwork(m_resourceRepository); 
            foreach (var dbChapter in dbChapters)
            {
                if (!updatedResourceChapterIds.Contains(dbChapter.Resource.Id))
                {
                    removeResourceSubwork.RemoveResource(dbChapter.Resource.Id);
                }
            }
        }
    }
}