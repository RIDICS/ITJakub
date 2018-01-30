using System.Collections.Generic;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;

namespace Vokabular.MainService.Core.Works.ProjectMetadata
{
    public class SetLiteraryOriginalWork : UnitOfWorkBase
    {
        private readonly ProjectRepository m_projectRepository;
        private readonly long m_projectId;
        private readonly IList<int> m_originalIdList;

        public SetLiteraryOriginalWork(ProjectRepository projectRepository, long projectId, IList<int> originalIdList) : base(projectRepository)
        {
            m_projectRepository = projectRepository;
            m_projectId = projectId;
            m_originalIdList = originalIdList;
        }

        protected override void ExecuteWorkImplementation()
        {
            var literaryOriginalList = new List<LiteraryOriginal>();
            foreach (var id in m_originalIdList)
            {
                var literaryOriginal = m_projectRepository.Load<LiteraryOriginal>(id);
                literaryOriginalList.Add(literaryOriginal);
            }

            var project = m_projectRepository.Load<Project>(m_projectId);
            project.LiteraryOriginals = literaryOriginalList;

            m_projectRepository.Update(project);
        }
    }
}