using System.Collections.Generic;
using System.Linq;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.Shared.DataEntities.UnitOfWork;

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
            var literaryKindList = m_kindIdList.Distinct().Select(id => m_projectRepository.Load<LiteraryKind>(id)).ToList();

            var project = m_projectRepository.Load<Project>(m_projectId);
            project.LiteraryKinds = literaryKindList;

            m_projectRepository.Update(project);
        }
    }
}