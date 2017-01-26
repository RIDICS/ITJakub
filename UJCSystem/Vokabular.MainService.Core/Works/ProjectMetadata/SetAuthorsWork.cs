using System.Collections.Generic;
using System.Linq;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;

namespace Vokabular.MainService.Core.Works.ProjectMetadata
{
    public class SetAuthorsWork : UnitOfWorkBase
    {
        private readonly MetadataRepository m_metadataRepository;
        private readonly long m_projectId;
        private readonly IList<int> m_authorIdList;

        public SetAuthorsWork(MetadataRepository metadataRepository, long projectId, IList<int> authorIdList) : base(metadataRepository)
        {
            m_metadataRepository = metadataRepository;
            m_projectId = projectId;
            m_authorIdList = authorIdList;
        }

        protected override void ExecuteWorkImplementation()
        {
            var dbProjectAuthorList = m_metadataRepository.GetProjectOriginalAuthorList(m_projectId);
            var project = m_metadataRepository.Load<Project>(m_projectId);

            var itemsToDelete = new List<ProjectOriginalAuthor>();
            foreach (var projectAuthor in dbProjectAuthorList)
            {
                if (!m_authorIdList.Contains(projectAuthor.OriginalAuthor.Id))
                {
                    itemsToDelete.Add(projectAuthor);
                }
            }

            m_metadataRepository.DeleteAll(itemsToDelete);

            var order = 1;
            foreach (var newAuthorId in m_authorIdList)
            {
                var projectAuthor = dbProjectAuthorList.SingleOrDefault(x => x.OriginalAuthor.Id == newAuthorId);
                if (projectAuthor == null)
                {
                    projectAuthor = new ProjectOriginalAuthor
                    {
                        Project = project,
                        OriginalAuthor = m_metadataRepository.Load<OriginalAuthor>(newAuthorId),
                        Sequence = order++
                    };
                    m_metadataRepository.Create(projectAuthor);
                }
                else
                {
                    projectAuthor.Sequence = order++;
                    m_metadataRepository.Update(projectAuthor);
                }
            }
        }
    }
}