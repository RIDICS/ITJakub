using System.Collections.Generic;
using NHibernate;
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
            OriginalAuthor authorAlias = null;

            query = query.Replace(" ", $"{WildcardAny} ");
            
            return GetSession().QueryOver(() => authorAlias)
                .Where(Restrictions.Or(
                    Restrictions.Like(Projections.SqlFunction("concat", NHibernateUtil.String, Projections.Property(() => authorAlias.LastName),
                        Projections.Constant(" "), Projections.Property(() => authorAlias.FirstName)), query, MatchMode.Start),
                    Restrictions.Like(Projections.Property(() => authorAlias.FirstName), query, MatchMode.Start)
                ))
                .OrderBy(x => x.LastName).Asc
                .ThenBy(x => x.FirstName).Asc
                .Take(count)
                .List();
        }

        public IList<ResponsiblePerson> GetResponsiblePersonAutocomplete(string query, int count)
        {
            ResponsiblePerson responsibleAlias = null;

            query = query.Replace(" ", $"{WildcardAny} ");

            return GetSession().QueryOver(() => responsibleAlias)
                .Where(Restrictions.Or(
                    Restrictions.Like(Projections.SqlFunction("concat", NHibernateUtil.String, Projections.Property(() => responsibleAlias.LastName),
                        Projections.Constant(" "), Projections.Property(() => responsibleAlias.FirstName)), query, MatchMode.Start),
                    Restrictions.Like(Projections.Property(() => responsibleAlias.FirstName), query, MatchMode.Start)
                ))
                .OrderBy(x => x.LastName).Asc
                .ThenBy(x => x.FirstName).Asc
                .Take(count)
                .List();
        }
    }
}