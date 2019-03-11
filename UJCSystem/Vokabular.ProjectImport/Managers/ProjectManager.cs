using Vokabular.DataEntities.Database.Repositories;
using Vokabular.ProjectImport.Works;
using Vokabular.ProjectParsing.Model.Entities;

namespace Vokabular.ProjectImport.Managers
{
    public class ProjectManager
    {
        private readonly ProjectRepository m_projectRepository;

        public ProjectManager(ProjectRepository projectRepository)
        {
            m_projectRepository = projectRepository;
        }

        public long CreateProject(ProjectImportMetadata projectImportMetadata, int userId)
        {
            return new CreateImportedProjectWork(m_projectRepository, projectImportMetadata, userId).Execute();
        }
    }
}
