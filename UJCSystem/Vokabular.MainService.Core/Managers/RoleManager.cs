using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using AutoMapper;
using log4net;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.Core.Communication;
using Vokabular.MainService.Core.Managers.Authentication;
using Vokabular.MainService.Core.Utils;
using Vokabular.MainService.Core.Works.Permission;
using Vokabular.MainService.Core.Works.Users;
using Vokabular.MainService.DataContracts;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Contracts.Permission;
using Vokabular.MainService.DataContracts.Contracts.Type;
using Vokabular.RestClient.Results;
using Vokabular.Shared.DataEntities.UnitOfWork;
using AuthRoleContract = Ridics.Authentication.DataContracts.RoleContract;
using AuthRoleContractBase = Ridics.Authentication.DataContracts.RoleContractBase;

namespace Vokabular.MainService.Core.Managers
{
    public class RoleManager
    {
        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly UserRepository m_userRepository;
        private readonly PermissionRepository m_permissionRepository;
        private readonly UserDetailManager m_userDetailManager;
        private readonly DefaultUserProvider m_defaultUserProvider;
        private readonly IMapper m_mapper;

        private readonly CommunicationProvider m_communicationProvider;

        public RoleManager(UserRepository userRepository, PermissionRepository permissionRepository,
            CommunicationProvider communicationProvider, UserDetailManager userDetailManager, DefaultUserProvider defaultUserProvider,
            IMapper mapper)
        {
            m_userRepository = userRepository;
            m_permissionRepository = permissionRepository;
            m_communicationProvider = communicationProvider;
            m_userDetailManager = userDetailManager;
            m_defaultUserProvider = defaultUserProvider;
            m_mapper = mapper;
        }

        public List<UserGroupContract> GetUserGroupsByUser(int userId)
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
            new GetOrCreateUserGroupsWork<AuthRoleContractBase>(m_userRepository, authUser.Roles, userId).Execute();

            var dbGroups = m_permissionRepository.InvokeUnitOfWork(x => x.GetUserGroupsByUser(userId));
            var resultList = m_mapper.Map<List<UserGroupContract>>(dbGroups);

            // Fill description for RoleUserGroups (local database doesn't contain this)
            foreach (var roleUserGroup in resultList.Where(x => x.Type == UserGroupTypeContract.Role))
            {
                roleUserGroup.Description = authUser.Roles.First(x => x.Id == roleUserGroup.ExternalId).Description;
            }

            return resultList;
        }


        public PagedResultList<UserContract> GetUsersByGroup(int groupId, int? start, int? count, string filterByName)
        {
            // Method required for Role management (select role, load users)

            var role = m_permissionRepository.InvokeUnitOfWork(x => x.FindById<UserGroup>(groupId));

            if (role is RoleUserGroup roleUserGroup)
            {
                // If UserGroup has relation to Roles on Auth service, use user list from Auth service (the main data source)

                var client = m_communicationProvider.GetAuthRoleApiClient();
                var result = client.GetUserListByRoleAsync(roleUserGroup.ExternalId, start, count, filterByName).GetAwaiter().GetResult();
                var users = m_mapper.Map<List<UserContract>>(result.Items);
                m_userDetailManager.AddIdForExternalUsers(users);

                return new PagedResultList<UserContract>
                {
                    List = users,
                    TotalCount = result.ItemsCount
                };
            }
            else
            {
                // If UserGroup has no relation to Auth service (permissions to books), use local user list

                var startValue = PagingHelper.GetStart(start);
                var countValue = PagingHelper.GetCount(count);
                var dbUsers = m_userRepository.GetUsersByGroup(groupId, startValue, countValue, filterByName);
                var users = m_mapper.Map<List<UserContract>>(dbUsers.List);

                return new PagedResultList<UserContract>
                {
                    List = users,
                    TotalCount = dbUsers.Count,
                };
            }
        }

        public int CreateRole(string roleName, string description)
        {
            return new CreateRoleWork(m_permissionRepository, m_communicationProvider, roleName, description).Execute();
        }

