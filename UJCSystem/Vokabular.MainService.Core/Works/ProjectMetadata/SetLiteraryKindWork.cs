using System.Collections.Generic;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;

namespace Vokabular.MainService.Core.Works.ProjectMetadata
{
    public class SetLiteraryKindWork : UnitOfWorkBase
    {
        private readonly ProjectRepository m_projectRepository;
        private readonly long m_projectId;
        private readonly IList<int> m_kindIdList;

        public SetLiteraryKindWork(ProjectRepository projectRepository, long projectId, IList<int> kindIdList) : base(projectRepository)
        {
            m_projectRepository = projectRepository;
            m_projectId = projectId;
            m_kindIdList = kindIdList;
        }

        protected override void ExecuteWorkImplementation()
        {
            var literaryKindList = new List<LiteraryKind>();
            foreach (var id in m_kindIdList)
            {
                var literaryKind = m_projectRepository.Load<LiteraryKind>(id);
                literaryKindList.Add(literaryKind);
            }

            var project = m_projectRepository.Load<Project>(m_projectId);
            project.LiteraryKinds = literaryKindList;

            m_projectRepository.Update(project);
        }
    }
}