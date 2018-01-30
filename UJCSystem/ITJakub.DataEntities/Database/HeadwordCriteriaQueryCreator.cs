using System.Collections.Generic;
using ITJakub.DataEntities.Database.Entities;
using NHibernate.Criterion;
using Vokabular.Shared.DataContracts.Search.Criteria;
using Vokabular.Shared.DataContracts.Types;

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

        protected abstract CriteriaKey GetCriteriaKey();
        public abstract ICriterion GetCondition();
    }


    public class HeadwordCriteriaQueryCreator : ConcreteCriteriaQueryCreatorBase
    {
        protected override CriteriaKey GetCriteriaKey()
        {
            return CriteriaKey.Headword;
        }

        public override ICriterion GetCondition()
        {
            var conjunction = new Conjunction();
            BookHeadword bookHeadwordAlias = null;

            foreach (var criteria in m_criteriaList)
            {
                var disjunction = new Disjunction();
                foreach (var conditionString in criteria.Disjunctions)
                {
                    disjunction.Add(new LikeExpression(Projections.Property(() => bookHeadwordAlias.Headword), conditionString, MatchMode.Exact));
                }
                conjunction.Add(disjunction);
            }

            return conjunction;
        }
    }

    public class TermCriteriaQueryCreator : ConcreteCriteriaQueryCreatorBase
    {
        protected override CriteriaKey GetCriteriaKey()
        {
            return CriteriaKey.Term;
        }

        public override ICriterion GetCondition()
        {
            var parentDisjunction = new Disjunction();
            Term termAlias = null;

            foreach (var criteria in m_criteriaList)
            {
                var disjunction = new Disjunction();
                foreach (var conditionString in criteria.Disjunctions)
                {
                    disjunction.Add(new LikeExpression(Projections.Property(() => termAlias.Text), conditionString, MatchMode.Exact));
                }
                parentDisjunction.Add(disjunction);
            }

            return parentDisjunction;
        }
    }
}