using System.Collections.Generic;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.Shared.Contracts.Searching.Criteria;
using NHibernate.Criterion;

namespace ITJakub.DataEntities.Database
{
    public abstract class ConcreteCriteriaQueryCreatorBase
    {
        protected readonly List<ConjunctionCriteria> m_criteriaList;

        protected ConcreteCriteriaQueryCreatorBase()
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

        protected class ConjunctionCriteria
        {
            public ConjunctionCriteria()
            {
                Disjunctions = new List<string>();
            }

            public List<string> Disjunctions { get; private set; }
        }


        public void AddCriteria(SearchCriteriaContract contract)
        {
            var wordListCriteria = contract as WordListCriteriaContract;
            if (contract.Key != GetCriteriaKey() || wordListCriteria == null)
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
            var conjunction = new Conjunction();

            foreach (var criteria in m_criteriaList)
            {
                var disjunction = new Disjunction();
                foreach (var conditionString in criteria.Disjunctions)
                {
                    disjunction.Add(GetConditionCriterion(conditionString));
                }
                conjunction.Add(disjunction);
            }

            return conjunction;
        }

        public abstract ICriterion GetConditionCriterion(string conditionString);
        protected abstract CriteriaKey GetCriteriaKey();
    }


    public class HeadwordCriteriaQueryCreator : ConcreteCriteriaQueryCreatorBase
    {
        protected override CriteriaKey GetCriteriaKey()
        {
            return CriteriaKey.Headword;
        }

        public override ICriterion GetConditionCriterion(string conditionString)
        {
            BookHeadword bookHeadwordAlias = null;
            return new LikeExpression(Projections.Property(() => bookHeadwordAlias.Headword), conditionString, MatchMode.Exact);
        }
    }

    public class TermCriteriaQueryCreator : ConcreteCriteriaQueryCreatorBase
    {
        protected override CriteriaKey GetCriteriaKey()
        {
            return CriteriaKey.Term;
        }

        public override ICriterion GetConditionCriterion(string conditionString)
        {
            Term termAlias = null;
            return new LikeExpression(Projections.Property(() => termAlias.Text), conditionString, MatchMode.Exact);
        }
    }
}