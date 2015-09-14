using System;
using System.Collections.Generic;
using System.Reflection;
using AutoMapper;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.DataEntities.Database.Repositories;
using ITJakub.Shared.Contracts;
using log4net;

namespace ITJakub.ITJakubService.Core
{
    public class PermissionManager
    {
        private readonly UserRepository m_userRepository;
        private readonly PermissionRepository m_permissionRepository;

        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public PermissionManager(UserRepository userRepository, PermissionRepository permissionRepository)
        {
            m_userRepository = userRepository;
            m_permissionRepository = permissionRepository;
        }

        public List<GroupContract> GetGroupsByUser(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                string message = "Username is empty, cannot get his groups";

                if (m_log.IsWarnEnabled)
                    m_log.Warn(message);
                throw new ArgumentException(message);
            }

            User user = m_userRepository.FindByUserName(userName);

            if (user == null)
            {
                string message = string.Format("Cannot locate user by username: '{0}'", userName);
                if (m_log.IsErrorEnabled)
                    m_log.Error(message);
                throw new ArgumentException(message);
            }

            var groups = m_permissionRepository.GetGroupsByUser(user.Id);
            return Mapper.Map<List<GroupContract>>(groups);
        }

        public List<UserContract> GetUsersByGroup(int groupId)
        {
            var users = m_permissionRepository.GetUsersByGroup(groupId);
            return Mapper.Map<List<UserContract>>(users);
        }

        public GroupContract CreateGroup(int founderUserId, string groupName, string description)
        {
            var user = m_userRepository.FindById(founderUserId);

            var group = new Group
            {
                Name = groupName,
                Description = description,
                CreateTime = DateTime.UtcNow,
                CreatedBy = user
            };

            var groupId = m_permissionRepository.CreateGroup(group);
            return GetGroupDetail(groupId);
        }

        public GroupContract GetGroupDetail(int groupId)
        {
            var group = m_permissionRepository.FindGroupById(groupId);
            if (group == null) return null;
            var groupContract = Mapper.Map<GroupContract>(group);
            return groupContract;
        }

        public void DeleteGroup(int groupId)
        {
            var group = m_permissionRepository.FindGroupById(groupId);
            m_permissionRepository.Delete(group);
        }

        public void RemoveUserFromGroup(int userId, int groupId)
        {
            var group = m_permissionRepository.FindGroupById(groupId);
            var user = m_userRepository.FindById(userId);

            if (group.Users == null)
            {
                if (m_log.IsWarnEnabled)
                {
                    string message = string.Format("Cannot remove user with id '{0}' from group with id '{1}'. Group is empty.", user.Id, group.Id);
                    m_log.Warn(message);
                }
                return;
            }

            group.Users.Remove(user);
            m_permissionRepository.Save(group);
        }

        public void AddUserToGroup(int userId, int groupId)
        {
            var group = m_permissionRepository.FindGroupById(groupId);
            var user = m_userRepository.FindById(userId);

            if (group.Users == null)
            {
                group.Users = new List<User>();
            }

            group.Users.Add(user);
            m_permissionRepository.Save(group);
        }
    }
}