using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;

namespace Vokabular.MainService.Core.Managers
{
    public class UserManager
    {
        private readonly UserRepository m_userRepository;

        public UserManager(UserRepository userRepository)
        {
            m_userRepository = userRepository;
        }

        public User GetCurrentUser()
        {
            // TODO get correct current user

            m_userRepository.UnitOfWork.BeginTransaction();
            return m_userRepository.GetUserByUsername("test");
        }

        public int GetCurrentUserId()
        {
            return GetCurrentUser().Id;
        }
    }
}
