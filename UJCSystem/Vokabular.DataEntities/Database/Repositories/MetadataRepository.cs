using System.Collections.Generic;
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

        public virtual IList<LiteraryKind> GetLiteraryKindList()
        {
            return GetSession().QueryOver<LiteraryKind>()
                .OrderBy(x => x.Name).Asc
                .List();
        }

        public virtual IList<LiteraryGenre> GetLiteraryGenreList()
        {
            return GetSession().QueryOver<LiteraryGenre>()
                .OrderBy(x => x.Name).Asc
                .List();
        }

        public virtual IList<LiteraryOriginal> GetLiteraryOriginalList()
        {
            return GetSession().QueryOver<LiteraryOriginal>()
                .OrderBy(x => x.Name).Asc
                .List();
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

        public virtual Project GetAdditionalProjectMetadata(long projectId, bool includeAuthors, bool includeResponsibles, bool includeKind, bool includeGenre, bool includeOriginal)
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

            return session.QueryOver<Project>()
                .Where(x => x.Id == projectId)
                .FutureValue().Value;
        }

        public virtual Project GetProjectWithKeywords(long projectId)
        {
            return GetSession().QueryOver<Project>()
                .Where(x => x.Id == projectId)
                .Fetch(x => x.Keywords).Eager
                .SingleOrDefault();
        }

        public virtual IList<ProjectOriginalAuthor> GetProjectOriginalAuthorList(long projectId, bool includeAuthors = false)
        {
            var query = GetSession().QueryOver<ProjectOriginalAuthor>()
                .Where(x => x.Project.Id == projectId);

            if (includeAuthors)
            {
                query.Fetch(x => x.OriginalAuthor);
            }

            return query.List();
        }

        public virtual OriginalAuthor GetAuthorByName(string firstName, string lastName)
        {
            return GetSession().QueryOver<OriginalAuthor>()
                .Where(x => x.FirstName == firstName && x.LastName == lastName)
                .SingleOrDefault();
        }
        
        public virtual Keyword GetKeywordByName(string name)
        {
            return GetSession().QueryOver<Keyword>()
                .Where(x => x.Text == name)
                .SingleOrDefault();
        }

        public virtual IList<ProjectResponsiblePerson> GetProjectResponsibleList(long projectId)
        {
            return GetSession().QueryOver<ProjectResponsiblePerson>()
                .Where(x => x.Project.Id == projectId)
                .Fetch(x => x.ResponsiblePerson).Eager
                .Fetch(x => x.ResponsibleType).Eager
                .List();
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
        
        public virtual IList<HeadwordResource> GetHeadwordWithFetch(IEnumerable<long> headwordIds)
        {
            var result = GetSession().QueryOver<HeadwordResource>()
                .WhereRestrictionOn(x => x.Id).IsInG(headwordIds)
                .Fetch(x => x.Resource).Eager
                .Fetch(x => x.HeadwordItems).Eager
                .List();
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

        public IList<PageResource> GetPagesWithTerms(TermCriteriaPageConditionCreator creator)
        {
            var query = GetSession().CreateQuery(creator.GetQueryString())
                .SetParameters(creator);
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
    }
}