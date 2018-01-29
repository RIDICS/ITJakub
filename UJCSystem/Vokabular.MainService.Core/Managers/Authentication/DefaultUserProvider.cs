using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;

namespace Vokabular.MainService.Core.Managers.Authentication
{
    public class DefaultUserProvider
    {
        private const string RegisteredUsersGroupName = "RegisteredUsersGroup";
        private const string UnregisteredUsersGroupName = "UnregisteredUsersGroup";
        private const string UnregisteredUserName = "UnregisteredUser";

        private readonly UserRepository m_userRepository;
        
        public DefaultUserProvider(UserRepository userRepository)
        {
            m_userRepository = userRepository;
        }

        public User GetDefaultUser()
        {
            return m_userRepository.UnitOfWork.CurrentSession == null
                ? m_userRepository.InvokeUnitOfWork(x => x.GetVirtualUserForUnregisteredUsersOrCreate(UnregisteredUserName, GetDefaultUnregisteredUserGroup()))
                : m_userRepository.GetVirtualUserForUnregisteredUsersOrCreate(UnregisteredUserName, GetDefaultUnregisteredUserGroup());
        }

        public string GetDefaultUserName()
        {
            return UnregisteredUserName;
        }

        public UserGroup GetDefaultUnregisteredUserGroup()
        {
            return m_userRepository.UnitOfWork.CurrentSession == null
                ? m_userRepository.InvokeUnitOfWork(x => x.GetDefaultGroupOrCreate(UnregisteredUsersGroupName))
                : m_userRepository.GetDefaultGroupOrCreate(UnregisteredUsersGroupName);
        }
        
        public UserGroup GetDefaultRegisteredUserGroup()
        {
            return m_userRepository.UnitOfWork.CurrentSession == null
                ? m_userRepository.InvokeUnitOfWork(x => x.GetDefaultGroupOrCreate(RegisteredUsersGroupName))
                : m_userRepository.GetDefaultGroupOrCreate(RegisteredUsersGroupName);
        }
    }
}