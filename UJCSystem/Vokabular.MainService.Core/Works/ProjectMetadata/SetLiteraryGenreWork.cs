using System.Collections.Generic;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.MainService.Core.Works.ProjectMetadata
{
    public class SetLiteraryGenreWork : UnitOfWorkBase
    {
        private readonly ProjectRepository m_projectRepository;
        private readonly long m_projectId;
        private readonly IList<int> m_genreIdList;
        
        public SetLiteraryGenreWork(ProjectRepository projectRepository, long projectId, IList<int> genreIdList) : base(projectRepository)
        {
            m_projectRepository = projectRepository;
            m_projectId = projectId;
            m_genreIdList = genreIdList;
        }

        protected override void ExecuteWorkImplementation()
        {
            var literaryGenreList = new List<LiteraryGenre>();
            foreach (var id in m_genreIdList)
            {
                var literaryGenre = m_projectRepository.Load<LiteraryGenre>(id);
                literaryGenreList.Add(literaryGenre);
            }

            var project = m_projectRepository.Load<Project>(m_projectId);
            project.LiteraryGenres = literaryGenreList;

            m_projectRepository.Update(project);
        }
    }
}