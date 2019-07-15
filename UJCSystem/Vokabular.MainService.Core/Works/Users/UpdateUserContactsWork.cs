using System;
using Ridics.Authentication.DataContracts;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;
using Vokabular.MainService.Core.Communication;
using Vokabular.MainService.DataContracts.Contracts;

namespace Vokabular.MainService.Core.Works.Users
{
    public class UpdateUserContactsWork : UnitOfWorkBase
    {
        private readonly UserRepository m_userRepository;
        private readonly int m_userId;
        private readonly UpdateUserContactContract m_data;
        private readonly CommunicationProvider m_communicationProvider;

        public UpdateUserContactsWork(UserRepository userRepository, int userId, UpdateUserContactContract data, CommunicationProvider communicationProvider) : base(userRepository)
        {
            m_userRepository = userRepository;
            m_userId = userId;
            m_data = data;
            m_communicationProvider = communicationProvider;
        }

        protected override void ExecuteWorkImplementation()
        {
            var user = m_userRepository.FindById<User>(m_userId);
            if (user.ExternalId == null)
            {
                throw new ArgumentException($"User with ID {user.Id} has missing ExternalID");
            }

            var contract = new ChangeContactContract
            {
                ContactType = m_data.ContactType,
                UserId = user.ExternalId.Value,
                NewContactValue = m_data.NewContactValue
            };

            var client = m_communicationProvider.GetAuthContactApiClient();
            client.ChangeContactAsync(contract).GetAwaiter().GetResult();
        }
    }
}