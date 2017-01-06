using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;

namespace Vokabular.MainService.Core.Works.ProjectMetadata
{
    public class CreateLiteraryGenreWork : UnitOfWorkBase<int>
    {
        private readonly ProjectRepository m_projectRepository;
        private readonly string m_name;

        public CreateLiteraryGenreWork(ProjectRepository projectRepository, string name) : base(projectRepository.UnitOfWork)
        {
            m_projectRepository = projectRepository;
            m_name = name;
        }

        protected override int ExecuteWorkImplementation()
        {
            var literaryGenre = new DataEntities.Database.Entities.LiteraryGenre
            {
                Name = m_name
            };
            return (int) m_projectRepository.Create(literaryGenre);
        }
    }
}