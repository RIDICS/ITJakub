using System.Collections.Generic;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;

namespace Vokabular.MainService.Core.Works.ProjectMetadata
{
    public class SetLiteraryGenreWork : UnitOfWorkBase
    {
        private readonly MetadataRepository m_metadataRepository;
        private readonly long m_projectId;
        private readonly IList<int> m_genreIdList;
        
        public SetLiteraryGenreWork(MetadataRepository metadataRepository, long projectId, IList<int> genreIdList) : base(metadataRepository.UnitOfWork)
        {
            m_metadataRepository = metadataRepository;
            m_projectId = projectId;
            m_genreIdList = genreIdList;
        }

        protected override void ExecuteWorkImplementation()
        {
            var literaryGenreList = new List<LiteraryGenre>();
            foreach (var id in m_genreIdList)
            {
                var literaryGenre = m_metadataRepository.Load<LiteraryGenre>(id);
                literaryGenreList.Add(literaryGenre);
            }

            var project = m_metadataRepository.Load<Project>(m_projectId);
            project.LiteraryGenres = literaryGenreList;

            m_metadataRepository.Update(project);
        }
    }
}