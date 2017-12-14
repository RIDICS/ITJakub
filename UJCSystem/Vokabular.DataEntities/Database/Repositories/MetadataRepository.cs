using System.Collections.Generic;
using System.Linq;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using NHibernate.Transform;
using Vokabular.DataEntities.Database.ConditionCriteria;
using Vokabular.DataEntities.Database.Daos;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.Entities.SelectResults;
using Vokabular.DataEntities.Database.Search;
using Vokabular.DataEntities.Database.UnitOfWork;

namespace Vokabular.DataEntities.Database.Repositories
{
    public class MetadataRepository : NHibernateDao
    {
        public MetadataRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public virtual MetadataResource GetLatestMetadataResource(long projectId)
        {
            Resource resourceAlias = null;

            var query = GetSession().QueryOver<MetadataResource>()
                .JoinAlias(x => x.Resource, () => resourceAlias)
                .Where(x => resourceAlias.Project.Id == projectId && resourceAlias.ResourceType == ResourceTypeEnum.ProjectMetadata && resourceAlias.LatestVersion.Id == x.Id)
                .Fetch(x => x.Resource).Eager;

            return query.SingleOrDefault();
        }

        public virtual Project GetAdditionalProjectMetadata(long projectId, bool includeAuthors, bool includeResponsibles, bool includeKind, bool includeGenre, bool includeOriginal, bool includeKeyword)
        {
            var session = GetSession();

            if (includeAuthors)
            {
                ProjectOriginalAuthor projectAuthorAlias = null;
                OriginalAuthor authorAlias = null;

                session.QueryOver<Project>()
                    .JoinAlias(x => x.Authors, () => projectAuthorAlias, JoinType.LeftOuterJoin)
                    .JoinAlias(() => projectAuthorAlias.OriginalAuthor, () => authorAlias, JoinType.LeftOuterJoin)
                    .Where(x => x.Id == projectId)
                    .OrderBy(() => projectAuthorAlias.Sequence).Asc
                    .Fetch(x => x.Authors).Eager
                    .FutureValue();
            }
            if (includeResponsibles)
            {
                session.QueryOver<Project>()
                    .Where(x => x.Id == projectId)
                    .Fetch(x => x.ResponsiblePersons).Eager
                    .Fetch(x => x.ResponsiblePersons[0].ResponsiblePerson).Eager
                    .Fetch(x => x.ResponsiblePersons[0].ResponsibleType).Eager
                    .FutureValue();
            }
            if (includeKind)
            {
                session.QueryOver<Project>()
                    .Where(x => x.Id == projectId)
                    .Fetch(x => x.LiteraryKinds).Eager
                    .FutureValue();
            }
            if (includeGenre)
            {
                session.QueryOver<Project>()
                    .Where(x => x.Id == projectId)
                    .Fetch(x => x.LiteraryGenres).Eager
                    .FutureValue();
            }
            if (includeOriginal)
            {
                session.QueryOver<Project>()
                    .Where(x => x.Id == projectId)
                    .Fetch(x => x.LiteraryOriginals).Eager
                    .FutureValue();
            }
            if (includeKeyword)
            {
                session.QueryOver<Project>()
                    .Where(x => x.Id == projectId)
                    .Fetch(x => x.Keywords).Eager
                    .FutureValue();
            }
            return session.QueryOver<Project>()
                .Where(x => x.Id == projectId)
                .FutureValue().Value;
        }
        
        public virtual IList<MetadataResource> GetMetadataByBookType(BookTypeEnum bookTypeEnum)
        {
            Resource resourceAlias = null;
            Project projectAlias = null;
            Snapshot snapshotAlias = null;
            BookType bookTypeAlias = null;

            return GetSession().QueryOver<MetadataResource>()
                .JoinAlias(x => x.Resource, () => resourceAlias)
                .JoinAlias(() => resourceAlias.Project, () => projectAlias)
                .JoinAlias(() => projectAlias.LatestPublishedSnapshot, () => snapshotAlias)
                .JoinAlias(() => snapshotAlias.BookTypes, () => bookTypeAlias)
                .Where(x => x.Id == resourceAlias.LatestVersion.Id && bookTypeAlias.Type == bookTypeEnum)
                .OrderBy(x => x.Title).Asc
                .Fetch(x => x.Resource.Project.Categories).Eager
                .List();
        }

