using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;

namespace Vokabular.MainService.Core.Works.ProjectMetadata
{
    public class CreateLiteraryKindWork : UnitOfWorkBase<int>
    {
        private readonly MetadataRepository m_metadataRepository;
        private readonly string m_name;

        public CreateLiteraryKindWork(MetadataRepository metadataRepository, string name) : base(metadataRepository.UnitOfWork)
        {
            m_metadataRepository = metadataRepository;
            m_name = name;
        }

        protected override int ExecuteWorkImplementation()
        {
            var literaryKind = new DataEntities.Database.Entities.LiteraryKind
            {
                Name = m_name
            };
            return (int) m_metadataRepository.Create(literaryKind);
        }
    }
}