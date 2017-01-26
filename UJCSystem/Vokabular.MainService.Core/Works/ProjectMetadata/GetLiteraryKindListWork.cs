using System.Collections.Generic;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;

namespace Vokabular.MainService.Core.Works.ProjectMetadata
{
    public class GetLiteraryKindListWork : UnitOfWorkBase<IList<LiteraryKind>>
    {
        private readonly MetadataRepository m_metadataRepository;

        public GetLiteraryKindListWork(MetadataRepository metadataRepository) : base(metadataRepository)
        {
            m_metadataRepository = metadataRepository;
        }

        protected override IList<LiteraryKind> ExecuteWorkImplementation()
        {
            return m_metadataRepository.GetLiteraryKindList();
        }
    }
}