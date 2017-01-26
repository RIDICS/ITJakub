using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;

namespace Vokabular.MainService.Core.Works
{
    public class GetProjectWork : UnitOfWorkBase<Project>
    {
        private readonly ProjectRepository m_projectRepository;
        private readonly long m_projectId;

        public GetProjectWork(ProjectRepository projectRepository, long projectId) : base(projectRepository)
        {
            m_projectRepository = projectRepository;
            m_projectId = projectId;
        }

        protected override Project ExecuteWorkImplementation()
        {
            return m_projectRepository.GetProject(m_projectId);
        }
    }
}