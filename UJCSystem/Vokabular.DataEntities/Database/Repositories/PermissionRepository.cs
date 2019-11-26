﻿using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate;
using NHibernate.Criterion;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.Entities.SelectResults;
using Vokabular.DataEntities.Database.Utils;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.DataEntities.Database.Repositories
{
    public class PermissionRepository : MainDbRepositoryBase
    {
        public PermissionRepository(UnitOfWorkProvider unitOfWorkProvider) : base(unitOfWorkProvider)
        {
        }

        public virtual User GetUserWithGroups(int userId)
        {
            var result = GetSession().QueryOver<User>()
                .Where(x => x.Id == userId)
                .Fetch(SelectMode.Fetch, x => x.Groups)
                .SingleOrDefault();
            return result;
        }

        public virtual IList<UserGroup> GetUserGroupsByUser(int userId)
        {
            User userAlias = null;

            var result = GetSession().QueryOver<UserGroup>()
                .JoinAlias(x => x.Users, () => userAlias)
                .Where(() => userAlias.Id == userId)
                .OrderBy(x => x.Name).Asc
                .List();
            return result;
        }

        public virtual RoleUserGroup FindGroupByExternalId(int externalId)
        {
            var group = GetSession().QueryOver<RoleUserGroup>()
                .Where(g => g.ExternalId == externalId)
                .SingleOrDefault();

            return group;
        }

        public virtual RoleUserGroup FindGroupByExternalIdOrCreate(int externalId)
        {
            return FindGroupByExternalIdOrCreate(externalId, null);
        }

        public virtual RoleUserGroup FindGroupByExternalIdOrCreate(int externalId, string roleName)
        {
            var group = FindGroupByExternalId(externalId);
            if (group != null)
            {
                return group;
            }

            var now = DateTime.UtcNow;
            var newGroup = new RoleUserGroup
            {
                ExternalId = externalId,
                CreateTime = now,
                LastChange = now,
                Name = roleName
            };

            CreateGroup(newGroup);

            return newGroup;
        }

        public virtual IList<int> GetGroupIdsByExternalIds(IEnumerable<int> externalIds)
        {
            var result = GetSession().QueryOver<RoleUserGroup>()
                .WhereRestrictionOn(x => x.ExternalId).IsInG(externalIds)
                .Select(x => x.Id)
                .List<int>();
            return result;
        }

        public virtual SingleUserGroup FindSingleUserGroupByName(string name)
        {
            var result = GetSession().QueryOver<SingleUserGroup>()
                .Where(x => x.Name == name)
                .SingleOrDefault();
            return result;
        }

        public virtual int CreateGroup(UserGroup group)
        {
            return (int) GetSession().Save(group);
        }

        public virtual IList<long> GetFilteredBookIdListByUserPermissions(int userId, IEnumerable<long> bookIds, PermissionFlag permission)
        {
            Project projectAlias = null;
            Permission permissionAlias = null;
            UserGroup groupAlias = null;
            User userAlias = null;

            var filteredBookIds = GetSession().QueryOver(() => projectAlias)
                .JoinQueryOver(x => x.Permissions, () => permissionAlias)
                .JoinQueryOver(x => permissionAlias.UserGroup, () => groupAlias)
                .JoinQueryOver(x => groupAlias.Users, () => userAlias)
                .Select(Projections.Distinct(Projections.Property(() => projectAlias.Id)))
                .Where(() => userAlias.Id == userId && projectAlias.IsRemoved == false)
                .AndRestrictionOn(() => projectAlias.Id).IsInG(bookIds)
                .And(BitwiseExpression.On(() => permissionAlias.Flags).HasBit(permission))
                .List<long>();

            return filteredBookIds;
        }

        public virtual IList<long> GetFilteredBookIdListByGroupPermissions(int groupId, IEnumerable<long> bookIds, PermissionFlag permission)
        {
            Project projectAlias = null;
            Permission permissionAlias = null;
            UserGroup groupAlias = null;

            var filteredBookIds = GetSession().QueryOver(() => projectAlias)
                .JoinQueryOver(x => x.Permissions, () => permissionAlias)
                .JoinQueryOver(x => permissionAlias.UserGroup, () => groupAlias)
                .Select(Projections.Distinct(Projections.Property(() => projectAlias.Id)))
                .Where(() => groupAlias.Id == groupId && projectAlias.IsRemoved == false)
                .AndRestrictionOn(() => projectAlias.Id).IsInG(bookIds)
                .And(BitwiseExpression.On(() => permissionAlias.Flags).HasBit(permission))
                .List<long>();

            return filteredBookIds;
        }

        public virtual Resource GetResourceByUserPermissions(int userId, long resourceId, PermissionFlag permission)
        {
            Resource resourceAlias = null;
            Project projectAlias = null;
            Permission permissionAlias = null;
            UserGroup groupAlias = null;
            User userAlias = null;

            var filteredResource = GetSession().QueryOver(() => resourceAlias)
                .JoinQueryOver(x => x.Project, () => projectAlias)
                .JoinQueryOver(x => x.Permissions, () => permissionAlias)
                .JoinQueryOver(x => x.UserGroup, () => groupAlias)
                .JoinQueryOver(x => x.Users, () => userAlias)
                .Where(() => userAlias.Id == userId && resourceAlias.Id == resourceId && projectAlias.IsRemoved == false)
                .And(BitwiseExpression.On(() => permissionAlias.Flags).HasBit(permission))
                .SingleOrDefault();

            return filteredResource;
        }

        public virtual Resource GetResourceByUserGroupPermissions(int groupId, long resourceId, PermissionFlag permission)
        {
            Resource resourceAlias = null;
            Project projectAlias = null;
            Permission permissionAlias = null;
            UserGroup groupAlias = null;

            var filteredResource = GetSession().QueryOver(() => resourceAlias)
                .JoinQueryOver(x => x.Project, () => projectAlias)
                .JoinQueryOver(x => x.Permissions, () => permissionAlias)
                .JoinQueryOver(x => x.UserGroup, () => groupAlias)
                .Where(() => groupAlias.Id == groupId && resourceAlias.Id == resourceId && projectAlias.IsRemoved == false)
                .And(BitwiseExpression.On(() => permissionAlias.Flags).HasBit(permission))
                .SingleOrDefault();

            return filteredResource;
        }

        public virtual IList<Permission> FindPermissionsByGroupAndBooks(int groupId, IList<long> bookIds)
        {
            Project projectAlias = null;
            Permission permissionAlias = null;
            UserGroup groupAlias = null;

            var permissions = GetSession().QueryOver(() => permissionAlias)
                .JoinQueryOver(x => permissionAlias.Project, () => projectAlias)
                .JoinQueryOver(x => permissionAlias.UserGroup, () => groupAlias)
                .Where(() => groupAlias.Id == groupId && projectAlias.IsRemoved == false)
                .AndRestrictionOn(() => projectAlias.Id).IsInG(bookIds)
                .List<Permission>();

            return permissions;
        }

        public virtual void CreatePermissionIfNotExist(Permission permission)
        {
            var tmpPermission = FindPermissionByBookAndGroup(permission.Project.Id, permission.UserGroup.Id);
            if (tmpPermission == null)
            {
                GetSession().Save(permission);
            }
        }

        public virtual Permission FindPermissionByBookAndGroup(long projectId, int groupId)
        {
            return
                GetSession().QueryOver<Permission>()
                    .Where(
                        permission =>
                            permission.Project.Id == projectId &&
                            permission.UserGroup.Id == groupId)
                    .SingleOrDefault<Permission>();
        }

        public virtual Permission FindPermissionByBookAndGroupExternalId(long projectId, int externalId)
        {
            RoleUserGroup groupAlias = null;

            return
                GetSession().QueryOver<Permission>()
                    .JoinAlias(x => x.UserGroup, () => groupAlias)
                    .Where(
                        permission =>
                            permission.Project.Id == projectId &&
                            groupAlias.ExternalId == externalId)
                    .SingleOrDefault<Permission>();
        }

        public virtual ListWithTotalCountResult<UserGroup> FindGroupsByBook(long bookId, int start, int count, string filterByName, bool fetchUser = false)
        {
            Project projectAlias = null;
            Permission permissionAlias = null;
            UserGroup groupAlias = null;

            var query = GetSession().QueryOver(() => groupAlias)
                .JoinAlias(x => groupAlias.Permissions, () => permissionAlias)
                .JoinAlias(x => permissionAlias.Project, () => projectAlias)
                .Where(() => projectAlias.Id == bookId && projectAlias.IsRemoved == false);

            if (!string.IsNullOrEmpty(filterByName))
            {
                query.WhereRestrictionOn( () => groupAlias.Name).IsInsensitiveLike(filterByName, MatchMode.Anywhere);
            }

            query.OrderBy(x => x.GroupType).Asc
                .OrderBy(x => x.Name).Asc
                .Skip(start)
                .Take(count);

            var list = query.Future();
            var totalCount = query.ToRowCountQuery().FutureValue<int>();

            var result = new ListWithTotalCountResult<UserGroup>
            {
                List = list.ToList(),
                Count = totalCount.Value
            };

            if (fetchUser)
            {
                GetSession().QueryOver<SingleUserGroup>()
                    .WhereRestrictionOn(x => x.Id).IsInG(result.List.Select(x => x.Id))
                    .Fetch(SelectMode.Fetch, x => x.User)
                    .List();
            }

            return result;
        }

        public virtual IList<Permission> FindPermissionsForSnapshotByUserId(long snapshotId, int userId)
        {
            UserGroup userGroupAlias = null;
            User userAlias = null;
            Project projectAlias = null;
            Snapshot snapshotAlias = null;

            return GetSession().QueryOver<Permission>()
                .JoinAlias(x => x.UserGroup, () => userGroupAlias)
                .JoinAlias(() => userGroupAlias.Users, () => userAlias)
                .JoinAlias(x => x.Project, () => projectAlias)
                .JoinAlias(() => projectAlias.Snapshots, () => snapshotAlias)
                .Where(() => snapshotAlias.Id == snapshotId && userAlias.Id == userId && projectAlias.IsRemoved == false)
                .List();
        }

        public virtual Permission FindPermissionForSnapshotByGroupId(long snapshotId, int userGroupId)
        {
            UserGroup userGroupAlias = null;
            Project projectAlias = null;
            Snapshot snapshotAlias = null;

            return GetSession().QueryOver<Permission>()
                .JoinAlias(x => x.UserGroup, () => userGroupAlias)
                .JoinAlias(x => x.Project, () => projectAlias)
                .JoinAlias(() => projectAlias.Snapshots, () => snapshotAlias)
                .Where(() => snapshotAlias.Id == snapshotId && userGroupAlias.Id == userGroupId && projectAlias.IsRemoved == false)
                .SingleOrDefault();
        }
    }
}