using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;
using ProjectImportMetadata = Vokabular.ProjectParsing.Model.Entities.ProjectImportMetadata;

namespace Vokabular.ProjectImport.Works
{
    public class UpdateImportedProjectWork : UnitOfWorkBase
    {
        private readonly ProjectRepository m_projectRepository;
        private readonly ProjectImportMetadata m_metadata;

        public UpdateImportedProjectWork(ProjectRepository projectRepository, ProjectImportMetadata metadata) : base(projectRepository)
        {
            m_projectRepository = projectRepository;
            m_metadata = metadata;
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
