using System;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.Core.Managers.Authentication;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.MainService.Core.Works.Users
{
    public class RenewCommunicationTokenWork : UnitOfWorkBase
    {
        private readonly UserRepository m_userRepository;
        private readonly ICommunicationTokenGenerator m_communicationTokenGenerator;
        private readonly int m_userId;
        private string m_communicationToken;

        public RenewCommunicationTokenWork(UserRepository userRepository, ICommunicationTokenGenerator communicationTokenGenerator, int userId) : base(userRepository)
        {
            m_userRepository = userRepository;
            m_communicationTokenGenerator = communicationTokenGenerator;
            m_userId = userId;
        }

        protected override void ExecuteWorkImplementation()
        {
            var now = DateTime.UtcNow;
            var user = m_userRepository.FindById<User>(m_userId);
            
            m_communicationToken = m_communicationTokenGenerator.GetNewCommunicationToken(user);
            user.CommunicationToken = m_communicationToken;
            user.CommunicationTokenCreateTime = now;

            m_userRepository.Update(user);
        }

        public string CommunicationToken => m_communicationToken;
    }
}
