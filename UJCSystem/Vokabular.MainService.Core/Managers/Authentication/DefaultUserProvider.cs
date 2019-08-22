using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Ridics.Authentication.DataContracts;
using Ridics.Core.Structures.Shared;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.Shared.DataEntities.UnitOfWork;
using Vokabular.MainService.Core.Communication;
using CustomClaimTypes = Vokabular.Shared.Const.CustomClaimTypes;

namespace Vokabular.MainService.Core.Managers.Authentication
{
    public class DefaultUserProvider
    {
        private const string Unregistered = RoleNames.Unregistered;
        private const string RegisteredUser = RoleNames.RegisteredUser;

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
                ? m_userRepository.InvokeUnitOfWork(x => x.GetDefaultGroupOrCreate(Unregistered, GetUnregisteredRoleExternalId))
                : m_userRepository.GetDefaultGroupOrCreate(Unregistered, GetUnregisteredRoleExternalId);
        }

        private int GetUnregisteredRoleExternalId()
        {
            return GetDefaultUnregisteredRole().Id;
        }

        public IList<Claim> GetDefaultUserPermissions()
        {
            var permissions = GetDefaultUnregisteredRole().Permissions;
            return permissions.Select(perm => new Claim(CustomClaimTypes.Permission, perm.Name)).ToList();
        }

        public RoleContract GetDefaultUnregisteredRole()
        {
            var client = m_communicationProvider.GetAuthRoleApiClient();
            return client.GetRoleByName(Unregistered).GetAwaiter().GetResult();
        }

        public RoleContract GetDefaultRegisteredRole()
        {
            var client = m_communicationProvider.GetAuthRoleApiClient();
            return client.GetRoleByName(RegisteredUser).GetAwaiter().GetResult();
        }
    }
}