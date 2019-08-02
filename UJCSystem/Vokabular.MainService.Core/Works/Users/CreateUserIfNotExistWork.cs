using System;
using System.Collections.Generic;
using System.Linq;
using Ridics.Authentication.DataContracts;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.MainService.Core.Works.Users
{
    public class CreateUserIfNotExistWork : UnitOfWorkBase<int>
    {
        private readonly UserRepository m_userRepository;
        private readonly int m_userExternalId;
        private readonly IList<RoleContractBase> m_roles;

        public CreateUserIfNotExistWork(UserRepository userRepository, int userExternalId, IList<RoleContractBase> roles) : base(userRepository)
        {
            m_userRepository = userRepository;
            m_userExternalId = userExternalId;
            m_roles = roles;
        }

        protected override int ExecuteWorkImplementation()
        {
            IList<UserGroup> dbUserGroups = null;

            var now = DateTime.UtcNow;
            
            if (m_roles != null)
            {
                dbUserGroups = UpdateAndGetUserGroups(now);
            }

            var user = m_userRepository.GetUserByExternalId(m_userExternalId);
            if (user != null)
            {
                if (dbUserGroups != null)
                {
                    user.Groups = dbUserGroups;
                    m_userRepository.Update(user);
                }

                return user.Id;
            }
           
            var dbUser = new User
            {
                ExternalId = m_userExternalId,
                CreateTime = now,
                Groups = dbUserGroups,
                //Groups = new List<Group> { m_defaultMembershipProvider.GetDefaultRegisteredUserGroup(), m_defaultMembershipProvider.GetDefaultUnRegisteredUserGroup() },
                //FavoriteLabels = new List<FavoriteLabel> { defaultFavoriteLabel }
            };

            //defaultFavoriteLabel.User = dbUser;
            // TODO generate default FavoriteLabel
            // TODO assign User Groups

            var userId = (int) m_userRepository.Create(dbUser);
            return userId;
        }

        private IList<UserGroup> UpdateAndGetUserGroups(DateTime now)
        {
            var dbUserGroups = m_userRepository.GetUserGroupsByExternalIds(m_roles.Select(x => x.Id));

            foreach (var roleContract in m_roles)
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