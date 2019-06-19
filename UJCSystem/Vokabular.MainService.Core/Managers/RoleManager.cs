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
using Vokabular.RestClient.Results;
using AuthRoleContract = Vokabular.Authentication.DataContracts.RoleContract;

namespace Vokabular.MainService.Core.Managers
{
    public class RoleManager
    {
        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly UserRepository m_userRepository;
        private readonly PermissionRepository m_permissionRepository;

        private readonly CommunicationProvider m_communicationProvider;

        public RoleManager(UserRepository userRepository, PermissionRepository permissionRepository, CommunicationProvider communicationProvider)
        {
            m_userRepository = userRepository;
            m_permissionRepository = permissionRepository;
            m_communicationProvider = communicationProvider;
        }

        public List<RoleContract> GetRolesByUser(int userId)
        {
            var user = m_userRepository.InvokeUnitOfWork(x => x.GetUserById(userId));

            if (user == null)
            {
                string message = $"Cannot locate user with id '{userId}'";
                if (m_log.IsErrorEnabled)
                    m_log.Error(message);
                throw new ArgumentException(message);
            }

            if (user.ExternalId == null)
            {
                throw new ArgumentException($"User with ID {userId} has missing ExternalID");
            }

            var client = m_communicationProvider.GetAuthUserApiClient();

            var authUser = client.GetUserForRoleAssignmentAsync(user.ExternalId.Value).GetAwaiter().GetResult();
            return Mapper.Map<List<RoleContract>>(authUser.Roles);
        }

        public List<UserContract> GetUsersByRole(int roleId)
        {
            var client = m_communicationProvider.GetAuthUserApiClient();

            var members = client.GetUserListByRoleAsync(roleId, null, null).GetAwaiter().GetResult();
            return Mapper.Map<List<UserContract>>(members);
        }

        public int CreateRole(string roleName, string description)
        {
            return new CreateRoleWork(m_permissionRepository, m_communicationProvider, roleName, description).Execute();
        }

        public RoleContract GetRoleDetail(int roleId)
        {
            var client = m_communicationProvider.GetAuthRoleApiClient();

            var role = client.HttpClient.GetItemAsync<AuthRoleContract>(roleId).GetAwaiter().GetResult();
            if (role == null)
                return null;

            return Mapper.Map<RoleContract>(role);
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

        public List<RoleContract> GetRoleAutocomplete(string query, int? count)
        {
            if (query == null)
                query = string.Empty;

            var countValue = PagingHelper.GetAutocompleteCount(count);

            var client = m_communicationProvider.GetAuthRoleApiClient();

            var result = client.HttpClient.GetListAsync<AuthRoleContract>(0, countValue, query).GetAwaiter().GetResult();
            return Mapper.Map<List<RoleContract>>(result.Items);
        }

        public PagedResultList<RoleContract> GetRoleList(int? start, int? count, string filterByName)
        {
            var startValue = PagingHelper.GetStart(start);
            var countValue = PagingHelper.GetCount(count);

            var client = m_communicationProvider.GetAuthRoleApiClient();

            var result = client.HttpClient.GetListAsync<AuthRoleContract>(startValue, countValue, filterByName).GetAwaiter().GetResult();
            var roleContracts = Mapper.Map<List<RoleContract>>(result.Items);

            return new PagedResultList<RoleContract>
            {
                List = roleContracts,
                TotalCount = result.ItemsCount,
            };
        }
    }
}