using System;
using System.Collections.Generic;
using System.Linq;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.SelectResults;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.DataEntities.Database.Repositories
{
    public class UserRepository : MainDbRepositoryBase
    {
        public UserRepository(UnitOfWorkProvider unitOfWorkProvider) : base(unitOfWorkProvider)
        {
        }

        public User GetUserById(int userId)
        {
            return GetSession().QueryOver<User>()
                .Where(x => x.Id == userId)
                .SingleOrDefault();
        }

        public User GetUserByExternalId(int externalUserId)
        {
            return GetSession().QueryOver<User>()
                .Where(x => x.ExternalId == externalUserId)
                .SingleOrDefault();
        }

        public virtual User GetVirtualUserForUnregisteredUsersOrCreate(UserGroup unregisteredUserGroup)
        {
            User userAlias = null;
            UserGroup groupAlias = null;

            var defaultUser = GetSession().QueryOver(() => userAlias)
                .JoinQueryOver(x => x.Groups, () => groupAlias)
                .Where(x => groupAlias.Id == unregisteredUserGroup.Id)
                .SingleOrDefault();

            if (defaultUser != null)
            {
                return defaultUser;
            }

            var now = DateTime.UtcNow;
            defaultUser = new User
            {
                CreateTime = now,
                Groups = new List<UserGroup> { unregisteredUserGroup },
            };

            GetSession().Save(defaultUser);

            return defaultUser;
        }

        public virtual RoleUserGroup GetDefaultGroupOrCreate(string defaultRegisteredGroupName, Func<int> getExternalId)
        {
            var registeredUsersGroup = GetSession().QueryOver<RoleUserGroup>()
                .Where(x => x.Name == defaultRegisteredGroupName)
                .SingleOrDefault();

            if (registeredUsersGroup != null)
            {
                return registeredUsersGroup;
            }

            var now = DateTime.UtcNow;
            registeredUsersGroup = new RoleUserGroup
            {
                Name = defaultRegisteredGroupName,
                CreateTime = now,
                LastChange = now,
                ExternalId = getExternalId.Invoke(),
            };

            GetSession().Save(registeredUsersGroup);

            return registeredUsersGroup;
        }

        public virtual IList<RoleUserGroup> GetUserGroupsByExternalIds(IEnumerable<int> externalIds)
        {
            var result = GetSession().QueryOver<RoleUserGroup>()
                .WhereRestrictionOn(x => x.ExternalId).IsInG(externalIds)
                .List();
            return result;
        }

        public virtual ListWithTotalCountResult<User> GetUsersByGroup(int groupId, int start, int count, string filterByName)
        {
            UserGroup userGroupAlias = null;

            var query = GetSession().QueryOver<User>()
                .JoinAlias(x => x.Groups, () => userGroupAlias)
                .Where(() => userGroupAlias.Id == groupId)
                // TODO add Name column to allow filtering and sorting
                //.WhereRestrictionOn(x => x.name).IsLike(filterByName, MatchMode.Anywhere)
                //.OrderBy(x => x.name).Asc
                .Skip(start)
                .Take(count);

            var result = query.Future();
            var resultCount = query.ToRowCountQuery().FutureValue<int>();
                
            return new ListWithTotalCountResult<User>
            {
                List = result.ToList(),
                Count = resultCount.Value,
            };
        }
    }
}
