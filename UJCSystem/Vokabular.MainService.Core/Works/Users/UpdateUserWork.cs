using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;
using Vokabular.MainService.Core.Communication;
using Vokabular.MainService.DataContracts.Contracts;

namespace Vokabular.MainService.Core.Works.Users
{
    public class UpdateUserWork : UnitOfWorkBase
    {
        private readonly UserRepository m_userRepository;
        private readonly int m_userId;
        private readonly UpdateUserContract m_data;
        private readonly CommunicationProvider m_communicationProvider;

        public UpdateUserWork(UserRepository userRepository, int userId, UpdateUserContract data, CommunicationProvider communicationProvider) : base(userRepository)
        {
            m_userRepository = userRepository;
            m_userId = userId;
            m_data = data;
            m_communicationProvider = communicationProvider;
        }

        protected override void ExecuteWorkImplementation()
        {
            var user = m_userRepository.FindById<User>(m_userId);

            using (var client = m_communicationProvider.GetAuthenticationServiceClient())
            {
                var authUser = client.GetUser(user.ExternalId);

                authUser.Email = m_data.Email;
                authUser.FirstName = m_data.FirstName;
                authUser.FamilyName = m_data.LastName;

                client.EditUser(user.ExternalId, authUser);
            }

            user.AvatarUrl = m_data.AvatarUrl;
            user.Email = m_data.Email;
            user.FirstName = m_data.FirstName;
            user.LastName = m_data.LastName;
            
            m_userRepository.Update(user);
        }
    }
}