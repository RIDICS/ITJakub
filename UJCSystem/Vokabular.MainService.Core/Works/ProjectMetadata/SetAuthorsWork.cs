using System.Collections.Generic;
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

        public SetAuthorsWork(MetadataRepository metadataRepository, long projectId, IList<int> authorIdList) : base(metadataRepository.UnitOfWork)
        {
            m_metadataRepository = metadataRepository;
            m_projectId = projectId;
            m_authorIdList = authorIdList;
        }

        protected override void ExecuteWorkImplementation()
        {
            var authorList = new List<OriginalAuthor>();
            foreach (var id in m_authorIdList)
            {
                var author = m_metadataRepository.Load<OriginalAuthor>(id);
                authorList.Add(author);
            }

            var project = m_metadataRepository.Load<Project>(m_projectId);
            project.Authors = authorList;

            m_metadataRepository.Update(project);
        }
    }
}