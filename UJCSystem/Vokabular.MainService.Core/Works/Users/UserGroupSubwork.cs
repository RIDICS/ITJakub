using System;
using System.Collections.Generic;
using System.Linq;
using Ridics.Authentication.DataContracts;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;

namespace Vokabular.MainService.Core.Works.Users
{
    public class UserGroupSubwork
    {
        private readonly UserRepository m_userRepository;

        public UserGroupSubwork(UserRepository userRepository)
        {
            m_userRepository = userRepository;
        }

        public IList<UserGroup> UpdateAndGetUserGroups(IList<RoleContractBase> roles)
        {
            var now = DateTime.UtcNow;
            var dbUserGroups = m_userRepository.GetUserGroupsByExternalIds(roles.Select(x => x.Id));

            foreach (var roleContract in roles)
            {
                var dbRole = dbUserGroups.FirstOrDefault(x => x.ExternalId == roleContract.Id);
                if (dbRole == null)
                {
                    var newDbRole = new UserGroup
                    {
                        ExternalId = roleContract.Id,
                        Name = roleContract.Name,
                        CreateTime = now,
                        LastChange = now,
                    };

                    m_userRepository.Create(newDbRole);

                    dbUserGroups.Add(newDbRole);
                }
            }

            return dbUserGroups;
        }
    }
}