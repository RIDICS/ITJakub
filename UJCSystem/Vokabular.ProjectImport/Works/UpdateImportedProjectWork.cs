using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;
using Vokabular.ProjectParsing.Model.Entities;
using Project = Vokabular.DataEntities.Database.Entities.Project;

namespace Vokabular.ProjectImport.Works
{
    public class UpdateImportedProjectWork : UnitOfWorkBase
    {
        private readonly ProjectRepository m_projectRepository;
        private readonly ImportedRecord m_metadata;

        public UpdateImportedProjectWork(ProjectRepository projectRepository, ImportedRecord metadata) : base(projectRepository)
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
