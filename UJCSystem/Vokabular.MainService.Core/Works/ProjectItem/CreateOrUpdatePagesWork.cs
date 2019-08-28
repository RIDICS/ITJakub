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
        private readonly IList<CreateOrUpdatePageContract> m_data;
        private readonly long m_projectId;
        private readonly int m_userId;

        public CreateOrUpdatePagesWork(ResourceRepository resourceRepository, IList<CreateOrUpdatePageContract> data, long projectId, int userId) : base(resourceRepository)
        {
            m_resourceRepository = resourceRepository;
            m_data = data;
            m_projectId = projectId;
            m_userId = userId;
        }

        protected override void ExecuteWorkImplementation()
        {
            var now = DateTime.UtcNow;
            var user = m_resourceRepository.Load<User>(m_userId);
            var pages = m_resourceRepository.GetProjectPages(m_projectId);
            var updatedPages = new List<long>();

            foreach (var page in m_data)
            {
                PageResource pageResource;
                if (page.Id == null)
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
                    pageResource = m_resourceRepository.GetLatestResourceVersion<PageResource>(page.Id.Value);
                    if (pageResource == null)
                    {
                        throw new MainServiceException(MainServiceErrorCode.EntityNotFound, "The entity was not found.");
                    }
                    updatedPages.Add(page.Id.Value);
                }

                pageResource.Name = page.Name;
                pageResource.Position = page.Position;
                pageResource.Resource.Name = page.Name;
                pageResource.Resource.LatestVersion = pageResource;
                pageResource.VersionNumber++;
                pageResource.CreateTime = now;
                pageResource.CreatedByUser = user;

                m_resourceRepository.Save(pageResource);
            }


            foreach (var page in pages)
            {
                if (!updatedPages.Contains(page.Id))
                {
                    //TODO remove page
                    //m_resourceRepository.Delete(page);
                }
            }
        }
    }
}
