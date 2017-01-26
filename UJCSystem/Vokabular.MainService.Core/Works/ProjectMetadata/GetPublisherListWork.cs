using System.Collections.Generic;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;

namespace Vokabular.MainService.Core.Works.ProjectMetadata
{
    public class GetPublisherListWork : UnitOfWorkBase<IList<Publisher>>
    {
        private readonly MetadataRepository m_metadataRepository;

        public GetPublisherListWork(MetadataRepository metadataRepository) : base(metadataRepository)
        {
            m_metadataRepository = metadataRepository;
        }

        protected override IList<Publisher> ExecuteWorkImplementation()
        {
            return m_metadataRepository.GetPublisherList();
        }
    }
}