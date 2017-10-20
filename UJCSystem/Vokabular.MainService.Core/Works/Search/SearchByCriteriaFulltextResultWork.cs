using System.Collections.Generic;
using Vokabular.DataEntities.Database.ConditionCriteria;
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
        private readonly TermCriteriaConditionCreator m_termCriteriaCreator;

        public SearchByCriteriaFulltextResultWork(MetadataRepository metadataRepository, IList<long> projectIdList, TermCriteriaConditionCreator termCriteriaCreator) : base(metadataRepository)
        {
            m_metadataRepository = metadataRepository;
            m_projectIdList = projectIdList;
            m_termCriteriaCreator = termCriteriaCreator;
        }

        protected override IList<MetadataResource> ExecuteWorkImplementation()
        {
            var metadataList = m_metadataRepository.GetMetadataWithFetchForBiblModuleByProject(m_projectIdList);

            PageCounts = m_metadataRepository.GetPageCount(m_projectIdList);

            if (m_termCriteriaCreator != null)
            {
                m_termCriteriaCreator.SetProjectIds(m_projectIdList);
                TermHits = m_projectIdList.Count > 0
                    ? m_metadataRepository.GetPagesWithTerms(m_termCriteriaCreator)
                    : new List<PageResource>();
            }

            return metadataList;
        }

        public IList<PageCountResult> PageCounts { get; set; }

        public IList<PageResource> TermHits { get; set; }
    }
}