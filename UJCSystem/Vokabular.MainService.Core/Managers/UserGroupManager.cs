﻿using System;
using System.Collections.Generic;
using System.Reflection;
using AutoMapper;
using log4net;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;
using Vokabular.MainService.Core.Utils;
using Vokabular.MainService.Core.Works.Permission;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Contracts.Permission;

namespace Vokabular.MainService.Core.Managers
{
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