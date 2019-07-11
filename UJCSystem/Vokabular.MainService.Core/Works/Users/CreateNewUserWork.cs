﻿using System;
using Ridics.Authentication.DataContracts.User;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;
using Vokabular.MainService.Core.Communication;
using CreateUserContract = Vokabular.MainService.DataContracts.Contracts.CreateUserContract;

namespace Vokabular.MainService.Core.Works.Users
{
    public class CreateNewUserWork : UnitOfWorkBase<int>
    {
        private readonly UserRepository m_userRepository;
        private readonly CommunicationProvider m_communicationProvider;
        private readonly CreateUserContract m_data;

        public CreateNewUserWork(UserRepository userRepository, CommunicationProvider communicationProvider, CreateUserContract data) : base(userRepository)
        {
            m_userRepository = userRepository;
            m_communicationProvider = communicationProvider;
            m_data = data;
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
}