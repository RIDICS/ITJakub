using System.Collections.Generic;
using System.Linq;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.MainService.Core.Works.ProjectMetadata
{
    public class SetAuthorsWork : UnitOfWorkBase
    {
        private readonly ProjectRepository m_projectRepository;
        private readonly long m_projectId;
        private readonly IList<int> m_authorIdList;

        public SetAuthorsWork(ProjectRepository projectRepository, long projectId, IList<int> authorIdList) : base(projectRepository)
        {
            m_projectRepository = projectRepository;
            m_projectId = projectId;
            m_authorIdList = authorIdList;
        }

        protected override void ExecuteWorkImplementation()
        {
            var dbProjectAuthorList = m_projectRepository.GetProjectOriginalAuthorList(m_projectId);
            var project = m_projectRepository.Load<Project>(m_projectId);

            var itemsToDelete = new List<ProjectOriginalAuthor>();
            foreach (var projectAuthor in dbProjectAuthorList)
            {
                if (!m_authorIdList.Contains(projectAuthor.OriginalAuthor.Id))
                {
                    itemsToDelete.Add(projectAuthor);
                }
            }

            m_projectRepository.DeleteAll(itemsToDelete);

            var order = 1;
            foreach (var newAuthorId in m_authorIdList)
            {
                var projectAuthor = dbProjectAuthorList.SingleOrDefault(x => x.OriginalAuthor.Id == newAuthorId);
                if (projectAuthor == null)
                {
                    projectAuthor = new ProjectOriginalAuthor
                    {
                        Project = project,
                        OriginalAuthor = m_projectRepository.Load<OriginalAuthor>(newAuthorId),
                        Sequence = order++
                    };
                    m_projectRepository.Create(projectAuthor);
                }
                else
                {
                    projectAuthor.Sequence = order++;
                    m_projectRepository.Update(projectAuthor);
                }
            }
        }
    }
}