        public virtual IList<MetadataResource> GetMetadataWithFetchForBiblModule(IEnumerable<long> metadataIdList)
        {
            var session = GetSession();

            var result = session.QueryOver<MetadataResource>()
                .WhereRestrictionOn(x => x.Id).IsInG(metadataIdList)
                .Fetch(x => x.Resource).Eager
                .Fetch(x => x.Resource.Project).Eager
                //.Fetch(x => x.Resource.Project.Authors).Eager // Authors are used from Metadata
                .Fetch(x => x.Resource.Project.LatestPublishedSnapshot).Eager
                .Fetch(x => x.Resource.Project.LatestPublishedSnapshot.DefaultBookType).Eager
                .List();
            return result;
        }

        public virtual IList<MetadataResource> GetMetadataWithFetchForBiblModuleByProject(IEnumerable<long> projectIdList)
        {
            Resource resourceAlias = null;

            var session = GetSession();

            var result = session.QueryOver<MetadataResource>()
                .JoinAlias(x => x.Resource, () => resourceAlias)
                .WhereRestrictionOn(() => resourceAlias.Project.Id).IsInG(projectIdList)
                .And(x => x.Id == resourceAlias.LatestVersion.Id)
                .Fetch(x => x.Resource).Eager
                .Fetch(x => x.Resource.Project).Eager
                .Fetch(x => x.Resource.Project.LatestPublishedSnapshot).Eager
                .Fetch(x => x.Resource.Project.LatestPublishedSnapshot.DefaultBookType).Eager
                .List();
            return result;
        }

        public virtual IList<MetadataResource> GetMetadataWithFetchForBiblModuleByProjectExternalIds(IEnumerable<string> projectExternalIdList)
        {
            Resource resourceAlias = null;
            Project projectAlias = null;

            var session = GetSession();

            var result = session.QueryOver<MetadataResource>()
                .JoinAlias(x => x.Resource, () => resourceAlias)
                .JoinAlias(() => resourceAlias.Project, () => projectAlias)
                .WhereRestrictionOn(() => projectAlias.ExternalId).IsInG(projectExternalIdList)
                .And(x => x.Id == resourceAlias.LatestVersion.Id)
                .Fetch(x => x.Resource).Eager
                .Fetch(x => x.Resource.Project).Eager
                .Fetch(x => x.Resource.Project.LatestPublishedSnapshot).Eager
                .Fetch(x => x.Resource.Project.LatestPublishedSnapshot.DefaultBookType).Eager
                .List();
            return result;
        }

        // TODO MOVE THIS AWAY: (THIS IS NOT METADATA!)
        public virtual IList<HeadwordResource> GetHeadwordWithFetch(IEnumerable<long> headwordIds)
        {
            var result = GetSession().QueryOver<HeadwordResource>()
                .WhereRestrictionOn(x => x.Id).IsInG(headwordIds)
                .Fetch(x => x.Resource).Eager
                .Fetch(x => x.HeadwordItems).Eager
                .TransformUsing(Transformers.DistinctRootEntity)
                .List();
            return result;
        }

        public virtual HeadwordResource GetHeadwordWithFetchByExternalId(string projectExternalId, string headwordExternalId)
        {
            Resource resourceAlias = null;
            Project projectAlias = null;

            var result = GetSession().QueryOver<HeadwordResource>()
                .JoinAlias(x => x.Resource, () => resourceAlias)
                .JoinAlias(() => resourceAlias.Project, () => projectAlias)
                .Where(x => x.ExternalId == headwordExternalId && projectAlias.ExternalId == projectExternalId && x.Id == resourceAlias.LatestVersion.Id)
                .Fetch(x => x.Resource).Eager
                .Fetch(x => x.HeadwordItems).Eager
                .TransformUsing(Transformers.DistinctRootEntity)
                .SingleOrDefault();
            return result;
        }

        public virtual IList<PageCountResult> GetPageCount(IEnumerable<long> projectIdList)
        {
            PageResource pageResourceAlias = null;
            Resource resourceAlias = null;
            PageCountResult resultAlias = null;

            var result = GetSession().QueryOver(() => pageResourceAlias)
                .JoinAlias(x => x.Resource, () => resourceAlias)
                .WhereRestrictionOn(() => resourceAlias.Project.Id).IsInG(projectIdList)
                .And(x => x.Id == resourceAlias.LatestVersion.Id)
                .SelectList(list => list
                    .SelectGroup(() => resourceAlias.Project.Id).WithAlias(() => resultAlias.ProjectId)
                    .SelectCount(() => pageResourceAlias.Id).WithAlias(() => resultAlias.PageCount))
                .TransformUsing(Transformers.AliasToBean<PageCountResult>())
                .List<PageCountResult>();

            return result;
        }

