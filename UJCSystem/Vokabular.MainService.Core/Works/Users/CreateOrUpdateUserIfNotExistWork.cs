using System;
using System.Collections.Generic;
using System.Linq;
using Ridics.Authentication.DataContracts;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.Core.Utils;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.MainService.Core.Works.Users
{
    public class CreateOrUpdateUserIfNotExistWork : UnitOfWorkBase<int>
    {
        private readonly UserRepository m_userRepository;
        private readonly int m_userExternalId;
        private readonly IList<RoleContractBase> m_roles;
        private readonly UpdateUserInfo m_userInfo;
        private readonly CodeGenerator m_codeGenerator;

        public CreateOrUpdateUserIfNotExistWork(UserRepository userRepository, int userExternalId, IList<RoleContractBase> roles,
            UpdateUserInfo userInfo, CodeGenerator codeGenerator) : base(userRepository)
        {
            m_userRepository = userRepository;
            m_userExternalId = userExternalId;
            m_roles = roles;
            m_userInfo = userInfo;
            m_codeGenerator = codeGenerator;
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
                if (m_userInfo.Username != null) // Username is not always returned
                {
                    user.ExtUsername = m_userInfo.Username;
                }
                user.ExtFirstName = m_userInfo.FirstName;
                user.ExtLastName = m_userInfo.LastName;

                // Update RoleUserGroups
                if (dbRoleUserGroups != null)
                {
                    // User already exists, so only update groups
                    var originalGroups = user.Groups;
                    var nonRoleGroups = originalGroups.Where(x => !(x is RoleUserGroup));

                    var newGroups = new List<UserGroup>(dbRoleUserGroups);
                    newGroups.AddRange(nonRoleGroups);

                    user.Groups = newGroups;
                }

                // Add SingleUserGroup
                if (user.Groups.OfType<SingleUserGroup>().Any() == false)
                {
                    user.Groups.Add(CreateSingleUserGroupObject(user, now));
                }

                m_userRepository.Update(user);

                return user.Id;
            }
           
            var dbUser = new User
            {
                ExternalId = m_userExternalId,
                CreateTime = now,
                Groups = dbRoleUserGroups?.Cast<UserGroup>().ToList() ?? new List<UserGroup>(),
                ExtUsername = m_userInfo.Username,
                ExtFirstName = m_userInfo.FirstName,
                ExtLastName = m_userInfo.LastName,
                //FavoriteLabels = new List<FavoriteLabel> { defaultFavoriteLabel }
            };

            var newSingleUserGroup = CreateSingleUserGroupObject(dbUser, now);
            dbUser.Groups.Add(newSingleUserGroup);


            //defaultFavoriteLabel.User = dbUser;
            // TODO generate default FavoriteLabel
            

            var userId = (int) m_userRepository.Create(dbUser);
            return userId;
        }

        private SingleUserGroup CreateSingleUserGroupObject(User user, DateTime now)
        {
            var newGroupCode = m_codeGenerator.Generate(CodeGenerator.UserGroupNameLength);
            var result = new SingleUserGroup
            {
                Name = newGroupCode,
                CreateTime = now,
                LastChange = now,
                User = user,
                Users = new List<User> {user},
                Permissions = null,
            };
            return result;
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