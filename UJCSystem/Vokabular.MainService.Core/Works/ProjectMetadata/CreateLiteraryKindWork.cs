using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;

namespace Vokabular.MainService.Core.Works.ProjectMetadata
{
    public class CreateLiteraryKindWork : UnitOfWorkBase<int>
    {
        private readonly ProjectRepository m_projectRepository;
        private readonly string m_name;

        public CreateLiteraryKindWork(ProjectRepository projectRepository, string name) : base(projectRepository.UnitOfWork)
        {
            m_projectRepository = projectRepository;
            m_name = name;
        }

        protected override int ExecuteWorkImplementation()
        {
            var literaryKind = new DataEntities.Database.Entities.LiteraryKind
            {
                Name = m_name
            };
            return (int) m_projectRepository.Create(literaryKind);
        }
    }
}