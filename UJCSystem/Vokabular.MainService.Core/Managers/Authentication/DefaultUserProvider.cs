using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using NHibernate.Util;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;
using Vokabular.MainService.Core.Communication;
using Vokabular.Shared.Const;

namespace Vokabular.MainService.Core.Managers.Authentication
{
    public class DefaultUserProvider
    {
        private const string RegisteredUsersGroupName = "RegisteredUsersGroup";
        private const string UnregisteredUsersGroupName = "UnregisteredUsersGroup";
        private const string Unregistered = "Unregistered";

        private readonly UserRepository m_userRepository;
        private readonly CommunicationProvider m_communicationProvider;

        public DefaultUserProvider(UserRepository userRepository, CommunicationProvider communicationProvider)
        {
            m_userRepository = userRepository;
            m_communicationProvider = communicationProvider;
        }

        public User GetDefaultUser()
        {
            return m_userRepository.UnitOfWork.CurrentSession == null
                ? m_userRepository.InvokeUnitOfWork(x => x.GetVirtualUserForUnregisteredUsersOrCreate(GetDefaultUnregisteredUserGroup()))
                : m_userRepository.GetVirtualUserForUnregisteredUsersOrCreate(GetDefaultUnregisteredUserGroup());
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

        public IList<Claim> GetDefaultUserPermissions()
        {
            //TODO get role by name
            using (var client = m_communicationProvider.GetAuthenticationServiceClient())
            {
                var permissions = client.GetAllRoles().First(role => role.Name == Unregistered).Permissions;
                return permissions.Select(perm => new Claim(CustomClaimTypes.Permission, perm.Name)).ToList();
            }
        }
    }
}