using System.Collections.Generic;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.Shared.Contracts.Searching.Criteria;
using NHibernate.Criterion;

namespace ITJakub.DataEntities.Database
{
    public class HeadwordCriteriaQueryCreator
    {
        private readonly List<ConjunctionCriteria> m_criteriaList;

        public HeadwordCriteriaQueryCreator()
        {
            m_criteriaList = new List<ConjunctionCriteria>();
        }

        public void AddCriteria(List<SearchCriteriaContract> criteriaList)
        {
            foreach (var searchCriteriaContract in criteriaList)
            {
                AddCriteria(searchCriteriaContract);
            }
        }

        public void AddCriteria(SearchCriteriaContract contract)
        {
            var wordListCriteria = contract as WordListCriteriaContract;
            if (contract.Key != CriteriaKey.Headword || wordListCriteria == null)
                return;

            var newCriteria = new ConjunctionCriteria();
            foreach (var wordCriteria in wordListCriteria.Disjunctions)
            {
                newCriteria.Disjunctions.Add(CriteriaConditionBuilder.Create(wordCriteria));
            }

            m_criteriaList.Add(newCriteria);
        }

        public Conjunction GetCondition()
        {
            BookHeadword bookHeadwordAlias = null;
            var conjunction = new Conjunction();

            foreach (var criteria in m_criteriaList)
            {
                var disjunction = new Disjunction();
                foreach (var conditionString in criteria.Disjunctions)
                {
                    disjunction.Add(new LikeExpression(Projections.Property(() => bookHeadwordAlias.Headword),
                        conditionString, MatchMode.Exact));
                }
                conjunction.Add(disjunction);
            }

            return conjunction;
        }

        private class ConjunctionCriteria
        {
            public ConjunctionCriteria()
            {
                Disjunctions = new List<string>();
            }

            public List<string> Disjunctions { get; private set; }
        }
    }
}