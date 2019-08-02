using System.Collections.Generic;
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
            var keywordList = new List<Keyword>();
            foreach (var id in m_keywordIdList)
            {
                var keyword = m_projectRepository.Load<Keyword>(id);
                keywordList.Add(keyword);
            }

            var project = m_projectRepository.Load<Project>(m_projectId);
            project.Keywords = keywordList;

            m_projectRepository.Update(project);
        }
    }
}