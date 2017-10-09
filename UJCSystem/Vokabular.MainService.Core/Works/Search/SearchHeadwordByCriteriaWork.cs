using System.Collections.Generic;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.Search;
using Vokabular.DataEntities.Database.UnitOfWork;

namespace Vokabular.MainService.Core.Works.Search
{
    public class SearchHeadwordByCriteriaWork : UnitOfWorkBase<IList<HeadwordResource>>
    {
        private readonly MetadataRepository m_metadataRepository;
        private readonly SearchCriteriaQueryCreator m_queryCreator;

        public SearchHeadwordByCriteriaWork(MetadataRepository metadataRepository, SearchCriteriaQueryCreator queryCreator) : base(metadataRepository)
        {
            m_metadataRepository = metadataRepository;
            m_queryCreator = queryCreator;
        }

        protected override IList<HeadwordResource> ExecuteWorkImplementation()
        {
            var headwordList = m_metadataRepository.SearchHeadwordByCriteriaQuery(m_queryCreator);
            
            // TODO fetch HeadwordItems in second query

            return headwordList;
        }
    }
}