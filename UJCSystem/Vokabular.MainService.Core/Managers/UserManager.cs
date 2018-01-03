using AutoMapper;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;
using Vokabular.MainService.Core.Managers.Authentication;
using Vokabular.MainService.Core.Works.Users;
using Vokabular.MainService.DataContracts.Contracts;

namespace Vokabular.MainService.Core.Managers
{
    public class UserManager
    {
        private readonly UserRepository m_userRepository;
        private readonly ICommunicationTokenGenerator m_communicationTokenGenerator;

        public UserManager(UserRepository userRepository, ICommunicationTokenGenerator communicationTokenGenerator)
        {
            m_userRepository = userRepository;
            m_communicationTokenGenerator = communicationTokenGenerator;
        }

        public int CreateNewUser(CreateUserContract data)
        {
            // TODO add data validation (min lenght, e-mail is valid, etc.)

            var userId = new CreateNewUserWork(m_userRepository, m_communicationTokenGenerator, data).Execute();
            return userId;
        }

        public UserDetailContract GetUserByToken(string authorizationToken)
        {
            var dbUser = m_userRepository.InvokeUnitOfWork(x => x.GetUserByToken(authorizationToken));
            var result = Mapper.Map<UserDetailContract>(dbUser);
            return result;
        }

        public void UpdateUser(string authorizationToken, UpdateUserContract data)
        {
            // TODO add data validation

            new UpdateUserWork(m_userRepository, authorizationToken, data).Execute();
        }

        public void UpdateUserPassword(string authorizationToken, UpdateUserPasswordContract data)
        {
            // TODO add data validation

            new UpdateUserPasswordWork(m_userRepository, authorizationToken, data).Execute();
        }
    }
}