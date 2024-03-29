﻿using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.Entities.SelectResults;
using Vokabular.DataEntities.Database.Utils;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.DataEntities.Database.Repositories
{
    public class ProjectRepository : MainDbRepositoryBase
    {
        public ProjectRepository(UnitOfWorkProvider unitOfWorkProvider) : base(unitOfWorkProvider)
        {
        }

        public virtual ListWithTotalCountResult<Project> GetProjectList(int start, int count, ProjectTypeEnum? projectType,
            string filterByName = null, int? includeUserId = null, int? excludeUserId = null)
        {
            Permission permissionAlias = null;
            UserGroup userGroupAlias = null;
            User userAlias = null;

            var query = GetSession().QueryOver<Project>()
                .Where(x => x.IsRemoved == false)
                .Fetch(SelectMode.Fetch, x => x.CreatedByUser);

            if (projectType != null)
            {
                query.Where(x => x.ProjectType == projectType.Value);
            }

            if (!string.IsNullOrEmpty(filterByName))
            {
                query.WhereRestrictionOn(x => x.Name).IsInsensitiveLike(filterByName, MatchMode.Anywhere);
            }

            var permissionSubquery = QueryOver.Of<Project>()
                .JoinAlias(x => x.Permissions, () => permissionAlias)
                .JoinAlias(() => permissionAlias.UserGroup, () => userGroupAlias)
                .JoinAlias(() => userGroupAlias.Users, () => userAlias)
                .Where(BitwiseExpression.On(() => permissionAlias.Flags).HasBit(PermissionFlag.ReadProject));

            if (includeUserId != null)
            {
                query.WithSubquery.WhereProperty(x => x.Id)
                    .In(permissionSubquery.Where(() => userAlias.Id == includeUserId.Value)
                        .Select(x => x.Id));
            }

            if (excludeUserId != null)
            {
                query.WithSubquery.WhereProperty(x => x.Id)
                    .NotIn(permissionSubquery.Where(() => userAlias.Id == excludeUserId.Value)
                        .Select(x => x.Id));
            }
            
            query.OrderBy(x => x.Name).Asc
                .Skip(start)
                .Take(count);

            var list = query.Future();
            var totalCount = query.ToRowCountQuery().FutureValue<int>();

            return new ListWithTotalCountResult<Project>
            {
                List = list.ToList(),
                Count = totalCount.Value
            };
        }

        public virtual Project GetProject(long projectId)
        {
            return GetSession().QueryOver<Project>()
                .Where(x => x.Id == projectId && x.IsRemoved == false)
                .SingleOrDefault();
        }

        public virtual Project GetProjectWithUser(long projectId)
        {
            return GetSession().QueryOver<Project>()
                .Where(x => x.Id == projectId && x.IsRemoved == false)
                .Fetch(SelectMode.Fetch, x => x.CreatedByUser)
                .SingleOrDefault();
        }

        public virtual IList<FullProjectImportLog> GetAllImportLogByExternalId(string projectExternalId, ProjectTypeEnum projectType)
        {
            Project projectAlias = null;

            return GetSession().QueryOver<FullProjectImportLog>()
                .JoinAlias(x => x.Project, () => projectAlias)
                .Where(x => projectAlias.ExternalId == projectExternalId && projectAlias.ProjectType == projectType && projectAlias.IsRemoved == false)
                .List();
        }

        public virtual Project GetProjectByExternalId(string externalId, ProjectTypeEnum projectType)
        {
            return GetSession().QueryOver<Project>()
                .Where(x => x.ExternalId == externalId && x.ProjectType == projectType && x.IsRemoved == false)
                .SingleOrDefault();
        }

        public virtual BookType GetBookTypeByEnum(BookTypeEnum bookTypeEnum)
        {
            return GetSession().QueryOver<BookType>()
                .Where(x => x.Type == bookTypeEnum)
                .SingleOrDefault();
        }

        public virtual IList<PageCountResult> GetAllPageCount(IEnumerable<long> projectIdList)
        {
            PageResource pageResourceAlias = null;
            Resource resourceAlias = null;
            Project projectAlias = null;
            PageCountResult resultAlias = null;

            var result = GetSession().QueryOver(() => pageResourceAlias)
                .JoinAlias(x => x.Resource, () => resourceAlias)
                .JoinAlias(() => resourceAlias.Project, () => projectAlias)
                .WhereRestrictionOn(() => resourceAlias.Project.Id).IsInG(projectIdList)
                .And(x => x.Id == resourceAlias.LatestVersion.Id && !resourceAlias.IsRemoved && projectAlias.IsRemoved == false)
                .SelectList(list => list
                    .SelectGroup(() => resourceAlias.Project.Id).WithAlias(() => resultAlias.ProjectId)
                    .SelectCount(() => pageResourceAlias.Id).WithAlias(() => resultAlias.PageCount))
                .TransformUsing(Transformers.AliasToBean<PageCountResult>())
                .List<PageCountResult>();

            return result;
        }
        
        public virtual IList<LatestChangedResourceResult> GetAllLatestChangedResource(IEnumerable<long> projectIdList)
        {
            ResourceVersion resourceVersionAlias = null;
            Resource resourceAlias = null;
            Project projectAlias = null;
            ResourceVersion resourceVersionAlias2 = null;
            Resource resourceAlias2 = null;
            LatestChangedResourceResult resultAlias = null;

            var result = GetSession().QueryOver(() => resourceVersionAlias)
                .JoinAlias(() => resourceVersionAlias.Resource, () => resourceAlias)
                .JoinAlias(() => resourceAlias.Project, () => projectAlias)
                .WhereRestrictionOn(() => resourceAlias.Project.Id).IsInG(projectIdList)
                .And(() => projectAlias.IsRemoved == false)
                .And(x => x.Id == resourceAlias.LatestVersion.Id /*&& !resourceAlias.IsRemoved*/)
                .WithSubquery.Where(() => resourceVersionAlias.CreateTime == QueryOver.Of(() => resourceAlias2)
                                              .JoinAlias(() => resourceAlias2.LatestVersion, () => resourceVersionAlias2)
                                              .Where(() => resourceAlias2.Project.Id == resourceAlias.Project.Id)
                                              .Select(Projections.Max(() => resourceVersionAlias2.CreateTime))
                                              .Take(1)
                                              .As<DateTime>())
                .SelectList(list => list
                    .SelectGroup(() => resourceAlias.Project.Id).WithAlias(() => resultAlias.ProjectId)
                    .SelectMax(() => resourceVersionAlias.CreateTime).WithAlias(() => resultAlias.CreateTime)
                    .SelectMin(() => resourceVersionAlias.CreatedByUser.Id).WithAlias(() => resultAlias.CreatedByUserId))
                .TransformUsing(Transformers.AliasToBean<LatestChangedResourceResult>())
                .List<LatestChangedResourceResult>();

            return result;
        }

        public virtual Snapshot GetLatestSnapshot(long projectId)
        {
            Project projectAlias = null;

            return GetSession().QueryOver<Snapshot>()
                .JoinAlias(x => x.Project, () => projectAlias)
                .Where(x => x.Project.Id == projectId && projectAlias.IsRemoved == false)
                .Fetch(SelectMode.Fetch, x => x.Project)
                .OrderBy(x => x.VersionNumber).Desc
                .Take(1)
                .SingleOrDefault();
        }

        public virtual Project GetProjectWithKeywords(long projectId)
        {
            return GetSession().QueryOver<Project>()
                .Where(x => x.Id == projectId && x.IsRemoved == false)
                .Fetch(SelectMode.Fetch, x => x.Keywords)
                .SingleOrDefault();
        }

        public virtual IList<ProjectOriginalAuthor> GetProjectOriginalAuthorList(long projectId, bool includeAuthors = false)
        {
            Project projectAlias = null;

            var query = GetSession().QueryOver<ProjectOriginalAuthor>()
                .JoinAlias(x => x.Project, () => projectAlias)
                .Where(x => x.Project.Id == projectId && projectAlias.IsRemoved == false)
                .OrderBy(x => x.Sequence).Asc;

            if (includeAuthors)
            {
                query.Fetch(SelectMode.Fetch, x => x.OriginalAuthor);
            }

            return query.List();
        }

        public virtual IList<ProjectResponsiblePerson> GetProjectResponsibleList(long projectId)
        {
            Project projectAlias = null;

            return GetSession().QueryOver<ProjectResponsiblePerson>()
                .JoinAlias(x => x.Project, () => projectAlias)
                .Where(x => x.Project.Id == projectId && projectAlias.IsRemoved == false)
                .Fetch(SelectMode.Fetch, x => x.ResponsiblePerson)
                .Fetch(SelectMode.Fetch, x => x.ResponsibleType)
                .OrderBy(x => x.Sequence).Asc
                .List();
        }

        public virtual IList<LiteraryKind> GetProjectLiteraryKinds(long projectId)
        {
            Project projectAlias = null;

            return GetSession().QueryOver<LiteraryKind>()
                .JoinAlias(x => x.Projects, () => projectAlias)
                .Where(() => projectAlias.Id == projectId && projectAlias.IsRemoved == false)
                .List();
        }

        public virtual IList<LiteraryGenre> GetProjectLiteraryGenres(long projectId)
        {
            Project projectAlias = null;

            return GetSession().QueryOver<LiteraryGenre>()
                .JoinAlias(x => x.Projects, () => projectAlias)
                .Where(() => projectAlias.Id == projectId && projectAlias.IsRemoved == false)
                .List();
        }

        public virtual IList<LiteraryOriginal> GetProjectLiteraryOriginals(long projectId)
        {
            Project projectAlias = null;

            return GetSession().QueryOver<LiteraryOriginal>()
                .JoinAlias(x => x.Projects, () => projectAlias)
                .Where(() => projectAlias.Id == projectId && projectAlias.IsRemoved == false)
                .List();
        }

        public virtual IList<Category> GetProjectCategories(long projectId)
        {
            Project projectAlias = null;

            return GetSession().QueryOver<Category>()
                .JoinAlias(x => x.Projects, () => projectAlias)
                .Where(() => projectAlias.Id == projectId && projectAlias.IsRemoved == false)
                .List();
        }

        public virtual IList<Permission> FindPermissionsForProjectsByUserId(IEnumerable<long> projectIds, int userId)
        {
            UserGroup userGroupAlias = null;
            User userAlias = null;
            Project projectAlias = null;

            return GetSession().QueryOver<Permission>()
                .JoinAlias(x => x.UserGroup, () => userGroupAlias)
                .JoinAlias(() => userGroupAlias.Users, () => userAlias)
                .JoinAlias(x => x.Project, () => projectAlias)
                .WhereRestrictionOn(() => projectAlias.Id).IsInG(projectIds)
                .Where(() => userAlias.Id == userId && projectAlias.IsRemoved == false)
                .List();
        }

        public virtual ProjectGroup GetProjectGroupWithProjects(long projectId)
        {
            var subquery = QueryOver.Of<Project>()
                .Where(x => x.Id == projectId)
                .Select(x => x.ProjectGroup.Id);

            return GetSession().QueryOver<ProjectGroup>()
                .WithSubquery.Where(x => x.Id == subquery.As<int>())
                .Fetch(SelectMode.Fetch, x => x.Projects)
                .SingleOrDefault<ProjectGroup>();
        }
    }
}