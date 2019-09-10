using System.Collections.Generic;
using System.Linq;
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
            var literaryGenreList = m_genreIdList.Distinct().Select(id => m_projectRepository.Load<LiteraryGenre>(id)).ToList();

            var project = m_projectRepository.Load<Project>(m_projectId);
            project.LiteraryGenres = literaryGenreList;

            m_projectRepository.Update(project);
        }
    }
}