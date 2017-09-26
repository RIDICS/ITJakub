using System.Collections.Generic;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using Vokabular.DataEntities.Database.Daos;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.Enums;
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

        public IList<OriginalAuthor> GetAuthorAutocomplete(string queryString, BookTypeEnum? bookTypeEnum, int count)
        {
            OriginalAuthor authorAlias = null;

            queryString = EscapeQuery(queryString);
            queryString = queryString.Replace(" ", $"{WildcardAny} ");
            
            var query = GetSession().QueryOver(() => authorAlias)
                .Where(Restrictions.Or(
                    Restrictions.Like(Projections.SqlFunction("concat", NHibernateUtil.String, Projections.Property(() => authorAlias.LastName),
                        Projections.Constant(" "), Projections.Property(() => authorAlias.FirstName)), queryString, MatchMode.Start),
                    Restrictions.Like(Projections.Property(() => authorAlias.FirstName), queryString, MatchMode.Start)
                ))
                .OrderBy(x => x.LastName).Asc
                .ThenBy(x => x.FirstName).Asc;


            if (bookTypeEnum != null)
            {
                ProjectOriginalAuthor projectOriginalAuthorAlias = null;
                Project projectAlias = null;
                Snapshot snapshotAlias = null;
                BookType bookTypeAlias = null;

                query.JoinAlias(x => x.Projects, () => projectOriginalAuthorAlias)
                    .JoinAlias(() => projectOriginalAuthorAlias.Project, () => projectAlias)
                    .JoinAlias(() => projectAlias.LatestPublishedSnapshot, () => snapshotAlias)
                    .JoinAlias(() => snapshotAlias.BookTypes, () => bookTypeAlias)
                    .Where(() => bookTypeAlias.Type == bookTypeEnum.Value)
                    .Select(Projections.Distinct(Projections.ProjectionList()
                        .Add(Projections.Property<OriginalAuthor>(x => x.Id).WithAlias(() => authorAlias.Id))
                        .Add(Projections.Property<OriginalAuthor>(x => x.FirstName).WithAlias(() => authorAlias.FirstName))
                        .Add(Projections.Property<OriginalAuthor>(x => x.LastName).WithAlias(() => authorAlias.LastName))
                        ))
                    .TransformUsing(Transformers.AliasToBean<OriginalAuthor>());
            }

            return query
                .Take(count)
                .List();
        }

        public IList<ResponsiblePerson> GetResponsiblePersonAutocomplete(string queryString, int count)
        {
            ResponsiblePerson responsibleAlias = null;

            queryString = EscapeQuery(queryString);
            queryString = queryString.Replace(" ", $"{WildcardAny} ");

            return GetSession().QueryOver(() => responsibleAlias)
                .Where(Restrictions.Or(
                    Restrictions.Like(Projections.SqlFunction("concat", NHibernateUtil.String, Projections.Property(() => responsibleAlias.LastName),
                        Projections.Constant(" "), Projections.Property(() => responsibleAlias.FirstName)), queryString, MatchMode.Start),
                    Restrictions.Like(Projections.Property(() => responsibleAlias.FirstName), queryString, MatchMode.Start)
                ))
                .OrderBy(x => x.LastName).Asc
                .ThenBy(x => x.FirstName).Asc
                .Take(count)
                .List();
        }
    }
}