using System.Collections.Generic;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;

namespace Vokabular.MainService.Core.Works
{
    public class GetProjectListWork : UnitOfWorkBase<IList<Project>>
    {
        private readonly ProjectRepository m_projectRepository;
        private readonly int m_start;
        private readonly int m_count;
        private int m_resultCount;

        public GetProjectListWork(ProjectRepository projectRepository, int start, int count) : base(projectRepository.UnitOfWork)
        {
            m_projectRepository = projectRepository;
            m_start = start;
            m_count = count;
        }

        protected override IList<Project> ExecuteWorkImplementation()
        {
            m_resultCount = m_projectRepository.GetProjectCount();
            return m_projectRepository.GetProjectList(m_start, m_count);
        }

        public int GetResultCount()
        {
            return m_resultCount;
        }
    }
}