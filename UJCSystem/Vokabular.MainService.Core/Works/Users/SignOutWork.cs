using System;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;

namespace Vokabular.MainService.Core.Works.Users
{
    public class SignOutWork : UnitOfWorkBase
    {
        private readonly UserRepository m_userRepository;
        private readonly string m_authorizationToken;

        public SignOutWork(UserRepository userRepository, string authorizationToken) : base(userRepository)
        {
            m_userRepository = userRepository;
            m_authorizationToken = authorizationToken;
        }

        protected override void ExecuteWorkImplementation()
        {
            var now = DateTime.UtcNow;
            var user = m_userRepository.GetUserByToken(m_authorizationToken);

            if (user == null)
                return;

            user.CommunicationToken = Guid.NewGuid().ToString(); //TODO currently not null constraint exists
            user.CommunicationTokenCreateTime = now;

            m_userRepository.Update(user);
        }
    }
}