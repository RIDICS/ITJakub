using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.MainService.Core.Works.Users
{
    public class UpdateUserWork : UnitOfWorkBase
    {
        private readonly UserRepository m_userRepository;
        private readonly int m_userId;
        private readonly UpdateUserInfo m_userInfo;

        public UpdateUserWork(UserRepository userRepository, int userId, UpdateUserInfo userInfo) : base(userRepository)
        {
            m_userRepository = userRepository;
            m_userId = userId;
            m_userInfo = userInfo;
        }

        protected override void ExecuteWorkImplementation()
        {
            var user = m_userRepository.FindById<User>(m_userId);
            user.ExtUsername = m_userInfo.Username;
            user.ExtFirstName = m_userInfo.FirstName;
            user.ExtLastName = m_userInfo.LastName;
            m_userRepository.Update(user);
        }
    }
}