        public virtual IList<PageResource> GetPagesWithTerms(TermCriteriaPageConditionCreator creator)
        {
            var query = GetSession().CreateQuery(creator.GetQueryString())
                .SetParameters(creator)
                .SetResultTransformer(Transformers.DistinctRootEntity);
            var result = query.List<PageResource>();
            return result;
        }

        public virtual MetadataResource GetMetadataWithDetail(long projectId)
        {
            Resource resourceAlias = null;
            Project projectAlias = null;
            Snapshot snapshotAlias = null;
            BookType bookTypeAlias = null;
            ProjectOriginalAuthor projectOriginalAuthorAlias = null;
            OriginalAuthor originalAuthorAlias = null;
            ProjectResponsiblePerson projectResponsiblePersonAlias = null;
            ResponsiblePerson responsiblePersonAlias = null;
            ResponsibleType responsibleTypeAlias = null;
            
            var session = GetSession();

            var result = session.QueryOver<MetadataResource>()
                .JoinAlias(x => x.Resource, () => resourceAlias)
                .JoinAlias(() => resourceAlias.Project, () => projectAlias)
                .JoinAlias(() => projectAlias.LatestPublishedSnapshot, () => snapshotAlias, JoinType.LeftOuterJoin)
                .JoinAlias(() => snapshotAlias.DefaultBookType, () => bookTypeAlias, JoinType.LeftOuterJoin)
                .Where(x => x.Id == resourceAlias.LatestVersion.Id && resourceAlias.Project.Id == projectId)
                .FutureValue();

            session.QueryOver<Project>()
                .Where(x => x.Id == projectId)
                .Fetch(x => x.Keywords).Eager
                .Future();

            session.QueryOver<Project>()
                .Where(x => x.Id == projectId)
                .Fetch(x => x.LiteraryGenres).Eager
                .Future();

            session.QueryOver<Project>()
                .Where(x => x.Id == projectId)
                .Fetch(x => x.LiteraryKinds).Eager
                .Future();

            session.QueryOver<Project>()
                .Where(x => x.Id == projectId)
                .Fetch(x => x.LiteraryOriginals).Eager
                .Future();

            session.QueryOver<Project>()
                .Where(x => x.Id == projectId)
                .JoinAlias(x => x.Authors, () => projectOriginalAuthorAlias, JoinType.LeftOuterJoin)
                .JoinAlias(() => projectOriginalAuthorAlias.OriginalAuthor, () => originalAuthorAlias, JoinType.LeftOuterJoin)
                .Fetch(x => x.Authors).Eager
                .Fetch(x => x.Authors[0].OriginalAuthor).Eager
                .OrderBy(() => projectOriginalAuthorAlias.Sequence).Asc
                .Future();

            session.QueryOver<Project>()
                .Where(x => x.Id == projectId)
                .JoinAlias(x => x.ResponsiblePersons, () => projectResponsiblePersonAlias, JoinType.LeftOuterJoin)
                .JoinAlias(() => projectResponsiblePersonAlias.ResponsiblePerson, () => responsiblePersonAlias, JoinType.LeftOuterJoin)
                .JoinAlias(() => projectResponsiblePersonAlias.ResponsibleType, () => responsibleTypeAlias, JoinType.LeftOuterJoin)
                .Fetch(x => x.ResponsiblePersons).Eager
                .Fetch(x => x.ResponsiblePersons[0].ResponsiblePerson).Eager
                .Fetch(x => x.ResponsiblePersons[0].ResponsibleType).Eager
                .Future();

            return result.Value;
        }

        public virtual IList<string> GetPublisherAutocomplete(string query, int count)
        {
            query = EscapeQuery(query);

            Resource resourceAlias = null;

            return GetSession().QueryOver<MetadataResource>()
                .JoinAlias(x => x.Resource, () => resourceAlias)
                .Where(x => x.Id == resourceAlias.LatestVersion.Id)
                .AndRestrictionOn(x => x.PublisherText).IsLike(query, MatchMode.Start)
                .Select(Projections.Distinct(Projections.Property<MetadataResource>(x => x.PublisherText)))
                .OrderBy(x => x.PublisherText).Asc
                .Take(count)
                .List<string>();
        }

        public virtual IList<string> GetCopyrightAutocomplete(string query, int count)
        {
            query = EscapeQuery(query);

            Resource resourceAlias = null;

            return GetSession().QueryOver<MetadataResource>()
                .JoinAlias(x => x.Resource, () => resourceAlias)
                .Where(x => x.Id == resourceAlias.LatestVersion.Id)
                .AndRestrictionOn(x => x.Copyright).IsLike(query, MatchMode.Start)
                .Select(Projections.Distinct(Projections.Property<MetadataResource>(x => x.Copyright)))
                .OrderBy(x => x.Copyright).Asc
                .Take(count)
                .List<string>();
        }


