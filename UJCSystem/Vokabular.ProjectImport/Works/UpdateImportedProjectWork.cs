using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;
using ProjectImportMetadata = Vokabular.ProjectParsing.Model.Entities.ProjectImportMetadata;

namespace Vokabular.ProjectImport.Works
{
    public class CreateImportedMetadataWork : UnitOfWorkBase
    {
        private readonly ProjectRepository m_projectRepository;
        private readonly ProjectImportMetadata m_metadata;
        private readonly int m_userId;

        public CreateImportedMetadataWork(ProjectRepository projectRepository, ProjectImportMetadata metadata, int userId) : base(projectRepository)
        {
            m_projectRepository = projectRepository;
            m_metadata = metadata;
            m_userId = userId;
        }

        protected override void ExecuteWorkImplementation()
        {
            var project = new Project
            {
                Name = m_metadata.Project.Name,
            };

            m_projectRepository.Update(project);
        }
    }
}
