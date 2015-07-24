using System.Collections.Generic;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.Shared.Contracts.Searching.Criteria;
using NHibernate;
using NHibernate.Criterion;

namespace ITJakub.DataEntities.Database
{
    public class HeadwordCriteriaQueryCreator
    {
        private List<Criteria> m_criteriaList;

        public HeadwordCriteriaQueryCreator()
        {
            m_criteriaList = new List<Criteria>();
        }

        public void AddCriteria(SearchCriteriaContract contract)
        {
            var wordListCriteria = contract as WordListCriteriaContract;
            if (contract.Key != CriteriaKey.Headword || wordListCriteria == null)
                return;

            var newCriteria = new Criteria();
            foreach (var wordCriteria in wordListCriteria.Disjunctions)
            {
                newCriteria.Disjunctions.Add(CriteriaConditionBuilder.Create(wordCriteria));
            }

            m_criteriaList.Add(newCriteria);
        }

        public void SetConditions(IQueryOver<Book, BookHeadword> query, BookHeadword bookHeadwordAlias)
        {
            //foreach (var criteria in m_criteriaList)
            //{
            //    new Conjunction().Add(() => )
            //    query.And(NHibernate.Criterion.)
            //}
        }

        public Conjunction GetCondition(BookHeadword bookHeadwordAlias)
        {
            return null;
        }

        private class Criteria
        {
            public List<string> Disjunctions { get; set; }
        }
    }
}