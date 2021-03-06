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
    public class CreateOrUpdatePagesWork : UnitOfWorkBase
    {
        private readonly ResourceRepository m_resourceRepository;
        private readonly IList<CreateOrUpdatePageContract> m_newPages;
        private readonly long m_projectId;
        private readonly int m_userId;

        public CreateOrUpdatePagesWork(ResourceRepository resourceRepository, IList<CreateOrUpdatePageContract> newPages, long projectId, int userId) : base(resourceRepository)
        {
            m_resourceRepository = resourceRepository;
            m_newPages = newPages;
            m_projectId = projectId;
            m_userId = userId;
        }

        protected override void ExecuteWorkImplementation()
        {
            var now = DateTime.UtcNow;
            var user = m_resourceRepository.Load<User>(m_userId);
            var dbPages = m_resourceRepository.GetProjectLatestPages(m_projectId);
            var updatedResourcePageIds = new List<long>();

            foreach (var newPage in m_newPages)
            {
                var pageResource = new PageResource
                {
                    Name = newPage.Name,
                    Comment = null,
                    Position = newPage.Position,
                    Resource = null,
                    VersionNumber = 0,
                    CreateTime = now,
                    CreatedByUser = user,
                    Terms = null, // Terms must be also updated
                };

                if (newPage.Id == null)
                {
                    var resource = new Resource
                    {
                        ContentType = ContentTypeEnum.Page,
                        ResourceType = ResourceTypeEnum.Page,
                        Project = m_resourceRepository.Load<Project>(m_projectId),
                    };

                    pageResource.Resource = resource;
                    pageResource.VersionNumber = 1;
                }
                else
                {
                    var latestPageResource = m_resourceRepository.GetLatestResourceVersion<PageResource>(newPage.Id.Value);
                    if (latestPageResource == null)
                    {
                        throw new MainServiceException(MainServiceErrorCode.EntityNotFound, "The entity was not found.");
                    }

                    pageResource.Resource = latestPageResource.Resource;
                    pageResource.VersionNumber = latestPageResource.VersionNumber + 1;
                    pageResource.Terms = new List<Term>(latestPageResource.Terms); // Lazy fetch

                    updatedResourcePageIds.Add(newPage.Id.Value);
                }

                pageResource.Resource.Name = newPage.Name;
                pageResource.Resource.LatestVersion = pageResource;

                m_resourceRepository.Create(pageResource);
            }

            var removeResourceSubwork = new RemoveResourceSubwork(m_resourceRepository);
            foreach (var dbPage in dbPages)
            {
                if (!updatedResourcePageIds.Contains(dbPage.Resource.Id))
                {
                    removeResourceSubwork.RemoveResource(dbPage.Resource.Id);
                }
            }
        }
    }
}
