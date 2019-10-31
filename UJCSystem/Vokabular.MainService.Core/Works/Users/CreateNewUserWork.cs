using System;
using System.Collections.Generic;
using Ridics.Authentication.DataContracts.User;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.Core.Communication;
using CreateUserContract = Vokabular.MainService.DataContracts.Contracts.CreateUserContract;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.MainService.Core.Works.Users
{
    public class CreateNewUserWork : UnitOfWorkBase<int>
    {
        private readonly UserRepository m_userRepository;
        private readonly CommunicationProvider m_communicationProvider;
        private readonly CreateUserContract m_data;
        private readonly string m_newUserGroupCode;

        public CreateNewUserWork(UserRepository userRepository, CommunicationProvider communicationProvider, CreateUserContract data, string newUserGroupCode) : base(userRepository)
        {
            m_userRepository = userRepository;
            m_communicationProvider = communicationProvider;
            m_data = data;
            m_newUserGroupCode = newUserGroupCode;
        }

        protected override int ExecuteWorkImplementation()
        {
            var client = m_communicationProvider.GetAuthRegistrationApiClient();

            var authUser = new Ridics.Authentication.DataContracts.User.CreateUserContract
            {
                Password = m_data.NewPassword,
                UserName = m_data.UserName,
                User = new UserContractBase
                {
                    FirstName = m_data.FirstName,
                    LastName = m_data.LastName,
                    Email = m_data.Email,
                    PhoneNumber = null
                }
            };

            var user = client.CreateUserAsync(authUser).GetAwaiter().GetResult();

            var now = DateTime.UtcNow;

            var dbUser = new User
            {
                ExternalId = user.Id,
                CreateTime = now,
                ExtUsername = user.UserName,
                ExtFirstName = user.FirstName,
                ExtLastName = user.LastName,
                Groups = null,
                //FavoriteLabels = new List<FavoriteLabel> { defaultFavoriteLabel }
            };

            var singleUserGroup = new SingleUserGroup
            {
                Name = m_newUserGroupCode,
                CreateTime = now,
                LastChange = now,
                User = dbUser,
                Users = new List<User> {dbUser},
                Permissions = null,
            };

            dbUser.Groups = new List<UserGroup> {singleUserGroup};
            // RoleUserGroups are assigned on every login


            //defaultFavoriteLabel.User = dbUser;
            // TODO generate default FavoriteLabel

            var userId = (int) m_userRepository.Create(dbUser);
            return userId;
        }
    }
}