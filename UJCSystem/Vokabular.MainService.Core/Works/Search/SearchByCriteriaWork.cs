using System.Collections.Generic;
using System.Linq;
using Vokabular.DataEntities.Database.ConditionCriteria;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.SelectResults;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.Search;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.MainService.Core.Works.Search
{
    public class SearchByCriteriaWork : UnitOfWorkBase<IList<MetadataResource>>
    {
        private readonly MetadataRepository m_metadataRepository;
        private readonly BookRepository m_bookRepository;
        private readonly SearchCriteriaQueryCreator m_queryCreator;
        private readonly TermCriteriaPageConditionCreator m_termCriteriaCreator;

        public SearchByCriteriaWork(MetadataRepository metadataRepository, BookRepository bookRepository,
            SearchCriteriaQueryCreator queryCreator, TermCriteriaPageConditionCreator termCriteriaCreator) : base(
            metadataRepository)
        {
            m_metadataRepository = metadataRepository;
            m_bookRepository = bookRepository;
            m_queryCreator = queryCreator;
            m_termCriteriaCreator = termCriteriaCreator;
        }

        protected override IList<MetadataResource> ExecuteWorkImplementation()
        {
            var metadataList = m_bookRepository.SearchByCriteriaQuery(m_queryCreator);

            var metadataIdList = metadataList.Select(x => x.Id);
            m_metadataRepository.GetMetadataWithFetchForBiblModule(metadataIdList);

            var projectIdList = metadataList.Select(x => x.Resource.Project.Id).ToList();
            PageCounts = m_metadataRepository.GetPageCount(projectIdList);
            
            if (m_termCriteriaCreator != null)
            {
                m_termCriteriaCreator.SetProjectIds(projectIdList);
                TermHits = projectIdList.Count > 0
                    ? m_metadataRepository.GetPagesWithTerms(m_termCriteriaCreator)
                    : new List<PageResource>();
            }

            return metadataList;
        }

        public IList<PageCountResult> PageCounts { get; set; }

        public IList<PageResource> TermHits { get; set; }
    }
}
