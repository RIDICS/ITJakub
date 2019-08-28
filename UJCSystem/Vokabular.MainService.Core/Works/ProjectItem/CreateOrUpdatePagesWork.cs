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
            var dbPages = m_resourceRepository.GetProjectPages(m_projectId);
            var updatedPageIds = new List<long>();

            foreach (var newPage in m_newPages)
            {
                PageResource pageResource;
                if (newPage.Id == null)
                {
                    pageResource = new PageResource
                    {
                        VersionNumber = 0,
                        Resource = new Resource
                        {
                            ContentType = ContentTypeEnum.Page,
                            ResourceType = ResourceTypeEnum.Page,
                            Project = m_resourceRepository.Load<Project>(m_projectId),
                        }
                    };
                }
                else
                {
                    pageResource = m_resourceRepository.GetLatestResourceVersion<PageResource>(newPage.Id.Value);
                    if (pageResource == null)
                    {
                        throw new MainServiceException(MainServiceErrorCode.EntityNotFound, "The entity was not found.");
                    }
                    updatedPageIds.Add(newPage.Id.Value);
                }

                pageResource.Name = newPage.Name;
                pageResource.Position = newPage.Position;
                pageResource.Resource.Name = newPage.Name;
                pageResource.Resource.LatestVersion = pageResource;
                pageResource.VersionNumber++;
                pageResource.CreateTime = now;
                pageResource.CreatedByUser = user;

                m_resourceRepository.Save(pageResource);
            }


            foreach (var dbPage in dbPages)
            {
                if (!updatedPageIds.Contains(dbPage.Id))
                {
                    //TODO remove page
                    //m_resourceRepository.Delete(dbPage);
                }
            }
        }
    }
}
