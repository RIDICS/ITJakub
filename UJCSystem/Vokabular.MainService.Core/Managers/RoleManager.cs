using System;
using System.Collections.Generic;
using System.Reflection;
using AutoMapper;
using log4net;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;
using Vokabular.MainService.Core.Communication;
using Vokabular.MainService.Core.Utils;
using Vokabular.MainService.Core.Works.Permission;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Contracts.Permission;

namespace Vokabular.MainService.Core.Managers
{
    public class RoleManager
    {
        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly UserRepository m_userRepository;
        private readonly PermissionRepository m_permissionRepository;
        private readonly AuthorizationManager m_authorizationManager;
        private readonly CommunicationProvider m_communicationProvider;

        public RoleManager(UserRepository userRepository, PermissionRepository permissionRepository, AuthorizationManager authorizationManager, CommunicationProvider communicationProvider)
        {
            m_userRepository = userRepository;
            m_permissionRepository = permissionRepository;
            m_authorizationManager = authorizationManager;
            m_communicationProvider = communicationProvider;
        }

        public List<UserGroupContract> GetRolesByUser(int userId)
        {
            var user = m_userRepository.InvokeUnitOfWork(x => x.GetUserById(userId));

            if (user == null)
            {
                string message = $"Cannot locate user with id '{userId}'";
                if (m_log.IsErrorEnabled)
                    m_log.Error(message);
                throw new ArgumentException(message);
            }

            using (var client = m_communicationProvider.GetAuthenticationServiceClient())
            {
                var authUser = client.GetUser(user.ExternalId);
                return Mapper.Map<List<UserGroupContract>>(authUser.Roles);
            }
        }

        public List<UserContract> GetUsersByRole(int roleId)
        {
            using (var client = m_communicationProvider.GetAuthenticationServiceClient())
            {
                var members = client.GetUsersByRole(roleId);
                return Mapper.Map<List<UserContract>>(members);
            }
        }

        public int CreateRole(string roleName, string description)
        {
            var userId = m_authorizationManager.GetCurrentUserId();
            return new CreateRoleWork(m_permissionRepository, m_communicationProvider, roleName, description, userId).Execute();
        }

        public UserGroupContract GetRoleDetail(int roleId)
        {
            using (var client = m_communicationProvider.GetAuthenticationServiceClient())
            {
                var role = client.GetRole(roleId);
                if (role == null)
                    return null;

                return Mapper.Map<UserGroupContract>(role);
            }
        }

        public void DeleteRole(int roleId)
        {
            new DeleteRoleWork(m_permissionRepository, m_communicationProvider, roleId).Execute();
        }

        public void RemoveUserFromRole(int userId, int roleId)
        {
            new RemoveUserFromRoleWork(m_permissionRepository, m_communicationProvider, userId, roleId).Execute();
        }

        public void AddUserToRole(int userId, int roleId)
        {
            new AddUserToRoleWork(m_permissionRepository, m_communicationProvider, userId, roleId).Execute();
        }

        public List<UserGroupContract> GetRoleAutocomplete(string query, int? count)
        {
            if (query == null)
                query = string.Empty;

            var countValue = PagingHelper.GetAutocompleteCount(count);

            using (var client = m_communicationProvider.GetAuthenticationServiceClient())
            {
                var result = client.GetListRole(query, countValue);
                return Mapper.Map<List<UserGroupContract>>(result.Items);
            }
        }
    }
}