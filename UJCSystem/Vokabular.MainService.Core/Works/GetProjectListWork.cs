using System.Collections.Generic;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;

namespace Vokabular.MainService.Core.Works
{
    public class GetProjectListWork : UnitOfWorkBase
    {
        private readonly ProjectRepository m_projectRepository;
        private IList<Project> m_result;

        public GetProjectListWork(ProjectRepository projectRepository) : base(projectRepository.UnitOfWork)
        {
            m_projectRepository = projectRepository;
        }

        protected override void ExecuteWorkImplementation()
        {
            m_result = m_projectRepository.GetProjectList();
        }

        public IList<Project> GetResult()
        {
            return m_result;
        }
    }
}