using System.Collections.Generic;
using System.Linq;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.Search;
using Vokabular.DataEntities.Database.UnitOfWork;

namespace Vokabular.MainService.Core.Works.Search
{
    public class SearchHeadwordByCriteriaWork : UnitOfWorkBase<IList<HeadwordResource>>
    {
        private readonly MetadataRepository m_metadataRepository;
        private readonly BookRepository m_bookRepository;
        private readonly SearchCriteriaQueryCreator m_queryCreator;

        public SearchHeadwordByCriteriaWork(MetadataRepository metadataRepository, BookRepository bookRepository, SearchCriteriaQueryCreator queryCreator) : base(metadataRepository)
        {
            m_metadataRepository = metadataRepository;
            m_bookRepository = bookRepository;
            m_queryCreator = queryCreator;
        }

        protected override IList<HeadwordResource> ExecuteWorkImplementation()
        {
            var headwordsDbResult = m_bookRepository.SearchHeadwordByCriteriaQuery(m_queryCreator);
            var headwordIds = headwordsDbResult.Select(x => x.Id).ToList();
            var headwords = m_metadataRepository.GetHeadwordWithFetch(headwordIds);

            var resultHeadwords = new List<HeadwordResource>(headwords.Count);
            foreach (var headwordId in headwordIds)
            {
                var headword = headwords.First(x => x.Id == headwordId);
                resultHeadwords.Add(headword);
            }
            
            return resultHeadwords;
        }
    }
}