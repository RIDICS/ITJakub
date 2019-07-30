using System;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.MainService.Core.Works.ProjectMetadata
{
    public class CreateNewMetadataVersionWork : UnitOfWorkBase<long>
    {
        private readonly MetadataRepository m_metadataRepository;
        private readonly long m_projectId;
        private readonly ProjectMetadataContract m_data;
        private readonly int m_userId;

        public CreateNewMetadataVersionWork(MetadataRepository metadataRepository, long projectId, ProjectMetadataContract data, int userId) : base(metadataRepository)
        {
            m_metadataRepository = metadataRepository;
            m_projectId = projectId;
            m_data = data;
            m_userId = userId;
        }

        protected override long ExecuteWorkImplementation()
        {
            var now = DateTime.UtcNow;

            var latestMetadataResource = m_metadataRepository.GetLatestMetadataResource(m_projectId);
            var userEntity = m_metadataRepository.Load<User>(m_userId);
            Resource resource;
            int versionNumber;
            
            if (latestMetadataResource == null)
            {
                var projectEntity = m_metadataRepository.Load<Project>(m_projectId);
                resource = new Resource
                {
                    Name = "Metadata",
                    Project = projectEntity,
                    ContentType = ContentTypeEnum.None,
                    ResourceType = ResourceTypeEnum.ProjectMetadata
                };
                versionNumber = 1;
            }
            else
            {
                resource = latestMetadataResource.Resource;
                versionNumber = latestMetadataResource.VersionNumber + 1;
            }

            var newResourceVersion = new MetadataResource
            {
                Resource = resource,
                AuthorsLabel = m_data.Authors,
                BiblText = m_data.BiblText,
                Copyright = m_data.Copyright,
                CreateTime = now,
                CreatedByUser = userEntity,
                ManuscriptCountry = m_data.ManuscriptCountry,
                ManuscriptExtent = m_data.ManuscriptExtent,
                ManuscriptSettlement = m_data.ManuscriptSettlement,
                ManuscriptIdno = m_data.ManuscriptIdno,
                ManuscriptRepository = m_data.ManuscriptRepository,
                NotAfter = m_data.NotAfter,
                NotBefore = m_data.NotBefore,
                OriginDate = m_data.OriginDate,
                PublishDate = m_data.PublishDate,
                PublishPlace = m_data.PublishPlace,
                PublisherText = m_data.PublisherText,
                PublisherEmail = m_data.PublisherEmail,
                RelicAbbreviation = m_data.RelicAbbreviation,
                SourceAbbreviation = m_data.SourceAbbreviation,
                SubTitle = m_data.SubTitle,
                Title = m_data.Title,
                VersionNumber = versionNumber
            };
            resource.LatestVersion = newResourceVersion;

            return (long) m_metadataRepository.Create(newResourceVersion);
        }
    }
}