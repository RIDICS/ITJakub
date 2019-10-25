using System;
using System.Collections.Generic;
using System.Linq;
using Ridics.Authentication.DataContracts;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.MainService.Core.Works.Users
{
    public class CreateOrUpdateUserIfNotExistWork : UnitOfWorkBase<int>
    {
        private readonly UserRepository m_userRepository;
        private readonly int m_userExternalId;
        private readonly IList<RoleContractBase> m_roles;
        private readonly UpdateUserInfo m_userInfo;

        public CreateOrUpdateUserIfNotExistWork(UserRepository userRepository, int userExternalId, IList<RoleContractBase> roles, UpdateUserInfo userInfo) : base(userRepository)
        {
            m_userRepository = userRepository;
            m_userExternalId = userExternalId;
            m_roles = roles;
            m_userInfo = userInfo;
        }

        protected override int ExecuteWorkImplementation()
        {
            IList<RoleUserGroup> dbRoleUserGroups = null;

            var now = DateTime.UtcNow;
            
            if (m_roles != null)
            {
                var userGroupSubwork = new UserGroupSubwork(m_userRepository);
                dbRoleUserGroups = userGroupSubwork.UpdateAndGetUserGroups(m_roles);
            }

            var user = m_userRepository.GetUserByExternalId(m_userExternalId);
            if (user != null)
            {
                // Update user data
                user.ExtUsername = m_userInfo.Username;
                user.ExtFirstName = m_userInfo.FirstName;
                user.ExtLastName = m_userInfo.LastName;

                if (dbRoleUserGroups != null)
                {
                    // User already exists, so only update groups
                    var originalGroups = user.Groups;
                    var nonRoleGroups = originalGroups.Where(x => !(x is RoleUserGroup));

                    var newGroups = new List<UserGroup>(dbRoleUserGroups);
                    newGroups.AddRange(nonRoleGroups);

                    user.Groups = newGroups;
                }

                m_userRepository.Update(user);

                return user.Id;
            }
           
            var dbUser = new User
            {
                ExternalId = m_userExternalId,
                CreateTime = now,
                Groups = dbRoleUserGroups?.Cast<UserGroup>().ToList(),
                ExtUsername = m_userInfo.Username,
                ExtFirstName = m_userInfo.FirstName,
                ExtLastName = m_userInfo.LastName,
                //Groups = new List<Group> { m_defaultMembershipProvider.GetDefaultRegisteredUserGroup(), m_defaultMembershipProvider.GetDefaultUnRegisteredUserGroup() },
                //FavoriteLabels = new List<FavoriteLabel> { defaultFavoriteLabel }
            };

            //defaultFavoriteLabel.User = dbUser;
            // TODO generate default FavoriteLabel
            // TODO assign User Groups

            var userId = (int) m_userRepository.Create(dbUser);
            return userId;
        }
    }

    public class UpdateUserInfo
    {
        public UpdateUserInfo(string username, string firstName, string lastName)
        {
            Username = username;
            FirstName = firstName;
            LastName = lastName;
        }

        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}