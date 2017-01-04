using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;

namespace Vokabular.MainService.Core.Works
{
    public class GetProjectWork : UnitOfWorkBase
    {
        private readonly ProjectRepository m_projectRepository;
        private readonly long m_projectId;
        private Project m_result;

        public GetProjectWork(ProjectRepository projectRepository, long projectId) : base(projectRepository.UnitOfWork)
        {
            m_projectRepository = projectRepository;
            m_projectId = projectId;
        }

        protected override void ExecuteWorkImplementation()
        {
            m_result = m_projectRepository.GetProject(m_projectId);
        }

        public Project GetResult()
        {
            return m_result;
        }
    }
}