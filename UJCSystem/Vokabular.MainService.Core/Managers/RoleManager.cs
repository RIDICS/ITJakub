using System.Collections.Generic;
using System.Net;
using System.Reflection;
using AutoMapper;
using log4net;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.Core.Communication;
using Vokabular.MainService.Core.Managers.Authentication;
using Vokabular.MainService.Core.Utils;
using Vokabular.MainService.Core.Works.Permission;
using Vokabular.MainService.DataContracts;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Contracts.Permission;
using Vokabular.RestClient.Results;
using Vokabular.Shared.DataEntities.UnitOfWork;
using AuthRoleContract = Ridics.Authentication.DataContracts.RoleContract;

namespace Vokabular.MainService.Core.Managers
{
    public class RoleManager
    {
        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly UserRepository m_userRepository;
        private readonly PermissionRepository m_permissionRepository;
        private readonly UserDetailManager m_userDetailManager;
        private readonly DefaultUserProvider m_defaultUserProvider;

        private readonly CommunicationProvider m_communicationProvider;

        public RoleManager(UserRepository userRepository, PermissionRepository permissionRepository,
            CommunicationProvider communicationProvider, UserDetailManager userDetailManager, DefaultUserProvider defaultUserProvider)
        {
            m_userRepository = userRepository;
            m_permissionRepository = permissionRepository;
            m_communicationProvider = communicationProvider;
            m_userDetailManager = userDetailManager;
            m_defaultUserProvider = defaultUserProvider;
        }

        public List<RoleContract> GetRolesByUser(int userId)
        {
            var user = m_userRepository.InvokeUnitOfWork(x => x.GetUserById(userId));

            if (user == null)
            {
                var message = $"Cannot locate user with id '{userId}'";
                if (m_log.IsErrorEnabled)
                    m_log.Error(message);

                throw new MainServiceException(MainServiceErrorCode.CannotLocateUser,
                    message,
                    HttpStatusCode.BadRequest,
                    new object[] {userId}
                );
            }

            if (user.ExternalId == null)
            {
                throw new MainServiceException(MainServiceErrorCode.UserHasMissingExternalId,
                    $"User with ID {user.Id} has missing ExternalID",
                    HttpStatusCode.BadRequest,
                    new object[] {user.Id}
                );
            }

            var client = m_communicationProvider.GetAuthUserApiClient();

            var authUser = client.GetUserForRoleAssignmentAsync(user.ExternalId.Value).GetAwaiter().GetResult();
            return Mapper.Map<List<RoleContract>>(authUser.Roles);
        }


        public PagedResultList<UserContract> GetUsersByRole(int roleId, int? start, int? count, string filterByName)
        {
            var client = m_communicationProvider.GetAuthRoleApiClient();
            var result = client.GetUserListByRoleAsync(roleId, start, count, filterByName).GetAwaiter().GetResult();
            var users = Mapper.Map<List<UserContract>>(result.Items);
            m_userDetailManager.AddIdForExternalUsers(users);

            return new PagedResultList<UserContract>
            {
                List = users,
                TotalCount = result.ItemsCount
            };
        }

        public int CreateRole(string roleName, string description)
        {
            return new CreateRoleWork(m_permissionRepository, m_communicationProvider, roleName, description).Execute();
        }

        public void UpdateRole(RoleContract data)
        {
            new UpdateRoleWork(m_permissionRepository, m_defaultUserProvider, m_communicationProvider, data).Execute();
        }

        public RoleDetailContract GetRoleDetail(int roleId)
        {
            var client = m_communicationProvider.GetAuthRoleApiClient();

            var role = client.HttpClient.GetItemAsync<AuthRoleContract>(roleId).GetAwaiter().GetResult();
            if (role == null)
                return null;

            return Mapper.Map<RoleDetailContract>(role);
        }

        public void DeleteRole(int roleId)
        {
            new DeleteRoleWork(m_permissionRepository, m_defaultUserProvider, m_communicationProvider, roleId).Execute();
        }

        public void RemoveUserFromRole(int userId, int roleId)
        {
            new SynchronizeRoleWork(m_permissionRepository, m_communicationProvider, roleId).Execute();
            new RemoveUserFromRoleWork(m_permissionRepository, m_communicationProvider, userId, roleId).Execute();
        }

        public void AddUserToRole(int userId, int roleId)
        {
            new SynchronizeRoleWork(m_permissionRepository, m_communicationProvider, roleId).Execute();
            new AddUserToRoleWork(m_permissionRepository, m_defaultUserProvider, m_communicationProvider, userId, roleId).Execute();
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

            foreach (var roleContract in result.Items)
            {
                new SynchronizeRoleWork(m_permissionRepository, m_communicationProvider, roleContract).Execute();
            }

            var roleContracts = Mapper.Map<List<RoleContract>>(result.Items);

            return new PagedResultList<RoleContract>
            {
                List = roleContracts,
                TotalCount = result.ItemsCount,
            };
        }
    }
}