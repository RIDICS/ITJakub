using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;

namespace Vokabular.MainService.Core.Works.ProjectMetadata
{
    public class GetLatestProjectMetadataWork : UnitOfWorkBase<MetadataResource>
    {
        private readonly MetadataRepository m_metadataRepository;
        private readonly long m_projectId;

        public GetLatestProjectMetadataWork(MetadataRepository metadataRepository, long projectId) : base(metadataRepository.UnitOfWork)
        {
            m_metadataRepository = metadataRepository;
            m_projectId = projectId;
        }

        protected override MetadataResource ExecuteWorkImplementation()
        {
            var result = m_metadataRepository.GetLatestMetadataResource(m_projectId);
            return result;
        }
    }
}