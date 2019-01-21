using System;
using System.Collections.Generic;
using Vokabular.DataEntities.Database.Daos;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.UnitOfWork;

namespace Vokabular.DataEntities.Database.Repositories
{
    public class UserRepository : NHibernateDao
    {
        public UserRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
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
                AvatarUrl = null,
            };

            GetSession().Save(defaultUser);

            return defaultUser;
        }

        public virtual UserGroup GetDefaultGroupOrCreate(string defaultRegisteredGroupName)
        {
            var registeredUsersGroup = GetSession().QueryOver<UserGroup>()
                .Where(x => x.Name == defaultRegisteredGroupName)
                .SingleOrDefault<UserGroup>();

            if (registeredUsersGroup != null)
            {
                return registeredUsersGroup;
            }

            var now = DateTime.UtcNow;
            registeredUsersGroup = new UserGroup
            {
                Name = defaultRegisteredGroupName,
                CreateTime = now,
                Description = "Default user group",
            };

            GetSession().Save(registeredUsersGroup);

            return registeredUsersGroup;
        }
    }
}
