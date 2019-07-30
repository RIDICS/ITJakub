using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.MainService.Core.Works
{
    public class SetForumIdToProjectWork : UnitOfWorkBase
    {
        private readonly ProjectRepository m_projectRepository;
        private readonly long m_projectId;
        private readonly int m_forumId;

        public SetForumIdToProjectWork(ProjectRepository projectRepository, long projectId, int forumId) : base(projectRepository)
        {
            m_projectRepository = projectRepository;
            m_projectId = projectId;
            m_forumId = forumId;
        }

        protected override void ExecuteWorkImplementation()
        {
            var project = m_projectRepository.Load<Project>(m_projectId);
            project.ForumId = m_forumId;

            m_projectRepository.Update(project);
        }
    }
}
