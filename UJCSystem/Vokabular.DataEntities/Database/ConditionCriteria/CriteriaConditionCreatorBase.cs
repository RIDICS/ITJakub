using System.Collections.Generic;
using System.Text;
using Vokabular.DataEntities.Database.Search;
using Vokabular.Shared.DataContracts.Search.Criteria;
using Vokabular.Shared.DataContracts.Types;

namespace Vokabular.DataEntities.Database.ConditionCriteria
{
    public abstract class CriteriaConditionCreatorBase
    {
        protected readonly List<ConjunctionCriteria> CriteriaList;

        protected CriteriaConditionCreatorBase()
        {
            CriteriaList = new List<ConjunctionCriteria>();
        }

        protected abstract CriteriaKey CriteriaKey { get; }

        //public abstract ICriterion GetCondition();

        public void AddCriteria(IList<SearchCriteriaContract> criteriaList)
        {
            foreach (var searchCriteriaContract in criteriaList)
            {
                AddCriteria(searchCriteriaContract);
            }
        }
        
        public void AddCriteria(SearchCriteriaContract contract)
        {
            var wordListCriteria = contract as WordListCriteriaContract;
            if (contract.Key != CriteriaKey || wordListCriteria == null)
                return;

            var newCriteria = new ConjunctionCriteria();
            foreach (var wordCriteria in wordListCriteria.Disjunctions)
            {
                newCriteria.Disjunctions.Add(CriteriaConditionBuilder.Create(wordCriteria));
            }

            CriteriaList.Add(newCriteria);
        }
        

        protected class ConjunctionCriteria
        {
            public ConjunctionCriteria()
            {
                Disjunctions = new List<string>();
            }

            public List<string> Disjunctions { get; }
        }
    }

    public class TermCriteriaPageConditionCreator : CriteriaConditionCreatorBase, ISearchCriteriaCreator
    {
        public TermCriteriaPageConditionCreator()
        {
            Parameters = new Dictionary<string, object>();
        }

        protected override CriteriaKey CriteriaKey => CriteriaKey.Term;

        public Dictionary<string, object> Parameters { get; }

        public void SetProjectIds(IEnumerable<long> projectIds)
        {
            Parameters["projectIds"] = projectIds;
        }

        public string GetQueryString()
        {
            var whereBuilder = new StringBuilder();
            var joinBuilder = new StringBuilder();
            var paramNumber = 1;

            for (var i = 0; i < CriteriaList.Count; i++)
            {
                var criteria = CriteriaList[i];
                var termAlias = $"term{i}";

                whereBuilder
                    .Append(" and ")
                    .Append('(');

                for (var j = 0; j < criteria.Disjunctions.Count; j++)
                {
                    var conditionString = criteria.Disjunctions[j];

                    var paramAlias = $"param{paramNumber}";
                    Parameters.Add(paramAlias, conditionString);

                    if (j > 0)
                        whereBuilder.Append(" or ");

                    whereBuilder.AppendFormat("{0}.Text like :{1}", termAlias, paramAlias);

                    paramNumber++;
                }

                whereBuilder.Append(')');

                joinBuilder.AppendFormat(" inner join page.Terms {0}", termAlias);
            }

            return $"select page from PageResource page join fetch page.Resource resource {joinBuilder} where resource.LatestVersion.Id = page.Id and resource.Project.Id in (:projectIds) {whereBuilder} order by resource.Project.Id, page.Position";
        }
    }
}