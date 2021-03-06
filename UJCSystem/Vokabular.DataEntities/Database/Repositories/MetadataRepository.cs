﻿using System.Collections.Generic;
using System.Linq;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.Entities.SelectResults;
using Vokabular.DataEntities.Database.Utils;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.DataEntities.Database.Repositories
{
    public class MetadataRepository : MainDbRepositoryBase
    {
        public MetadataRepository(UnitOfWorkProvider unitOfWorkProvider) : base(unitOfWorkProvider)
        {
        }

        public virtual MetadataResource GetLatestMetadataResource(long projectId)
        {
            Resource resourceAlias = null;
            Project projectAlias = null;

            var query = GetSession().QueryOver<MetadataResource>()
                .JoinAlias(x => x.Resource, () => resourceAlias)
                .JoinAlias(() => resourceAlias.Project, () => projectAlias)
                .Where(x => resourceAlias.Project.Id == projectId && resourceAlias.ResourceType == ResourceTypeEnum.ProjectMetadata &&
                            resourceAlias.LatestVersion.Id == x.Id && !resourceAlias.IsRemoved && projectAlias.IsRemoved == false)
                .Fetch(SelectMode.Fetch, x => x.Resource);

            return query.SingleOrDefault();
        }
        
        public virtual MetadataResource GetLatestMetadataResourceByExternalId(string projectExternalId, ProjectTypeEnum projectType)
        {
            Resource resourceAlias = null;
            Project projectAlias = null;

            var query = GetSession().QueryOver<MetadataResource>()
                .JoinAlias(x => x.Resource, () => resourceAlias)
                .JoinAlias(() => resourceAlias.Project, () => projectAlias)
                .Where(x => projectAlias.ExternalId == projectExternalId && projectAlias.ProjectType == projectType &&
                            resourceAlias.ResourceType == ResourceTypeEnum.ProjectMetadata && resourceAlias.LatestVersion.Id == x.Id &&
                            !resourceAlias.IsRemoved && projectAlias.IsRemoved == false)
                .Fetch(SelectMode.Fetch, x => x.Resource);

            return query.SingleOrDefault();
        }

        public virtual Project GetAdditionalProjectMetadata(long projectId, bool includeAuthors, bool includeResponsibles, bool includeKind, bool includeGenre, bool includeOriginal, bool includeKeyword, bool includeCategory)
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
                    .Fetch(SelectMode.Fetch, x => x.Authors)
                    .FutureValue();
            }
            if (includeResponsibles)
            {
                ProjectResponsiblePerson projectResponsiblePersonAlias = null;

                session.QueryOver<Project>()
                    .JoinAlias(x => x.ResponsiblePersons, () => projectResponsiblePersonAlias, JoinType.LeftOuterJoin)
                    .Where(x => x.Id == projectId)
                    .Fetch(SelectMode.Fetch, x => x.ResponsiblePersons)
                    .Fetch(SelectMode.Fetch, x => x.ResponsiblePersons[0].ResponsiblePerson)
                    .Fetch(SelectMode.Fetch, x => x.ResponsiblePersons[0].ResponsibleType)
                    .OrderBy(() => projectResponsiblePersonAlias.Sequence).Asc
                    .FutureValue();
            }
            if (includeKind)
            {
                session.QueryOver<Project>()
                    .Where(x => x.Id == projectId)
                    .Fetch(SelectMode.Fetch, x => x.LiteraryKinds)
                    .FutureValue();
            }
            if (includeGenre)
            {
                session.QueryOver<Project>()
                    .Where(x => x.Id == projectId)
                    .Fetch(SelectMode.Fetch, x => x.LiteraryGenres)
                    .FutureValue();
            }
            if (includeOriginal)
            {
                session.QueryOver<Project>()
                    .Where(x => x.Id == projectId)
                    .Fetch(SelectMode.Fetch, x => x.LiteraryOriginals)
                    .FutureValue();
            }
            if (includeKeyword)
            {
                session.QueryOver<Project>()
                    .Where(x => x.Id == projectId)
                    .Fetch(SelectMode.Fetch, x => x.Keywords)
                    .FutureValue();
            }
            if (includeCategory)
            {
                session.QueryOver<Project>()
                    .Where(x => x.Id == projectId)
                    .Fetch(SelectMode.Fetch, x => x.Categories)
                    .FutureValue();
            }
            return session.QueryOver<Project>()
                .Where(x => x.Id == projectId && x.IsRemoved == false)
                .FutureValue().Value;
        }

        public virtual IList<MetadataResource> GetMetadataByBookTypeWithCategories(BookTypeEnum bookTypeEnum, int userId, ProjectTypeEnum projectType,
            int start, int count)
        {
            Resource resourceAlias = null;
            Snapshot snapshotAlias = null;
            BookType bookTypeAlias = null;
            Permission permissionAlias = null;
            UserGroup userGroupAlias = null;
            User userAlias = null;

            var resultList = GetSession().QueryOver<MetadataResource>()
                .JoinAlias(x => x.Resource, () => resourceAlias)
                .Where(x => x.Id == resourceAlias.LatestVersion.Id && !resourceAlias.IsRemoved)
                .WithSubquery.WhereProperty(() => resourceAlias.Project.Id).In(QueryOver.Of<Project>()
                    .Where(x => x.ProjectType == projectType && x.IsRemoved == false)
                    .JoinAlias(x => x.LatestPublishedSnapshot, () => snapshotAlias)
                    .JoinAlias(() => snapshotAlias.BookTypes, () => bookTypeAlias)
                    .Where(() => bookTypeAlias.Type == bookTypeEnum)
                    .JoinAlias(x => x.Permissions, () => permissionAlias)
                    .JoinAlias(() => permissionAlias.UserGroup, () => userGroupAlias)
                    .JoinAlias(() => userGroupAlias.Users, () => userAlias)
                    .Where(() => userAlias.Id == userId)
                    .And(BitwiseExpression.On(() => permissionAlias.Flags).HasBit(PermissionFlag.ShowPublished))
                    .Select(x => x.Id))
                .OrderBy(x => x.Title).Asc
                .Skip(start)
                .Take(count)
                .List();

            GetSession().QueryOver<Project>()
                .WhereRestrictionOn(x => x.Id).IsInG(resultList.Select(x => x.Resource.Project.Id))
                .Fetch(SelectMode.Fetch, x => x.Categories)
                .List();

            return resultList;
        }

        public virtual IList<MetadataResource> GetMetadataWithFetchForBiblModule(IEnumerable<long> metadataVersionIdList)
        {
            var session = GetSession();

            var result = session.QueryOver<MetadataResource>()
                .WhereRestrictionOn(x => x.Id).IsInG(metadataVersionIdList)
                .Fetch(SelectMode.Fetch, x => x.Resource)
                .Fetch(SelectMode.Fetch, x => x.Resource.Project)
                //.Fetch(SelectMode.Fetch, x => x.Resource.Project.Authors) // Authors are used from Metadata
                .Fetch(SelectMode.Fetch, x => x.Resource.Project.LatestPublishedSnapshot)
                .Fetch(SelectMode.Fetch, x => x.Resource.Project.LatestPublishedSnapshot.DefaultBookType)
                .List();
            return result;
        }

        public virtual IList<MetadataResource> GetMetadataWithFetchForBiblModuleByProject(IEnumerable<long> projectIdList)
        {
            Resource resourceAlias = null;
            Project projectAlias = null;

            var session = GetSession();

            var result = session.QueryOver<MetadataResource>()
                .JoinAlias(x => x.Resource, () => resourceAlias)
                .JoinAlias(() => resourceAlias.Project, () => projectAlias)
                .WhereRestrictionOn(() => resourceAlias.Project.Id).IsInG(projectIdList)
                .And(x => x.Id == resourceAlias.LatestVersion.Id && !resourceAlias.IsRemoved && projectAlias.IsRemoved == false)
                .Fetch(SelectMode.Fetch, x => x.Resource)
                .Fetch(SelectMode.Fetch, x => x.Resource.Project)
                .Fetch(SelectMode.Fetch, x => x.Resource.Project.LatestPublishedSnapshot)
                .Fetch(SelectMode.Fetch, x => x.Resource.Project.LatestPublishedSnapshot.DefaultBookType)
                .List();
            return result;
        }

        public virtual IList<MetadataResource> GetMetadataWithFetchForBiblModuleByProjectExternalIds(IEnumerable<string> projectExternalIdList, ProjectTypeEnum projectType)
        {
            Resource resourceAlias = null;
            Project projectAlias = null;

            var session = GetSession();

            var result = session.QueryOver<MetadataResource>()
                .JoinAlias(x => x.Resource, () => resourceAlias)
                .JoinAlias(() => resourceAlias.Project, () => projectAlias)
                .WhereRestrictionOn(() => projectAlias.ExternalId).IsInG(projectExternalIdList)
                .And(() => projectAlias.ProjectType == projectType)
                .And(x => x.Id == resourceAlias.LatestVersion.Id && !resourceAlias.IsRemoved && projectAlias.IsRemoved == false)
                .Fetch(SelectMode.Fetch, x => x.Resource)
                .Fetch(SelectMode.Fetch, x => x.Resource.Project)
                .Fetch(SelectMode.Fetch, x => x.Resource.Project.LatestPublishedSnapshot)
                .Fetch(SelectMode.Fetch, x => x.Resource.Project.LatestPublishedSnapshot.DefaultBookType)
                .List();
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
                .Where(x => x.Id == resourceAlias.LatestVersion.Id && !resourceAlias.IsRemoved && resourceAlias.Project.Id == projectId && projectAlias.IsRemoved == false)
                .FutureValue();

            session.QueryOver<Project>()
                .Where(x => x.Id == projectId)
                .Fetch(SelectMode.Fetch, x => x.Keywords)
                .Future();

            session.QueryOver<Project>()
                .Where(x => x.Id == projectId)
                .Fetch(SelectMode.Fetch, x => x.LiteraryGenres)
                .Future();

            session.QueryOver<Project>()
                .Where(x => x.Id == projectId)
                .Fetch(SelectMode.Fetch, x => x.LiteraryKinds)
                .Future();

            session.QueryOver<Project>()
                .Where(x => x.Id == projectId)
                .Fetch(SelectMode.Fetch, x => x.LiteraryOriginals)
                .Future();

            session.QueryOver<Project>()
                .Where(x => x.Id == projectId)
                .JoinAlias(x => x.Authors, () => projectOriginalAuthorAlias, JoinType.LeftOuterJoin)
                .JoinAlias(() => projectOriginalAuthorAlias.OriginalAuthor, () => originalAuthorAlias, JoinType.LeftOuterJoin)
                .Fetch(SelectMode.Fetch, x => x.Authors)
                .Fetch(SelectMode.Fetch, x => x.Authors[0].OriginalAuthor)
                .OrderBy(() => projectOriginalAuthorAlias.Sequence).Asc
                .Future();

            session.QueryOver<Project>()
                .Where(x => x.Id == projectId)
                .JoinAlias(x => x.ResponsiblePersons, () => projectResponsiblePersonAlias, JoinType.LeftOuterJoin)
                .JoinAlias(() => projectResponsiblePersonAlias.ResponsiblePerson, () => responsiblePersonAlias, JoinType.LeftOuterJoin)
                .JoinAlias(() => projectResponsiblePersonAlias.ResponsibleType, () => responsibleTypeAlias, JoinType.LeftOuterJoin)
                .Fetch(SelectMode.Fetch, x => x.ResponsiblePersons)
                .Fetch(SelectMode.Fetch, x => x.ResponsiblePersons[0].ResponsiblePerson)
                .Fetch(SelectMode.Fetch, x => x.ResponsiblePersons[0].ResponsibleType)
                .OrderBy(() => projectResponsiblePersonAlias.Sequence).Asc
                .Future();

            return result.Value;
        }

        public virtual IList<string> GetPublisherAutocomplete(string query, int count)
        {
            query = EscapeQuery(query);

            Resource resourceAlias = null;
            Project projectAlias = null;

            return GetSession().QueryOver<MetadataResource>()
                .JoinAlias(x => x.Resource, () => resourceAlias)
                .JoinAlias(() => resourceAlias.Project, () => projectAlias)
                .Where(x => x.Id == resourceAlias.LatestVersion.Id && !resourceAlias.IsRemoved && projectAlias.IsRemoved == false)
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
            Project projectAlias = null;

            return GetSession().QueryOver<MetadataResource>()
                .JoinAlias(x => x.Resource, () => resourceAlias)
                .JoinAlias(() => resourceAlias.Project, () => projectAlias)
                .Where(x => x.Id == resourceAlias.LatestVersion.Id && !resourceAlias.IsRemoved && projectAlias.IsRemoved == false)
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
            Project projectAlias = null;

            return GetSession().QueryOver<MetadataResource>()
                .JoinAlias(x => x.Resource, () => resourceAlias)
                .JoinAlias(() => resourceAlias.Project, () => projectAlias)
                .Where(x => x.Id == resourceAlias.LatestVersion.Id && !resourceAlias.IsRemoved && projectAlias.IsRemoved == false)
                .AndRestrictionOn(x => x.ManuscriptRepository).IsLike(query, MatchMode.Start)
                .Select(Projections.Distinct(Projections.Property<MetadataResource>(x => x.ManuscriptRepository)))
                .OrderBy(x => x.ManuscriptRepository).Asc
                .Take(count)
                .List<string>();
        }

        public virtual IList<string> GetTitleAutocomplete(string queryString, BookTypeEnum? bookType, ProjectTypeEnum? projectType,
            IList<int> selectedCategoryIds, IList<long> selectedProjectIds, int count, int userId)
        {
            queryString = EscapeQuery(queryString);

            Resource resourceAlias = null;
            Project projectAlias = null;
            Snapshot snapshotAlias = null;
            Permission permissionAlias = null;
            UserGroup userGroupAlias = null;
            User userAlias = null;
            BookType bookTypeAlias = null;
            Category categoryAlias = null;

            var query = GetSession().QueryOver<MetadataResource>()
                .JoinAlias(x => x.Resource, () => resourceAlias)
                .JoinAlias(() => resourceAlias.Project, () => projectAlias)
                .JoinAlias(() => projectAlias.LatestPublishedSnapshot, () => snapshotAlias)
                .JoinAlias(() => projectAlias.Permissions, () => permissionAlias)
                .JoinAlias(() => permissionAlias.UserGroup, () => userGroupAlias)
                .JoinAlias(() => userGroupAlias.Users, () => userAlias)
                .Where(x => x.Id == resourceAlias.LatestVersion.Id && !resourceAlias.IsRemoved && userAlias.Id == userId && projectAlias.IsRemoved == false)
                .And(BitwiseExpression.On(() => permissionAlias.Flags).HasBit(PermissionFlag.ShowPublished))
                .AndRestrictionOn(x => x.Title).IsLike(queryString, MatchMode.Anywhere)
                .Select(Projections.Distinct(Projections.Property<MetadataResource>(x => x.Title)))
                .OrderBy(x => x.Title).Asc;

            if (bookType != null)
            {
                query.JoinAlias(() => snapshotAlias.BookTypes, () => bookTypeAlias)
                    .Where(() => bookTypeAlias.Type == bookType.Value);
            }

            if (projectType != null)
            {
                query.Where(() => projectAlias.ProjectType == projectType.Value);
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

        public virtual IList<MetadataResource> GetMetadataByProjectIds(IList<long> projectIds, bool fetchAuthors, bool fetchResponsiblePersons, bool fetchBookTypes)
        {
            Resource resourceAlias = null;
            Project projectAlias = null;
            ProjectOriginalAuthor projectOriginalAuthorAlias = null;
            ProjectResponsiblePerson projectResponsiblePersonAlias = null;

            var metadataListFuture = GetSession().QueryOver<MetadataResource>()
                .JoinAlias(x => x.Resource, () => resourceAlias)
                .JoinAlias(() => resourceAlias.Project, () => projectAlias)
                .WhereRestrictionOn(() => projectAlias.Id).IsInG(projectIds)
                .Fetch(SelectMode.Fetch, x => x.Resource)
                .Fetch(SelectMode.Fetch, x => x.Resource.Project)
                .And(x => x.Id == resourceAlias.LatestVersion.Id && !resourceAlias.IsRemoved && projectAlias.IsRemoved == false)
                .Future();

            if (fetchAuthors)
            {
                GetSession().QueryOver<Project>()
                    .WhereRestrictionOn(x => x.Id).IsInG(projectIds)
                    .JoinAlias(x => x.Authors, () => projectOriginalAuthorAlias, JoinType.LeftOuterJoin)
                    .Fetch(SelectMode.Fetch, x => x.Authors)
                    .Fetch(SelectMode.Fetch, x => x.Authors[0].OriginalAuthor)
                    .OrderBy(() => projectOriginalAuthorAlias.Sequence).Asc
                    .Future();
            }

            if (fetchResponsiblePersons)
            {
                GetSession().QueryOver<Project>()
                    .JoinAlias(x => x.ResponsiblePersons, () => projectResponsiblePersonAlias, JoinType.LeftOuterJoin)
                    .WhereRestrictionOn(x => x.Id).IsInG(projectIds)
                    .Fetch(SelectMode.Fetch, x => x.ResponsiblePersons)
                    .Fetch(SelectMode.Fetch, x => x.ResponsiblePersons[0].ResponsiblePerson)
                    .Fetch(SelectMode.Fetch, x => x.ResponsiblePersons[0].ResponsibleType)
                    .OrderBy(() => projectResponsiblePersonAlias.Sequence).Asc
                    .Future();
            }

            if (fetchBookTypes)
            {
                GetSession().QueryOver<Project>()
                    .WhereRestrictionOn(x => x.Id).IsInG(projectIds)
                    .Fetch(SelectMode.Fetch, x => x.LatestPublishedSnapshot)
                    .Fetch(SelectMode.Fetch, x => x.LatestPublishedSnapshot.BookTypes)
                    .Future();
            }

            return metadataListFuture.ToList();
        }

        public virtual IList<MetadataResource> GetMetadataByProjectExternalIds(IEnumerable<string> projectExternalIds, ProjectTypeEnum projectType)
        {
            Resource resourceAlias = null;
            Project projectAlias = null;

            return GetSession().QueryOver<MetadataResource>()
                .JoinAlias(x => x.Resource, () => resourceAlias)
                .JoinAlias(() => resourceAlias.Project, () => projectAlias)
                .WhereRestrictionOn(() => projectAlias.ExternalId).IsInG(projectExternalIds)
                .And(() => projectAlias.ProjectType == projectType && projectAlias.IsRemoved == false)
                .And(x => x.Id == resourceAlias.LatestVersion.Id && !resourceAlias.IsRemoved)
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
                .Where(x => x.Id == resourceAlias.LatestVersion.Id && !resourceAlias.IsRemoved && projectOriginalAuthorAlias.OriginalAuthor.Id == authorId && projectAlias.IsRemoved == false)
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
                .Where(x => x.Id == resourceAlias.LatestVersion.Id && !resourceAlias.IsRemoved && projectResponsiblePersonAlias.ResponsiblePerson.Id == responsiblePersonId && projectAlias.IsRemoved == false)
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
            ProjectResponsiblePerson projectResponsiblePersonAlias = null;

            GetSession().QueryOver<Project>()
                .WhereRestrictionOn(x => x.Id).IsInG(projectIds)
                .JoinAlias(x => x.Authors, () => projectOriginalAuthorAlias, JoinType.LeftOuterJoin)
                .Fetch(SelectMode.Fetch, x => x.Authors)
                .Fetch(SelectMode.Fetch, x => x.Authors[0].OriginalAuthor)
                .OrderBy(x => x.Id).Asc
                .ThenBy(() => projectOriginalAuthorAlias.Sequence).Asc
                .Future();

            return GetSession().QueryOver<Project>()
                .WhereRestrictionOn(x => x.Id).IsInG(projectIds)
                .JoinAlias(x => x.ResponsiblePersons, () => projectResponsiblePersonAlias, JoinType.LeftOuterJoin)
                .Fetch(SelectMode.Fetch, x => x.ResponsiblePersons)
                .Fetch(SelectMode.Fetch, x => x.ResponsiblePersons[0].ResponsiblePerson)
                .Fetch(SelectMode.Fetch, x => x.ResponsiblePersons[0].ResponsibleType)
                .OrderBy(x => x.Id).Asc
                .ThenBy(() => projectResponsiblePersonAlias.Sequence).Asc
                .Future().ToList();
        }
    }
}