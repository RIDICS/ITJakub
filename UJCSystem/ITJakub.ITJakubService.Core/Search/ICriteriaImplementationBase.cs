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
            get { return CriteriaKey.Author; }
        }

        public void ProcessCriteria(SearchCriteriaContract searchCriteriaContract, DetachedCriteria databaseCriteria)
        {
            var disjunction = Restrictions.Disjunction();
            foreach (var value in ((StringListCriteriaContract) searchCriteriaContract).Values)
            {
                disjunction.Add(Restrictions.Like("authors.Name", value));
            }

            databaseCriteria.CreateAlias("Authors", "authors")
                .Add(disjunction);
            
        }
    }

    public class TitleCriteriaImplementation : ICriteriaImplementationBase
    {
        public CriteriaKey CriteriaKey
        {
            get { return CriteriaKey.Title; }
        }

        public void ProcessCriteria(SearchCriteriaContract searchCriteriaContract, DetachedCriteria databaseCriteria)
        {
            var disjunction = Restrictions.Disjunction();
            foreach (var value in ((StringListCriteriaContract) searchCriteriaContract).Values)
            {
                disjunction.Add(Restrictions.Like("Title", value));
            }
            databaseCriteria.Add(disjunction);
        }
    }

    public class EditorCriteriaImplementation : ICriteriaImplementationBase
    {
        public CriteriaKey CriteriaKey
        {
            get { return CriteriaKey.Editor; }
        }

        public void ProcessCriteria(SearchCriteriaContract searchCriteriaContract, DetachedCriteria databaseCriteria)
        {
            var disjunction = Restrictions.Disjunction();
            foreach (var value in ((StringListCriteriaContract) searchCriteriaContract).Values)
            {
                disjunction.Add(Restrictions.Like("responsibles.Text", value));
            }

            databaseCriteria.CreateAlias("Responsibles", "responsibles")
                .CreateAlias("responsibles.ResponsibleType", "responsibleType")
                .Add(Restrictions.Eq("responsibleType.Text", "Editor"))
                .Add(disjunction);
        }
    }
}