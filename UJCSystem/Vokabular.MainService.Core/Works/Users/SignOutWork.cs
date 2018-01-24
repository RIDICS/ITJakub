using System;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;

namespace Vokabular.MainService.Core.Works.Users
{
    public class SignOutWork : UnitOfWorkBase
    {
        private readonly UserRepository m_userRepository;
        private readonly int m_userId;

        public SignOutWork(UserRepository userRepository, int userId) : base(userRepository)
        {
            m_userRepository = userRepository;
            m_userId = userId;
        }

        protected override void ExecuteWorkImplementation()
        {
            var now = DateTime.UtcNow;
            var user = m_userRepository.FindById<User>(m_userId);

            if (user == null)
                return;

            user.CommunicationToken = Guid.NewGuid().ToString(); //TODO currently not null constraint exists
            user.CommunicationTokenCreateTime = now;

            m_userRepository.Update(user);
        }
    }
}