using System.Collections.Generic;
using NHibernate.Criterion;
using Vokabular.DataEntities.Database.Daos;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.UnitOfWork;

namespace Vokabular.DataEntities.Database.Repositories
{
    public class PermissionRepository : NHibernateDao
    {
        public PermissionRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public virtual UserGroup FindGroupById(int groupId)
        {
            var group = GetSession().QueryOver<UserGroup>()
                .Fetch(g => g.Users).Eager
                .Fetch(g => g.CreatedBy).Eager
                .Where(g => g.Id == groupId)
                .SingleOrDefault();

            return group;
        }

        public virtual UserGroup GetGroupByName(string groupName)
        {
            return GetSession().QueryOver<UserGroup>()
                .Where(g => g.Name == groupName)
                .SingleOrDefault();
        }

        public virtual int CreateGroup(UserGroup group)
        {
            return (int) GetSession().Save(group);
        }

        public virtual IList<string> GetFilteredBookXmlIdListByUserPermissions(int userId, IEnumerable<string> projectExternalIds)
        {
            Project projectAlias = null;
            Permission permissionAlias = null;
            UserGroup groupAlias = null;
            User userAlias = null;

            var filteredBookXmlIds = GetSession().QueryOver(() => projectAlias)
                .JoinQueryOver(x => x.Permissions, () => permissionAlias)
                .JoinQueryOver(x => permissionAlias.UserGroup, () => groupAlias)
                .JoinQueryOver(x => groupAlias.Users, () => userAlias)
                .Select(Projections.Distinct(Projections.Property(() => projectAlias.ExternalId)))
                .Where(() => userAlias.Id == userId)
                .AndRestrictionOn(() => projectAlias.ExternalId).IsInG(projectExternalIds)
                .List<string>();

            return filteredBookXmlIds;
        }

        public virtual IList<long> GetFilteredBookIdListByUserPermissions(int userId, IEnumerable<long> bookIds)
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
                .Where(() => userAlias.Id == userId)
                .AndRestrictionOn(() => projectAlias.Id).IsInG(bookIds)
                .List<long>();

            return filteredBookIds;
        }

        public virtual IList<long> GetFilteredBookIdListByGroupPermissions(int groupId, IEnumerable<long> bookIds)
        {
            Project projectAlias = null;
            Permission permissionAlias = null;
            UserGroup groupAlias = null;

            var filteredBookIds = GetSession().QueryOver(() => projectAlias)
                .JoinQueryOver(x => x.Permissions, () => permissionAlias)
                .JoinQueryOver(x => permissionAlias.UserGroup, () => groupAlias)
                .Select(Projections.Distinct(Projections.Property(() => projectAlias.Id)))
                .Where(() => groupAlias.Id == groupId)
                .AndRestrictionOn(() => projectAlias.Id).IsInG(bookIds)
                .List<long>();

            return filteredBookIds;
        }

        public virtual Resource GetResourceByUserPermissions(int userId, long resourceId)
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
                .Where(() => userAlias.Id == userId && resourceAlias.Id == resourceId)
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
                .Where(() => groupAlias.Id == groupId)
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
    }
}