using System;
using System.Net;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;
using Vokabular.Jewelry;
using Vokabular.MainService.Core.Managers.Authentication;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.RestClient.Errors;

namespace Vokabular.MainService.Core.Works.Users
{
    public class SignInWork : UnitOfWorkBase
    {
        private readonly UserRepository m_userRepository;
        private readonly ICommunicationTokenGenerator m_communicationTokenGenerator;
        private readonly SignInContract m_data;
        private string m_communicationToken;

        public SignInWork(UserRepository userRepository, ICommunicationTokenGenerator communicationTokenGenerator, SignInContract data) : base(userRepository)
        {
            m_userRepository = userRepository;
            m_communicationTokenGenerator = communicationTokenGenerator;
            m_data = data;
        }

        protected override void ExecuteWorkImplementation()
        {
            var now = DateTime.UtcNow;
            var user = m_userRepository.GetUserByUsername(m_data.Username);

            if (user == null || !CustomPasswordHasher.ValidatePassword(m_data.Password, user.PasswordHash))
            {
                throw new HttpErrorCodeException("Invalid login or password", HttpStatusCode.Unauthorized);
            }

            m_communicationToken = m_communicationTokenGenerator.GetNewCommunicationToken(user);
            user.CommunicationToken = m_communicationToken;
            user.CommunicationTokenCreateTime = now;

            m_userRepository.Update(user);
        }

        public string CommunicationToken => m_communicationToken;
    }
}