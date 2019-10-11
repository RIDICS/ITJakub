using System.Collections.Generic;
using System.Linq;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.MainService.Core.Works.ProjectMetadata
{
    public class SetKeywordsWork : UnitOfWorkBase
    {
        private readonly ProjectRepository m_projectRepository;
        private readonly long m_projectId;
        private readonly IList<int> m_keywordIdList;

        public SetKeywordsWork(ProjectRepository projectRepository, long projectId, IList<int> keywordIdList) : base(projectRepository)
        {
            m_projectRepository = projectRepository;
            m_projectId = projectId;
            m_keywordIdList = keywordIdList;
        }

        protected override void ExecuteWorkImplementation()
        {
            var keywordList = m_keywordIdList.Distinct().Select(id => m_projectRepository.Load<Keyword>(id)).ToList();

            var project = m_projectRepository.Load<Project>(m_projectId);
            project.Keywords = keywordList;

            m_projectRepository.Update(project);
        }
    }
}