using System.Collections.Generic;
using NHibernate.Criterion;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.Shared.DataEntities.Daos;
using Vokabular.Shared.DataEntities.UnitOfWork;

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

        public virtual UserGroup FindGroupWithSpecialPermissionsById(int groupId)
        {
            var group = GetSession().QueryOver<UserGroup>()
                .Fetch(g => g.CreatedBy).Eager
                .Fetch(g => g.SpecialPermissions).Eager
                .Where(g => g.Id == groupId)
                .SingleOrDefault();

            return group;
        }

        public virtual IList<UserGroup> GetGroupsAutocomplete(string queryString, int recordCount)
        {
            queryString = EscapeQuery(queryString);

            return GetSession().QueryOver<UserGroup>()
                .Where(Restrictions.On<UserGroup>(x => x.Name).IsInsensitiveLike(queryString, MatchMode.Anywhere))
                .OrderBy(x => x.Name).Asc
                .Take(recordCount)
                .List<UserGroup>();
        }

        public virtual IList<User> GetUsersByGroup(int groupId)
        {
            User userAlias = null;
            UserGroup groupAlias = null;

            var users = GetSession().QueryOver(() => userAlias)
                .JoinQueryOver(x => x.Groups, () => groupAlias)
                .Where(x => groupAlias.Id == groupId)
                .List<User>();

            return users;
        }

        public virtual User GetUserWithGroups(int userId)
        {
            var user = GetSession().QueryOver<User>()
                .Fetch(x => x.Groups).Eager
                .Where(x => x.Id == userId)
                .SingleOrDefault();

            return user;
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
        
        public virtual IList<SpecialPermission> GetSpecialPermissionsByUser(int userId)
        {
            SpecialPermission specPermissionAlias = null;
            UserGroup groupAlias = null;
            User userAlias = null;

            var permissions = GetSession().QueryOver(() => specPermissionAlias)
                .JoinQueryOver(x => specPermissionAlias.UserGroups, () => groupAlias)
                .JoinQueryOver(x => groupAlias.Users, () => userAlias)
                .Where(() => userAlias.Id == userId)
                .List<SpecialPermission>();

            return permissions;
        }
        
        public virtual IList<SpecialPermission> GetSpecialPermissionsByUserAndType(int userId, SpecialPermissionCategorization type)
        {
            SpecialPermission specPermissionAlias = null;
            UserGroup groupAlias = null;
            User userAlias = null;

            var permissions = GetSession().QueryOver(() => specPermissionAlias)
                .JoinQueryOver(x => specPermissionAlias.UserGroups, () => groupAlias)
                .JoinQueryOver(x => groupAlias.Users, () => userAlias)
                .Where(() => userAlias.Id == userId)
                .And(()=> specPermissionAlias.PermissionCategorization == type)
                .List<SpecialPermission>();

            return permissions;
        }

        public virtual IList<SpecialPermission> GetSpecialPermissions()
        {
            SpecialPermission specPermissionAlias = null;

            var permissions = GetSession().QueryOver(() => specPermissionAlias)
                .List<SpecialPermission>();

            return permissions;
        }

        public virtual IList<SpecialPermission> GetSpecialPermissionsByGroup(int groupId)
        {
            SpecialPermission specPermissionAlias = null;
            UserGroup groupAlias = null;

            var permissions = GetSession().QueryOver(() => specPermissionAlias)
                .JoinQueryOver(x => specPermissionAlias.UserGroups, () => groupAlias)
                .Where(() => groupAlias.Id == groupId)
                .List<SpecialPermission>();

            return permissions;
        }

        public virtual IList<SpecialPermission> GetSpecialPermissionsByIds(IEnumerable<int> specialPermissionIds)
        {
            SpecialPermission specPermissionAlias = null;

            var permissions = GetSession().QueryOver(() => specPermissionAlias)
                .AndRestrictionOn(() => specPermissionAlias.Id).IsInG(specialPermissionIds)
                .List<SpecialPermission>();

            return permissions;
        }

        public virtual IList<AutoImportBookTypePermission> GetAutoimportPermissionsByBookTypeList(IEnumerable<BookTypeEnum> bookTypes)
        {
            AutoImportBookTypePermission autoimportPermissionAlias = null;
            BookType bookTypeAlias = null;

            var permissions = GetSession().QueryOver(() => autoimportPermissionAlias)
                .JoinQueryOver(x => autoimportPermissionAlias.BookType, () => bookTypeAlias)
                .AndRestrictionOn(() => bookTypeAlias.Type).IsInG(bookTypes)
                .List<AutoImportBookTypePermission>();

            return permissions;
        }

        public virtual IList<UserGroup> GetGroupsBySpecialPermissionIds(IEnumerable<int> specialPermissionIds)
        {
            UserGroup groupAlias = null;
            SpecialPermission specialPermissionAlias = null;

            var groups = GetSession().QueryOver(() => groupAlias)
                .JoinQueryOver(x => groupAlias.SpecialPermissions, () => specialPermissionAlias)
                .AndRestrictionOn(() => specialPermissionAlias.Id).IsInG(specialPermissionIds)
                .List<UserGroup>();

            return groups;
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