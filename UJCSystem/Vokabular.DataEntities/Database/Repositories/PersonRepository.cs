using System.Collections.Generic;
using System.Linq;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.Entities.SelectResults;
using Vokabular.Shared.DataEntities.Daos;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.DataEntities.Database.Repositories
{
    public class PersonRepository : NHibernateDao
    {
        public PersonRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
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

        public ListWithTotalCountResult<ResponsiblePerson> GetResponsiblePersonList(int start, int count)
        {
            var query = GetSession().QueryOver<ResponsiblePerson>()
                .OrderBy(x => x.LastName).Asc
                .ThenBy(x => x.FirstName).Asc
                .Skip(start)
                .Take(count);

            var list = query.Future();
            var totalCount = query.ToRowCountQuery()
                .FutureValue<int>();

            return new ListWithTotalCountResult<ResponsiblePerson>
            {
                List = list.ToList(),
                Count = totalCount.Value
            };
        }

        public ListWithTotalCountResult<OriginalAuthor> GetOriginalAuthorList(int start, int count)
        {
            var query = GetSession().QueryOver<OriginalAuthor>()
                .OrderBy(x => x.LastName).Asc
                .ThenBy(x => x.FirstName).Asc
                .Skip(start)
                .Take(count);

            var list = query.Future();
            var totalCount = query.ToRowCountQuery()
                .FutureValue<int>();

            return new ListWithTotalCountResult<OriginalAuthor>
            {
                List = list.ToList(),
                Count = totalCount.Value
            };
        }

        public virtual OriginalAuthor GetAuthorByName(string firstName, string lastName)
        {
            return GetSession().QueryOver<OriginalAuthor>()
                .Where(x => x.FirstName == firstName && x.LastName == lastName)
                .SingleOrDefault();
        }

        public virtual ResponsiblePerson GetResponsiblePersonByName(string firstName, string lastName)
        {
            return GetSession().QueryOver<ResponsiblePerson>()
                .Where(x => x.FirstName == firstName && x.LastName == lastName)
                .SingleOrDefault();
        }

        public virtual ResponsibleType GetResponsibleTypeByName(string text)
        {
            return GetSession().QueryOver<ResponsibleType>()
                .Where(x => x.Text == text)
                .SingleOrDefault();
        }
    }
}