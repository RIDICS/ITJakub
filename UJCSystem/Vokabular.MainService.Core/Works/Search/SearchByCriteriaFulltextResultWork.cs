using System.Collections.Generic;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.SelectResults;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;

namespace Vokabular.MainService.Core.Works.Search
{
    public class SearchByCriteriaFulltextResultWork : UnitOfWorkBase<IList<MetadataResource>>
    {
        private readonly MetadataRepository m_metadataRepository;
        private readonly IList<long> m_projectIdList;

        public SearchByCriteriaFulltextResultWork(MetadataRepository metadataRepository, IList<long> projectIdList) : base(metadataRepository)
        {
            m_metadataRepository = metadataRepository;
            m_projectIdList = projectIdList;
        }

        protected override IList<MetadataResource> ExecuteWorkImplementation()
        {
            var metadataList = m_metadataRepository.GetMetadataWithFetchForBiblModuleByProject(m_projectIdList);

            PageCounts = m_metadataRepository.GetPageCount(m_projectIdList);

            return metadataList;
        }

        public IList<PageCountResult> PageCounts { get; set; }
    }
}