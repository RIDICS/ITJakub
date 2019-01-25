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
    public class UserGroupManager
    {
        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly UserRepository m_userRepository;
        private readonly PermissionRepository m_permissionRepository;
        private readonly AuthorizationManager m_authorizationManager;
        private readonly CommunicationProvider m_communicationProvider;

        public UserGroupManager(UserRepository userRepository, PermissionRepository permissionRepository, AuthorizationManager authorizationManager, CommunicationProvider communicationProvider)
        {
            m_userRepository = userRepository;
            m_permissionRepository = permissionRepository;
            m_authorizationManager = authorizationManager;
            m_communicationProvider = communicationProvider;
        }

        public List<UserGroupContract> GetGroupsByUser(int userId)
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

        public List<UserContract> GetUsersByGroup(int groupId)
        {
            using (var client = m_communicationProvider.GetAuthenticationServiceClient())
            {
                var members = client.GetUsersByRole(groupId);
                return Mapper.Map<List<UserContract>>(members);
            }
        }

        public int CreateGroup(string groupName, string description)
        {
            var userId = m_authorizationManager.GetCurrentUserId();
            return new CreateRoleWork(m_permissionRepository, m_communicationProvider, groupName, description, userId).Execute();
        }

        public UserGroupContract GetGroupDetail(int groupId)
        {
            using (var client = m_communicationProvider.GetAuthenticationServiceClient())
            {
                var role = client.GetRole(groupId);
                if (role == null)
                    return null;

                return Mapper.Map<UserGroupContract>(role);
            }
        }

        public void DeleteGroup(int groupId)
        {
            new DeleteRoleWork(m_permissionRepository, m_communicationProvider, groupId).Execute();
        }

        public void RemoveUserFromGroup(int userId, int groupId)
        {
            new RemoveUserFromRoleWork(m_permissionRepository, m_communicationProvider, userId, groupId).Execute();
        }

        public void AddUserToGroup(int userId, int groupId)
        {
            new AddUserToRoleWork(m_permissionRepository, m_communicationProvider, userId, groupId).Execute();
        }

        public List<UserGroupContract> GetUserGroupAutocomplete(string query, int? count)
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