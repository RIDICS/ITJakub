using System;
using System.Collections.Generic;
using System.Reflection;
using AutoMapper;
using log4net;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;
using Vokabular.MainService.Core.Utils;
using Vokabular.MainService.Core.Works.Permission;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Contracts.Permission;
using Vokabular.Shared.DataContracts.Types;

namespace Vokabular.MainService.Core.Managers
{
    public class PermissionManager
    {
        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly AuthenticationManager m_authenticationManager;
        private readonly AuthorizationManager m_authorizationManager;
        private readonly PermissionRepository m_permissionRepository;

        public PermissionManager(AuthenticationManager authenticationManager, AuthorizationManager authorizationManager, PermissionRepository permissionRepository)
        {
            m_authenticationManager = authenticationManager;
            m_authorizationManager = authorizationManager;
            m_permissionRepository = permissionRepository;
        }
        
        public List<SpecialPermissionContract> GetSpecialPermissionsForUser(SpecialPermissionCategorizationEnumContract? filterByType)
        {
            var userId = m_authenticationManager.GetCurrentUserId();
            var categorizationType = Mapper.Map<SpecialPermissionCategorization?>(filterByType);

            var specPermissions = m_permissionRepository.InvokeUnitOfWork(x =>
                categorizationType == null
                    ? x.GetSpecialPermissionsByUser(userId)
                    : x.GetSpecialPermissionsByUserAndType(userId, categorizationType.Value));
            return Mapper.Map<List<SpecialPermissionContract>>(specPermissions);
        }

        public List<SpecialPermissionContract> GetSpecialPermissions()
        {
            m_authorizationManager.CheckUserCanManagePermissions();
            var specPermissions = m_permissionRepository.InvokeUnitOfWork(x => x.GetSpecialPermissions());
            return Mapper.Map<List<SpecialPermissionContract>>(specPermissions);
        }

        public List<SpecialPermissionContract> GetSpecialPermissionsForGroup(int groupId)
        {
            m_authorizationManager.CheckUserCanManagePermissions();
            var specPermissions = m_permissionRepository.InvokeUnitOfWork(x => x.GetSpecialPermissionsByGroup(groupId));
            return Mapper.Map<List<SpecialPermissionContract>>(specPermissions);
        }

        public void AddSpecialPermissionsToGroup(int groupId, IList<int> specialPermissionsIds)
        {
            m_authorizationManager.CheckUserCanManagePermissions();

            if (specialPermissionsIds == null || specialPermissionsIds.Count == 0)
            {
                return;
            }

            new AddSpecialPermissionsToGroupWork(m_permissionRepository, groupId, specialPermissionsIds).Execute();
        }

        public void RemoveSpecialPermissionsFromGroup(int groupId, IList<int> specialPermissionsIds)
        {
            m_authorizationManager.CheckUserCanManagePermissions();

            if (specialPermissionsIds == null || specialPermissionsIds.Count == 0)
            {
                return;
            }

            new RemoveSpecialPermissionsFromGroupWork(m_permissionRepository, groupId, specialPermissionsIds).Execute();
        }

        public void AddBooksAndCategoriesToGroup(int groupId, IList<long> bookIds)
        {
            m_authorizationManager.CheckUserCanManagePermissions();
            new AddBooksAndCategoriesToGroupWork(m_permissionRepository, groupId, bookIds).Execute();
        }

        public void RemoveBooksAndCategoriesFromGroup(int groupId, IList<long> bookIds)
        {
            m_authorizationManager.CheckUserCanManagePermissions();
            new RemoveBooksAndCategoriesFromGroupWork(m_permissionRepository, groupId, bookIds).Execute();
        }
    }

    public class UserGroupManager
    {
        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly PermissionRepository m_permissionRepository;
        private readonly AuthorizationManager m_authorizationManager;

        public UserGroupManager(PermissionRepository permissionRepository, AuthorizationManager authorizationManager)
        {
            m_permissionRepository = permissionRepository;
            m_authorizationManager = authorizationManager;
        }

        public List<UserGroupContract> GetGroupsByUser(int userId)
        {
            var user = m_permissionRepository.InvokeUnitOfWork(x => x.GetUserWithGroups(userId));

            if (user == null)
            {
                string message = $"Cannot locate user with id '{userId}'";
                if (m_log.IsErrorEnabled)
                    m_log.Error(message);
                throw new ArgumentException(message);
            }

            var groups = user.Groups;
            return Mapper.Map<List<UserGroupContract>>(groups);
        }

        public List<UserContract> GetUsersByGroup(int groupId)
        {
            var users = m_permissionRepository.InvokeUnitOfWork(x => x.GetUsersByGroup(groupId));
            return Mapper.Map<List<UserContract>>(users);
        }

        public int CreateGroup(string groupName, string description)
        {
            var permissionResult = m_authorizationManager.CheckUserCanManagePermissions();
            var userId = permissionResult.UserId;
            var result = new CreateGroupWork(m_permissionRepository, groupName, description, userId).Execute();
            return result;
        }

        public UserGroupDetailContract GetGroupDetail(int groupId)
        {
            var group = m_permissionRepository.InvokeUnitOfWork(x => x.FindGroupById(groupId));
            if (group == null) return null;
            var groupContract = Mapper.Map<UserGroupDetailContract>(group);
            return groupContract;
        }

        public void DeleteGroup(int groupId)
        {
            m_authorizationManager.CheckUserCanManagePermissions();
            new DeleteGroupWork(m_permissionRepository, groupId).Execute();
        }

        public void RemoveUserFromGroup(int userId, int groupId)
        {
            m_authorizationManager.CheckUserCanManagePermissions();
            new RemoveUserFromGroupWork(m_permissionRepository, userId, groupId).Execute();
        }

        public void AddUserToGroup(int userId, int groupId)
        {
            m_authorizationManager.CheckUserCanManagePermissions();
            new AddUserToGroupWork(m_permissionRepository, userId, groupId).Execute();
        }

        public List<UserGroupContract> GetUserGroupAutocomplete(string query, int? count)
        {
            m_authorizationManager.CheckUserCanManagePermissions();

            if (query == null)
                query = string.Empty;

            var countValue = PagingHelper.GetAutocompleteCount(count);

            var result = m_permissionRepository.InvokeUnitOfWork(x => x.GetGroupsAutocomplete(query, countValue));
            return Mapper.Map<List<UserGroupContract>>(result);
        }
    }
}
