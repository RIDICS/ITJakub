using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Vokabular.Authentication.DataContracts;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;
using Vokabular.MainService.Core.Communication;
using Vokabular.Shared.Const;

namespace Vokabular.MainService.Core.Managers.Authentication
{
    public class DefaultUserProvider
    {
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
                ? m_userRepository.InvokeUnitOfWork(x => x.GetDefaultGroupOrCreate(Unregistered))
                : m_userRepository.GetDefaultGroupOrCreate(Unregistered);
        }

        public IList<Claim> GetDefaultUserPermissions()
        {
            //TODO get role by name
            var client = m_communicationProvider.GetAuthRoleApiClient();

            var permissions = client.GetAllRolesAsync().GetAwaiter().GetResult().First(role => role.Name == Unregistered).Permissions;
            return permissions.Select(perm => new Claim(CustomClaimTypes.Permission, perm.Name)).ToList();
        }

        public RoleContract GetDefaultUnregisteredRole()
        {
            //TODO get role by name
            var client = m_communicationProvider.GetAuthRoleApiClient();

            return client.GetAllRolesAsync().GetAwaiter().GetResult().First(role => role.Name == Unregistered);
        }
    }
}