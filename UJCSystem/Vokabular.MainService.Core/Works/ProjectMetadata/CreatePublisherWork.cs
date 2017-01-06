using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;
using Vokabular.MainService.DataContracts.Contracts;

namespace Vokabular.MainService.Core.Works.ProjectMetadata
{
    public class CreatePublisherWork : UnitOfWorkBase<int>
    {
        private readonly ProjectRepository m_projectRepository;
        private readonly PublisherContract m_data;

        public CreatePublisherWork(ProjectRepository projectRepository, PublisherContract data) : base(projectRepository.UnitOfWork)
        {
            m_projectRepository = projectRepository;
            m_data = data;
        }

        protected override int ExecuteWorkImplementation()
        {
            var publisher = new DataEntities.Database.Entities.Publisher
            {
                Text = m_data.Text,
                Email = m_data.Email
            };
            return (int) m_projectRepository.Create(publisher);
        }
    }
}
