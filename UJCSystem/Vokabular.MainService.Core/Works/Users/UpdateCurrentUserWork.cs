using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;
using Vokabular.MainService.Core.Communication;
using Vokabular.MainService.DataContracts.Contracts;
using AuthUserContract = Vokabular.Authentication.DataContracts.User.UserContract;

namespace Vokabular.MainService.Core.Works.Users
{
    public class UpdateCurrentUserWork : UnitOfWorkBase
    {
        private readonly UserRepository m_userRepository;
        private readonly int m_userId;
        private readonly UpdateUserContract m_data;
        private readonly CommunicationProvider m_communicationProvider;

        public UpdateCurrentUserWork(UserRepository userRepository, int userId, UpdateUserContract data, CommunicationProvider communicationProvider) : base(userRepository)
        {
            m_userRepository = userRepository;
            m_userId = userId;
            m_data = data;
            m_communicationProvider = communicationProvider;
        }

        protected override void ExecuteWorkImplementation()
        {
            var user = m_userRepository.FindById<User>(m_userId);

            var client = m_communicationProvider.GetAuthUserApiClient();

            var authUser = client.HttpClient.GetItemAsync<AuthUserContract>(user.ExternalId).GetAwaiter().GetResult();

            authUser.Email = m_data.Email;
            authUser.FirstName = m_data.FirstName;
            authUser.LastName = m_data.LastName;

            client.EditSelfAsync(user.ExternalId, authUser).GetAwaiter().GetResult();


            user.AvatarUrl = m_data.AvatarUrl;
            m_userRepository.Update(user);
        }
    }
}