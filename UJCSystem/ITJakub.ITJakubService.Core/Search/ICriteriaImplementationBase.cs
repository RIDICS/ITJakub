using ITJakub.ITJakubService.DataContracts;
using NHibernate.Criterion;

namespace ITJakub.ITJakubService.Core.Search
{
    public interface ICriteriaImplementationBase
    {
        CriteriaKey CriteriaKey { get; }
        void ProcessCriteria(SearchCriteriaContract searchCriteriaContract, DetachedCriteria databaseCriteria);
    }

    public class AuthorCriteriaImplementation : ICriteriaImplementationBase
    {
        public CriteriaKey CriteriaKey
        {
            get
            {
                return CriteriaKey.Author;
            }
        }

        public void ProcessCriteria(SearchCriteriaContract searchCriteriaContract, DetachedCriteria databaseCriteria)
        {
            databaseCriteria.CreateAlias("Authors", "authors")
                .Add(Restrictions.Like("authors.Name", ((StringCriteriaContract) searchCriteriaContract).Value));
        }
    }

    public class TitleCriteriaImplementation : ICriteriaImplementationBase
    {
        public CriteriaKey CriteriaKey
        {
            get
            {
                return CriteriaKey.Title;
            }
        }

        public void ProcessCriteria(SearchCriteriaContract searchCriteriaContract, DetachedCriteria databaseCriteria)
        {
            databaseCriteria.Add(Restrictions.Like("Title", ((StringCriteriaContract) searchCriteriaContract).Value));
        }
    }
}