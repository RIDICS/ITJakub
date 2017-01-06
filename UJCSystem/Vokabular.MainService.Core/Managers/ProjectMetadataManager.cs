using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.Core.Works.ProjectMetadata;
using Vokabular.MainService.DataContracts.Contracts;

namespace Vokabular.MainService.Core.Managers
{
    public class ProjectMetadataManager
    {
        private readonly ProjectRepository m_projectRepository;

        public ProjectMetadataManager(ProjectRepository projectRepository)
        {
            m_projectRepository = projectRepository;
        }

        public int CreatePublisher(PublisherContract data)
        {
            return new CreatePublisherWork(m_projectRepository, data).Execute();
        }

        public int CreateLiteraryKind(string name)
        {
            return new CreateLiteraryKindWork(m_projectRepository, name).Execute();
        }

        public int CreateLiteraryGenre(string name)
        {
            return new CreateLiteraryGenreWork(m_projectRepository, name).Execute();
        }
    }
}
