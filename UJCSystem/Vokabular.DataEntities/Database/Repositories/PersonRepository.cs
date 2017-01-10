using System.Collections.Generic;
using NHibernate.Criterion;
using Vokabular.DataEntities.Database.Daos;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.UnitOfWork;

namespace Vokabular.DataEntities.Database.Repositories
{
    public class PersonRepository : NHibernateDao
    {
        public PersonRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public IList<ResponsibleType> GetResponsibleTypeList()
        {
            return GetSession().QueryOver<ResponsibleType>()
                .OrderBy(x => x.Text).Asc
                .List();
        }

        public IList<OriginalAuthor> GetAuthorAutocomplete(string query, int count)
        {
            return GetSession().QueryOver<OriginalAuthor>()
                .Where(Restrictions.Or(
                    Restrictions.On<OriginalAuthor>(x => x.FirstName).IsLike(query, MatchMode.Start),
                    Restrictions.On<OriginalAuthor>(x => x.LastName).IsLike(query, MatchMode.Start)
                ))
                .OrderBy(x => x.LastName).Asc
                .ThenBy(x => x.FirstName).Asc
                .Take(count)
                .List();
        }

        public IList<ResponsiblePerson> GetResponsiblePersonAutocomplete(string query, int count)
        {
            return GetSession().QueryOver<ResponsiblePerson>()
                .Where(Restrictions.Or(
                    Restrictions.On<ResponsiblePerson>(x => x.FirstName).IsLike(query, MatchMode.Start),
                    Restrictions.On<ResponsiblePerson>(x => x.LastName).IsLike(query, MatchMode.Start)
                ))
                .OrderBy(x => x.LastName).Asc
                .ThenBy(x => x.FirstName).Asc
                .Take(count)
                .List();
        }
    }
}