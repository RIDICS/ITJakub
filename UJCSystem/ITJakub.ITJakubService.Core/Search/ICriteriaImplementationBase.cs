using System;
using System.Collections.Generic;
using System.Text;
using ITJakub.DataEntities.Database;
using ITJakub.Shared.Contracts.Searching.Criteria;

namespace ITJakub.ITJakubService.Core.Search
{
    public interface ICriteriaImplementationBase
    {
        CriteriaKey CriteriaKey { get; }
        SearchCriteriaQuery CreateCriteriaQuery(SearchCriteriaContract searchCriteriaContract);
    }

    public class AuthorCriteriaImplementation : ICriteriaImplementationBase
    {
        public CriteriaKey CriteriaKey
        {
            get { return CriteriaKey.Author; }
        }

        public SearchCriteriaQuery CreateCriteriaQuery(SearchCriteriaContract searchCriteriaContract)
        {
            var wordListCriteria = (WordListCriteriaContract) searchCriteriaContract;
            var authorAlias = string.Format("a{0}", Guid.NewGuid().ToString("N"));
            var whereBuilder = new StringBuilder();
            var parameters = new List<object>();

            foreach (WordCriteriaContract wordCriteria in wordListCriteria.Disjunctions)
            {
                if (whereBuilder.Length > 0)
                    whereBuilder.Append(" or");

                whereBuilder.AppendFormat(" {0}.Name like ?", authorAlias);
                parameters.Add(CriteriaConditionBuilder.Create(wordCriteria));
            }

            return new SearchCriteriaQuery
            {
                Join = string.Format("inner join bv.Authors {0}", authorAlias),
                Where = whereBuilder.ToString(),
                Parameters = parameters
            };
        }
    }

    public class TitleCriteriaImplementation : ICriteriaImplementationBase
    {
        public CriteriaKey CriteriaKey
        {
            get { return CriteriaKey.Title; }
        }

        public SearchCriteriaQuery CreateCriteriaQuery(SearchCriteriaContract searchCriteriaContract)
        {
            var wordListCriteria = (WordListCriteriaContract) searchCriteriaContract;
            var whereBuilder = new StringBuilder();
            var parameters = new List<object>();

            foreach (WordCriteriaContract wordCriteria in wordListCriteria.Disjunctions)
            {
                if (whereBuilder.Length > 0)
                    whereBuilder.Append(" or");

                whereBuilder.Append(" bv.Title like ?");
                parameters.Add(CriteriaConditionBuilder.Create(wordCriteria));
            }

            return new SearchCriteriaQuery
            {
                Join = string.Empty,
                Where = whereBuilder.ToString(),
                Parameters = parameters
            };
        }
    }

    public class EditorCriteriaImplementation : ICriteriaImplementationBase
    {
        public CriteriaKey CriteriaKey
        {
            get { return CriteriaKey.Editor; }
        }

        public SearchCriteriaQuery CreateCriteriaQuery(SearchCriteriaContract searchCriteriaContract)
        {
            var wordListCriteria = (WordListCriteriaContract) searchCriteriaContract;
            var responsiblesAlias = string.Format("r{0}", Guid.NewGuid().ToString("N"));
            var responsibleTypeAlias = string.Format("t{0}", Guid.NewGuid().ToString("N"));
            var whereBuilder = new StringBuilder();
            var parameters = new List<object>();

            foreach (WordCriteriaContract wordCriteria in wordListCriteria.Disjunctions)
            {
                if (whereBuilder.Length > 0)
                    whereBuilder.Append(" or");

                whereBuilder.AppendFormat(" {0}.Text like ?", responsiblesAlias);
                parameters.Add(CriteriaConditionBuilder.Create(wordCriteria));
            }

            return new SearchCriteriaQuery
            {
                Join = string.Format("inner join bv.Responsibles {0} inner join {0}.ResponsibleType {1}", responsiblesAlias, responsibleTypeAlias),
                Where = string.Format("{0}.Text like 'Editor' and ({1})", responsibleTypeAlias, whereBuilder),
                Parameters = parameters
            };
        }
    }

    public class DatingCriteriaImplementation : ICriteriaImplementationBase
    {
        public CriteriaKey CriteriaKey
        {
            get { return CriteriaKey.Dating; }
        }

        public SearchCriteriaQuery CreateCriteriaQuery(SearchCriteriaContract searchCriteriaContract)
        {
            var datingListCriteriaContract = (DatingListCriteriaContract) searchCriteriaContract;
            var manuscriptAlias = string.Format("m{0}", Guid.NewGuid().ToString("N"));
            var whereBuilder = new StringBuilder();
            var datingParameters = new List<object>();

            foreach (DatingCriteriaContract datingCriteriaContract in datingListCriteriaContract.Disjunctions)
            {
                if (whereBuilder.Length > 0)
                    whereBuilder.Append(" or ");

                var notBeforeUsed = false;
                whereBuilder.Append("(");

                if (datingCriteriaContract.NotBefore != null)
                {
                    notBeforeUsed = true;
                    whereBuilder.AppendFormat("{0}.NotAfter >= ?", manuscriptAlias);
                    datingParameters.Add(datingCriteriaContract.NotBefore.Value);
                }

                if (datingCriteriaContract.NotAfter != null)
                {
                    if (notBeforeUsed)
                        whereBuilder.Append(" and ");
                    whereBuilder.AppendFormat("{0}.NotBefore <= ?", manuscriptAlias);
                    datingParameters.Add(datingCriteriaContract.NotAfter.Value);
                }
                whereBuilder.Append(")");
            }

            return new SearchCriteriaQuery
            {
                Join = string.Format("inner join bv.ManuscriptDescriptions {0}", manuscriptAlias),
                Where = whereBuilder.ToString(),
                Parameters = datingParameters
            };
        }
    }
}