        public void UpdateRole(RoleContract data)
        {
            new UpdateRoleWork(m_permissionRepository, m_defaultUserProvider, m_communicationProvider, data).Execute();
        }

        public RoleDetailContract GetUserGroupDetail(int groupId)
        {
            var dbRole = m_permissionRepository.InvokeUnitOfWork(x => x.FindById<UserGroup>(groupId));

            var client = m_communicationProvider.GetAuthRoleApiClient();

            if (dbRole is RoleUserGroup roleUserGroup)
            {
                var role = client.GetRoleAsync(roleUserGroup.ExternalId, true).GetAwaiter().GetResult();
                if (role == null)
                    return null;

                var result = m_mapper.Map<RoleDetailContract>(role);
                result.Id = groupId;

                return result;
            }
            else
            {
                var result = m_mapper.Map<RoleDetailContract>(dbRole);
                return result;
            }
        }

        public void DeleteRole(int roleId)
        {
            new DeleteRoleWork(m_permissionRepository, m_defaultUserProvider, m_communicationProvider, roleId).Execute();
        }

        public void RemoveUserFromRole(int userId, int roleId)
        {
            //new SynchronizeRoleWork(m_permissionRepository, m_communicationProvider, roleId).Execute();
            new RemoveUserFromRoleWork(m_permissionRepository, m_communicationProvider, userId, roleId).Execute();
        }

        public void AddUserToRole(int userId, int roleId)
        {
            //new SynchronizeRoleWork(m_permissionRepository, m_communicationProvider, roleId).Execute();
            new AddUserToRoleWork(m_permissionRepository, m_defaultUserProvider, m_communicationProvider, userId, roleId).Execute();
        }

        public List<RoleContract> GetRoleAutocomplete(string query, int? count)
        {
            if (query == null)
                query = string.Empty;

            var countValue = PagingHelper.GetAutocompleteCount(count);

            var client = m_communicationProvider.GetAuthRoleApiClient();
            var authResultList = client.GetRoleListAsync(0, countValue, query).GetAwaiter().GetResult();
            
            var dbUserGroups = new GetOrCreateUserGroupsWork<AuthRoleContract>(m_userRepository, authResultList.Items).Execute();

            var resultList = new List<RoleContract>();
            foreach (var authRole in authResultList.Items)
            {
                var resultRole = m_mapper.Map<RoleContract>(authRole);
                resultRole.Id = dbUserGroups.First(x => x.ExternalId == resultRole.ExternalId).Id;
                resultList.Add(resultRole);
            }

            return resultList;
        }

        public IList<UserGroupContract> GetSingleUserGroupAutocomplete(string query, int? count, bool includeSearchInUsers)
        {
            if (query == null)
                query = string.Empty;

            var countValue = PagingHelper.GetAutocompleteCount(count);

            var dbResult = m_userRepository.InvokeUnitOfWork(x => x.FindSingleUserGroups(0, countValue, query, includeSearchInUsers));
            var result = m_mapper.Map<IList<UserGroupContract>>(dbResult);

            return result;
        }

        public PagedResultList<RoleContract> GetRoleList(int? start, int? count, string filterByName)
        {
            var startValue = PagingHelper.GetStart(start);
            var countValue = PagingHelper.GetCount(count);

            var client = m_communicationProvider.GetAuthRoleApiClient();

            var authResult = client.GetRoleListAsync(startValue, countValue, filterByName).GetAwaiter().GetResult();

            var resultList = new List<RoleContract>();
            foreach (var authRoleContract in authResult.Items)
            {
                var synchronizeRoleWork = new SynchronizeRoleWork(m_permissionRepository, m_communicationProvider, authRoleContract);
                synchronizeRoleWork.Execute();

                var resultRoleContract = m_mapper.Map<RoleContract>(authRoleContract);
                resultRoleContract.Id = synchronizeRoleWork.LocalId;
                
                resultList.Add(resultRoleContract);
            }

            return new PagedResultList<RoleContract>
            {
                List = resultList,
                TotalCount = authResult.ItemsCount,
            };
        }
    }
}