        public virtual IList<string> GetManuscriptRepositoryAutocomplete(string query, int count)
        {
            query = EscapeQuery(query);

            Resource resourceAlias = null;

            return GetSession().QueryOver<MetadataResource>()
                .JoinAlias(x => x.Resource, () => resourceAlias)
                .Where(x => x.Id == resourceAlias.LatestVersion.Id)
                .AndRestrictionOn(x => x.ManuscriptRepository).IsLike(query, MatchMode.Start)
                .Select(Projections.Distinct(Projections.Property<MetadataResource>(x => x.ManuscriptRepository)))
                .OrderBy(x => x.ManuscriptRepository).Asc
                .Take(count)
                .List<string>();
        }

        public virtual IList<string> GetTitleAutocomplete(string queryString, BookTypeEnum? bookType, IList<int> selectedCategoryIds, IList<long> selectedProjectIds, int count)
        {
            queryString = EscapeQuery(queryString);

            Resource resourceAlias = null;
            Project projectAlias = null;
            Snapshot snapshotAlias = null;
            BookType bookTypeAlias = null;
            Category categoryAlias = null;

            var query = GetSession().QueryOver<MetadataResource>()
                .JoinAlias(x => x.Resource, () => resourceAlias)
                .JoinAlias(() => resourceAlias.Project, () => projectAlias)
                .JoinAlias(() => projectAlias.LatestPublishedSnapshot, () => snapshotAlias)
                .Where(x => x.Id == resourceAlias.LatestVersion.Id)
                .AndRestrictionOn(x => x.Title).IsLike(queryString, MatchMode.Anywhere)
                .Select(Projections.Distinct(Projections.Property<MetadataResource>(x => x.Title)))
                .OrderBy(x => x.Title).Asc;

            if (bookType != null)
            {
                query.JoinAlias(() => snapshotAlias.BookTypes, () => bookTypeAlias)
                    .Where(() => bookTypeAlias.Type == bookType.Value);
            }

            if (selectedCategoryIds.Count > 0 || selectedProjectIds.Count > 0)
            {
                query.JoinAlias(() => projectAlias.Categories, () => categoryAlias, JoinType.LeftOuterJoin)
                    .Where(Restrictions.Or(
                        Restrictions.InG(Projections.Property(() => categoryAlias.Id), selectedCategoryIds),
                        Restrictions.InG(Projections.Property(() => projectAlias.Id), selectedProjectIds)
                    ));
            }

            return query
                .Take(count)
                .List<string>();
        }

        public virtual IList<MetadataResource> GetMetadataByProjectIds(IList<long> projectIds, bool fetchAuthors, bool fetchResponsiblePersons)
        {
            Resource resourceAlias = null;
            Project projectAlias = null;
            ProjectOriginalAuthor projectOriginalAuthorAlias = null;

            var metadataListFuture = GetSession().QueryOver<MetadataResource>()
                .JoinAlias(x => x.Resource, () => resourceAlias)
                .JoinAlias(() => resourceAlias.Project, () => projectAlias)
                .WhereRestrictionOn(() => projectAlias.Id).IsInG(projectIds)
                .Fetch(x => x.Resource).Eager
                .Fetch(x => x.Resource.Project).Eager
                .And(x => x.Id == resourceAlias.LatestVersion.Id)
                .Future();

            if (fetchAuthors)
            {
                GetSession().QueryOver<Project>()
                    .WhereRestrictionOn(x => x.Id).IsInG(projectIds)
                    .JoinAlias(x => x.Authors, () => projectOriginalAuthorAlias, JoinType.LeftOuterJoin)
                    .Fetch(x => x.Authors).Eager
                    .Fetch(x => x.Authors[0].OriginalAuthor).Eager
                    .OrderBy(() => projectOriginalAuthorAlias.Sequence).Asc
                    .Future();
            }

            if (fetchResponsiblePersons)
            {
                GetSession().QueryOver<Project>()
                    .WhereRestrictionOn(x => x.Id).IsInG(projectIds)
                    .Fetch(x => x.ResponsiblePersons).Eager
                    .Fetch(x => x.ResponsiblePersons[0].ResponsiblePerson).Eager
                    .Fetch(x => x.ResponsiblePersons[0].ResponsibleType).Eager
                    .Future();
            }

            return metadataListFuture.ToList();
        }

