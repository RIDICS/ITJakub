using System;
using System.Text;
using ITJakub.DataEntities.Database;
using ITJakub.ITJakubService.DataContracts;

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
            var authorAlias = string.Format("a{0}", Guid.NewGuid().ToString("N"));
            var whereBuilder = new StringBuilder();

            for (int i = 0; i < ((StringListCriteriaContract) searchCriteriaContract).Values.Count; i++)
            {
                if (whereBuilder.Length > 0)
                    whereBuilder.Append(" or");

                whereBuilder.AppendFormat(" {0}.Name like ?", authorAlias);
            }

            return new SearchCriteriaQuery
            {
                Join = string.Format("inner join bv.Authors {0}", authorAlias),
                Where = whereBuilder.ToString(),
                Parameters = ((StringListCriteriaContract)searchCriteriaContract).Values
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
            var whereBuilder = new StringBuilder();

            for (int i = 0; i < ((StringListCriteriaContract) searchCriteriaContract).Values.Count; i++)
            {
                if (whereBuilder.Length > 0)
                    whereBuilder.Append(" or");

                whereBuilder.Append("bv.Title like ?");
            }

            return new SearchCriteriaQuery
            {
                Join = string.Empty,
                Where = whereBuilder.ToString(),
                Parameters = ((StringListCriteriaContract)searchCriteriaContract).Values
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
            var responsiblesAlias = string.Format("r{0}", Guid.NewGuid().ToString("N"));
            var responsibleTypeAlias = string.Format("t{0}", Guid.NewGuid().ToString("N"));
            var whereBuilder = new StringBuilder();

            for (int i = 0; i < ((StringListCriteriaContract) searchCriteriaContract).Values.Count; i++)
            {
                if (whereBuilder.Length > 0)
                    whereBuilder.Append(" or");
                whereBuilder.AppendFormat(" {0}.Text like ?", responsiblesAlias);
            }

            return new SearchCriteriaQuery
            {
                Join = string.Format("inner join bv.Responsibles {0} inner join {0}.ResponsibleType {1}", responsiblesAlias, responsibleTypeAlias),
                Where = string.Format("{0}.Text like 'Editor' and ({1})", responsibleTypeAlias, whereBuilder),
                Parameters = ((StringListCriteriaContract)searchCriteriaContract).Values
            };
        }
    }
}