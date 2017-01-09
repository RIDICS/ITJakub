using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;
using Vokabular.MainService.DataContracts.Contracts;

namespace Vokabular.MainService.Core.Works.ProjectMetadata
{
    public class CreatePublisherWork : UnitOfWorkBase<int>
    {
        private readonly MetadataRepository m_metadataRepository;
        private readonly PublisherContract m_data;

        public CreatePublisherWork(MetadataRepository metadataRepository, PublisherContract data) : base(metadataRepository.UnitOfWork)
        {
            m_metadataRepository = metadataRepository;
            m_data = data;
        }

        protected override int ExecuteWorkImplementation()
        {
            var publisher = new Publisher
            {
                Text = m_data.Text,
                Email = m_data.Email
            };
            return (int) m_metadataRepository.Create(publisher);
        }
    }
}
