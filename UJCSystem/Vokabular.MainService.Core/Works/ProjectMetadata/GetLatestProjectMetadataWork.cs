using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;
using Vokabular.MainService.Core.Parameter;

namespace Vokabular.MainService.Core.Works.ProjectMetadata
{
    public class GetLatestProjectMetadataWork : UnitOfWorkBase<MetadataResource>
    {
        private readonly MetadataRepository m_metadataRepository;
        private readonly long m_projectId;
        private readonly GetProjectMetadataParameter m_parameters;

        public GetLatestProjectMetadataWork(MetadataRepository metadataRepository, long projectId, GetProjectMetadataParameter parameters) : base(metadataRepository)
        {
            m_metadataRepository = metadataRepository;
            m_projectId = projectId;
            m_parameters = parameters;
        }

        protected override MetadataResource ExecuteWorkImplementation()
        {
            var result = m_metadataRepository.GetLatestMetadataResource(m_projectId);

            if (m_parameters.IsAnyAdditionalParameter())
            {
                m_metadataRepository.GetAdditionalProjectMetadata(m_projectId, m_parameters.IncludeAuthor, m_parameters.IncludeResponsiblePerson,
                    m_parameters.IncludeKind, m_parameters.IncludeGenre, m_parameters.IncludeOriginal);
            }
            
            return result;
        }
    }
}