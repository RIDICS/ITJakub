using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;

namespace Vokabular.MainService.Core.Managers
{
    public class UserManager
    {
        private readonly UserRepository m_userRepository;

        public UserManager(IUnitOfWork unitOfWork, UserRepository userRepository)
        {
            m_userRepository = userRepository;
        }

        public User GetCurrentUser()
        {
            // TODO get correct current user
            return m_userRepository.GetUserByUsername("test");
        }
    }
}
