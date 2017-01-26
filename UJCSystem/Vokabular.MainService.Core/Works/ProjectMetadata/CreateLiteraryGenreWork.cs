using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;

namespace Vokabular.MainService.Core.Works.ProjectMetadata
{
    public class CreateLiteraryGenreWork : UnitOfWorkBase<int>
    {
        private readonly MetadataRepository m_metadataRepository;
        private readonly string m_name;

        public CreateLiteraryGenreWork(MetadataRepository metadataRepository, string name) : base(metadataRepository)
        {
            m_metadataRepository = metadataRepository;
            m_name = name;
        }

        protected override int ExecuteWorkImplementation()
        {
            var literaryGenre = new DataEntities.Database.Entities.LiteraryGenre
            {
                Name = m_name
            };
            return (int) m_metadataRepository.Create(literaryGenre);
        }
    }
}