using Vokabular.Authentication.DataContracts.User;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;
using Vokabular.MainService.Core.Communication;
using Vokabular.MainService.DataContracts.Contracts;

namespace Vokabular.MainService.Core.Works.Users
{
    public class UpdateUserPasswordWork : UnitOfWorkBase
    {
        private readonly UserRepository m_userRepository;
        private readonly int m_userId;
        private readonly UpdateUserPasswordContract m_data;
        private readonly CommunicationProvider m_communicationProvider;

        public UpdateUserPasswordWork(UserRepository userRepository, int userId, UpdateUserPasswordContract data, CommunicationProvider communicationProvider) : base(userRepository)
        {
            m_userRepository = userRepository;
            m_userId = userId;
            m_data = data;
            m_communicationProvider = communicationProvider;
        }

        protected override void ExecuteWorkImplementation()
        {
            var user = m_userRepository.FindById<User>(m_userId);

            var contract = new ChangePasswordContract
            {
                OriginalPassword = m_data.OldPassword,
                Password = m_data.NewPassword
            };

            using (var client = m_communicationProvider.GetAuthenticationServiceClient())
            {
                client.ChangePassword(user.ExternalId, contract);
            }
        }
    }
}