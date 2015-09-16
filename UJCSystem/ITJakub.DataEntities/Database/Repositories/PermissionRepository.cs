using System.Collections.Generic;
using System.Linq;
using Castle.Facilities.NHibernateIntegration;
using Castle.Services.Transaction;
using ITJakub.DataEntities.Database.Daos;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.DataEntities.Database.Entities.Enums;
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
                    .Select(Projections.Distinct(Projections.Property(() => bookAlias.Guid)))
                    .Where(() => userAlias.Id == userId)
                    .AndRestrictionOn(() => bookAlias.Id).IsInG(bookIds)
                    .List<long>();

                return filteredBookIds;
            }
        }
    }
}