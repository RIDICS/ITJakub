using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.MainService.Core.Works.ProjectGroups
{
    public class RemoveProjectFromGroupWork : UnitOfWorkBase
    {
        private readonly ProjectRepository m_projectRepository;
        private readonly long m_projectId;

        public RemoveProjectFromGroupWork(ProjectRepository projectRepository, long projectId) : base(projectRepository)
        {
            m_projectRepository = projectRepository;
            m_projectId = projectId;
        }

        protected override void ExecuteWorkImplementation()
        {
            var projectGroup = m_projectRepository.GetProjectGroupWithProjects(m_projectId);
            if (projectGroup == null)
            {
                return;
            }

            var project = m_projectRepository.FindById<Project>(m_projectId);

            project.ProjectGroup = null;

            m_projectRepository.Update(project);

            if (projectGroup.Projects.Count <= 1)
            {
                m_projectRepository.Delete(projectGroup);
            }
        }
    }
}