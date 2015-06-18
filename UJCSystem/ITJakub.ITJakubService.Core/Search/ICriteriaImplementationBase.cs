using System;
using System.Collections.Generic;
using ITJakub.ITJakubService.DataContracts;
using NHibernate.Criterion;

namespace ITJakub.ITJakubService.Core.Search
{
    public interface ICriteriaImplementationBase
    {
        CriteriaKey CriteriaKey { get; }
        CriteriaQuery CreateCriteriaQuery(SearchCriteriaContract searchCriteriaContract);
        void ProcessCriteria(SearchCriteriaContract searchCriteriaContract, DetachedCriteria databaseCriteria);
    }

    public class CriteriaQuery
    {
        public CriteriaQuery()
        {
            Parameters = new List<string>();
        }

        public string Join { get; set; }
        
        public string ParameterName { get; set; }

        public List<string> Parameters { get; set; }
    }

    public class AuthorCriteriaImplementation : ICriteriaImplementationBase
    {
        public CriteriaKey CriteriaKey
        {
            get { return CriteriaKey.Author; }
        }

        public CriteriaQuery CreateCriteriaQuery(SearchCriteriaContract searchCriteriaContract)
        {
            var authorAlias = Guid.NewGuid().ToString();
            var criteriaDisjunction = new CriteriaQuery
            {
                ParameterName = string.Format("{0}.Name", authorAlias),
                Join = string.Format("inner join b.Authors {0}", authorAlias)
            };

            foreach (var value in ((StringListCriteriaContract)searchCriteriaContract).Values)
            {
                criteriaDisjunction.Parameters.Add(value);
            }

            return criteriaDisjunction;
        }

        public void ProcessCriteria(SearchCriteriaContract searchCriteriaContract, DetachedCriteria databaseCriteria)
        {
            var authorAlias = Guid.NewGuid().ToString();
            var disjunction = Restrictions.Disjunction();
            foreach (var value in ((StringListCriteriaContract) searchCriteriaContract).Values)
            {
                disjunction.Add(Restrictions.Like(string.Format("{0}.Name", authorAlias), value));
            }
            
            databaseCriteria.CreateAlias("Authors", authorAlias)
                .Add(disjunction);
        }
    }

    public class TitleCriteriaImplementation : ICriteriaImplementationBase
    {
        public CriteriaKey CriteriaKey
        {
            get { return CriteriaKey.Title; }
        }

        public CriteriaQuery CreateCriteriaQuery(SearchCriteriaContract searchCriteriaContract)
        {
            var criteriaDisjunction = new CriteriaQuery
            {
                ParameterName = "b.Title",
                Join = string.Empty
            };
            
            foreach (var value in ((StringListCriteriaContract)searchCriteriaContract).Values)
            {
                criteriaDisjunction.Parameters.Add(value);
            }

            return criteriaDisjunction;
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

        public CriteriaQuery CreateCriteriaQuery(SearchCriteriaContract searchCriteriaContract)
        {
            throw new NotImplementedException();
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