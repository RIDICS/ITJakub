using System.Collections.Generic;
using Castle.Facilities.NHibernateIntegration;
using Castle.Services.Transaction;
using ITJakub.DataEntities.Database.Daos;
using ITJakub.DataEntities.Database.Entities;
using NHibernate.Criterion;

namespace ITJakub.DataEntities.Database.Repositories
{
    [Transactional]
    public class PermissionRepository : NHibernateTransactionalDao
    {
        public PermissionRepository(ISessionManager sessManager)
            : base(sessManager)
        {
        }

        [Transaction(TransactionMode.Requires)]
        public virtual Group FindGroupById(int groupId)
        {
            using (var session = GetSession())
            {
                var group = session.QueryOver<Group>()
                    .Fetch(g => g.Users).Eager
                    .Fetch(g => g.CreatedBy).Eager
                    .Where(g => g.Id == groupId)
                    .SingleOrDefault();

                return group;
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual IList<Group> GetLastGroups(int recordCount)
        {
            using (var session = GetSession())
            {
                return session.QueryOver<Group>()
                    .OrderBy(x => x.CreateTime).Desc
                    .Take(recordCount)
                    .List<Group>();
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual IList<Group> GetTypeaheadGroups(string query, int recordCount)
        {
            using (var session = GetSession())
            {
                return session.QueryOver<Group>()
                    .Where(Restrictions.On<Group>(x => x.Name).IsInsensitiveLike(query))
                    .Take(recordCount)
                    .List<Group>();
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual IList<Book> GetAllowedBooksByGroup(int groupId)
        {

            using (var session = GetSession())
            {
                Book bookAlias = null;
                Permission permissionAlias = null;
                Group groupAlias = null;

                var books = session.QueryOver(() => bookAlias)
                    .JoinQueryOver(x => x.Permissions, () => permissionAlias)
                    .JoinQueryOver(x => permissionAlias.Group, () => groupAlias)
                    .Where(x => groupAlias.Id == groupId)
                    .List<Book>();

                return books;
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual IList<User> GetUsersByGroup(int groupId)
        {
            using (var session = GetSession())
            {
                User userAlias = null;
                Group groupAlias = null;

                var users = session.QueryOver(() => userAlias)
                    .JoinQueryOver(x => x.Groups, () => groupAlias)
                    .Where(x => groupAlias.Id == groupId)
                    .List<User>();

                return users;
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual IList<Group> GetGroupsByUser(int userId)
        {
            using (var session = GetSession())
            {
                User userAlias = null;
                Group groupAlias = null;

                var groups = session.QueryOver(() => groupAlias)
                    .JoinQueryOver(x => x.Users, () => userAlias)
                    .Where(x => userAlias.Id == userId)
                    .List<Group>();

                return groups;
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual int CreateGroup(Group group)
        {
            using (var session = GetSession())
            {
                return (int) session.Save(group);
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual IList<string> GetFilteredBookXmlIdListByUserPermissions(int userId, IEnumerable<string> bookXmlIds)
        {
            using (var session = GetSession())
            {
                Book bookAlias = null;
                Permission permissionAlias = null;
                Group groupAlias = null;
                User userAlias = null;

                var filteredBookXmlIds = session.QueryOver(() => bookAlias)
                    .JoinQueryOver(x => x.Permissions, () => permissionAlias)
                    .JoinQueryOver(x => permissionAlias.Group, () => groupAlias)
                    .JoinQueryOver(x => groupAlias.Users, () => userAlias)
                    .Select(Projections.Distinct(Projections.Property(() => bookAlias.Guid)))
                    .Where(() => userAlias.Id == userId)
                    .AndRestrictionOn(() => bookAlias.Guid).IsInG(bookXmlIds)
                    .List<string>();

                return filteredBookXmlIds;
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual IList<long> GetFilteredBookIdListByUserPermissions(int userId, IEnumerable<long> bookIds)
        {
            using (var session = GetSession())
            {
                Book bookAlias = null;
                Permission permissionAlias = null;
                Group groupAlias = null;
                User userAlias = null;

                var filteredBookIds = session.QueryOver(() => bookAlias)
                    .JoinQueryOver(x => x.Permissions, () => permissionAlias)
                    .JoinQueryOver(x => permissionAlias.Group, () => groupAlias)
                    .JoinQueryOver(x => groupAlias.Users, () => userAlias)
                    .Select(Projections.Distinct(Projections.Property(() => bookAlias.Id)))
                    .Where(() => userAlias.Id == userId)
                    .AndRestrictionOn(() => bookAlias.Id).IsInG(bookIds)
                    .List<long>();

                return filteredBookIds;
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual IList<long> GetFilteredBookIdListByGroupPermissions(int groupId, IEnumerable<long> bookIds)
        {
            using (var session = GetSession())
            {
                Book bookAlias = null;
                Permission permissionAlias = null;
                Group groupAlias = null;

                var filteredBookIds = session.QueryOver(() => bookAlias)
                    .JoinQueryOver(x => x.Permissions, () => permissionAlias)
                    .JoinQueryOver(x => permissionAlias.Group, () => groupAlias)
                    .Select(Projections.Distinct(Projections.Property(() => bookAlias.Id)))
                    .Where(() => groupAlias.Id == groupId)
                    .AndRestrictionOn(() => bookAlias.Id).IsInG(bookIds)
                    .List<long>();

                return filteredBookIds;
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void CreatePermission(Permission permission)
        {
            using (var session = GetSession())
            {
                session.Save(permission);
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual IList<Permission> FindPermissionsByGroupAndBooks(int groupId, IList<long> bookIds)
        {
            using (var session = GetSession())
            {
                Book bookAlias = null;
                Permission permissionAlias = null;
                Group groupAlias = null;

                var permissions = session.QueryOver(() => permissionAlias)
                    .JoinQueryOver(x => permissionAlias.Book, () => bookAlias)
                    .JoinQueryOver(x => permissionAlias.Group, () => groupAlias)
                    .Where(() => groupAlias.Id == groupId)
                    .AndRestrictionOn(() => bookAlias.Id).IsInG(bookIds)
                    .List<Permission>();

                return permissions;
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeletePermissions(IList<Permission> permissionsList)
        {
            using (var session = GetSession())
            {
                foreach (var permission in permissionsList)
                {
                    session.Delete(permission);
                }
            }
        }
    }
}