        public virtual IList<MetadataResource> GetMetadataByProjectExternalIds(IEnumerable<string> projectExternalIds)
        {
            Resource resourceAlias = null;
            Project projectAlias = null;

            return GetSession().QueryOver<MetadataResource>()
                .JoinAlias(x => x.Resource, () => resourceAlias)
                .JoinAlias(() => resourceAlias.Project, () => projectAlias)
                .WhereRestrictionOn(() => projectAlias.ExternalId).IsInG(projectExternalIds)
                .And(x => x.Id == resourceAlias.LatestVersion.Id)
                .List();
        }

        public virtual ListWithTotalCountResult<MetadataResource> GetMetadataByAuthor(int authorId, int start, int count)
        {
            Resource resourceAlias = null;
            Project projectAlias = null;
            ProjectOriginalAuthor projectOriginalAuthorAlias = null;
            User userAlias = null;
            
            var query = GetSession().QueryOver<MetadataResource>()
                .JoinAlias(x => x.Resource, () => resourceAlias)
                .JoinAlias(() => resourceAlias.Project, () => projectAlias)
                .JoinAlias(() => projectAlias.Authors, () => projectOriginalAuthorAlias)
                .JoinAlias(() => projectAlias.CreatedByUser, () => userAlias) // fetch user
                .Where(x => x.Id == resourceAlias.LatestVersion.Id && projectOriginalAuthorAlias.OriginalAuthor.Id == authorId)
                .OrderBy(x => x.Title).Asc
                .Take(count)
                .Skip(start);

            var countFuture = query.ToRowCountQuery()
                .FutureValue<int>();

            var metadata = query.Future()
                .ToList();

            var projectIds = metadata.Select(x => x.Resource.Project.Id).ToList();
            FetchAuthorsAndResponsibles(projectIds);

            return new ListWithTotalCountResult<MetadataResource>
            {
                List = metadata,
                Count = countFuture.Value,
            };
        }

        public virtual ListWithTotalCountResult<MetadataResource> GetMetadataByResponsiblePerson(int responsiblePersonId, int start, int count)
        {
            Resource resourceAlias = null;
            Project projectAlias = null;
            ProjectResponsiblePerson projectResponsiblePersonAlias = null;
            User userAlias = null;

            var query = GetSession().QueryOver<MetadataResource>()
                .JoinAlias(x => x.Resource, () => resourceAlias)
                .JoinAlias(() => resourceAlias.Project, () => projectAlias)
                .JoinAlias(() => projectAlias.ResponsiblePersons, () => projectResponsiblePersonAlias)
                .JoinAlias(() => projectAlias.CreatedByUser, () => userAlias) // fetch user
                .Where(x => x.Id == resourceAlias.LatestVersion.Id && projectResponsiblePersonAlias.ResponsiblePerson.Id == responsiblePersonId)
                .OrderBy(x => x.Title).Asc
                .Take(count)
                .Skip(start);

            var countFuture = query.ToRowCountQuery()
                .FutureValue<int>();

            var metadata = query.Future()
                .ToList();

            var projectIds = metadata.Select(x => x.Resource.Project.Id).ToList();
            FetchAuthorsAndResponsibles(projectIds);

            return new ListWithTotalCountResult<MetadataResource>
            {
                List = metadata,
                Count = countFuture.Value,
            };
        }

        public virtual IList<Project> FetchAuthorsAndResponsibles(IList<long> projectIds)
        {
            ProjectOriginalAuthor projectOriginalAuthorAlias = null;

            GetSession().QueryOver<Project>()
                .WhereRestrictionOn(x => x.Id).IsInG(projectIds)
                .JoinAlias(x => x.Authors, () => projectOriginalAuthorAlias, JoinType.LeftOuterJoin)
                .Fetch(x => x.Authors).Eager
                .Fetch(x => x.Authors[0].OriginalAuthor).Eager
                .OrderBy(x => x.Id).Asc
                .ThenBy(() => projectOriginalAuthorAlias.Sequence).Asc
                .Future();

            return GetSession().QueryOver<Project>()
                .WhereRestrictionOn(x => x.Id).IsInG(projectIds)
                .Fetch(x => x.ResponsiblePersons).Eager
                .Fetch(x => x.ResponsiblePersons[0].ResponsiblePerson).Eager
                .Fetch(x => x.ResponsiblePersons[0].ResponsibleType).Eager
                .OrderBy(x => x.Id).Asc
                .Future().ToList();
        }
    }
}