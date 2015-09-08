using System;
using System.Collections.Generic;
using Castle.MicroKernel;
using ITJakub.MobileApps.Core.Applications;
using ITJakub.MobileApps.Core.Groups;
using ITJakub.MobileApps.Core.Tasks;
using ITJakub.MobileApps.Core.Users;
using ITJakub.MobileApps.DataContracts;
using ITJakub.MobileApps.DataContracts.Applications;
using ITJakub.MobileApps.DataContracts.Groups;
using ITJakub.MobileApps.DataContracts.Tasks;

namespace ITJakub.MobileApps.Core
{
    public class MobileServiceManager : IMobileAppsService
    {
        private readonly UserManager m_userManager;
        private readonly GroupManager m_groupManager;
        private readonly ApplicationManager m_applicationManager;
        private readonly TaskManager m_taskManager;

        public MobileServiceManager(IKernel container)
        {
            m_userManager = container.Resolve<UserManager>();
            m_groupManager = container.Resolve<GroupManager>();
            m_applicationManager = container.Resolve<ApplicationManager>();
            m_taskManager = container.Resolve<TaskManager>();
        }

        public void CreateUser(AuthProvidersContract providerContract, string providerToken, UserDetailContract userDetail)
        {
            m_userManager.CreateUser(providerContract, providerToken, userDetail);
        }

        public LoginUserResponse LoginUser(AuthProvidersContract providerContract, string providerToken, string email)
        {
            return m_userManager.LoginUser(providerContract, providerToken, email);
        }

        public List<GroupInfoContract> GetMembershipGroups(long userId)
        {
            return m_groupManager.GetMembershipGroups(userId);
        }

        public List<OwnedGroupInfoContract> GetOwnedGroups(long userId)
        {
            return m_groupManager.GetOwnedGroups(userId);
        }

        public UserGroupsContract GetGroupsByUser(long userId)
        {
            return m_groupManager.GetGroupByUser(userId);
        }

        public CreateGroupResponse CreateGroup(long userId, string groupName)
        {
            return m_groupManager.CreateGroup(userId, groupName);
        }

        public void AddUserToGroup(string groupAccessCode, long userId)
        {
            m_groupManager.AddUserToGroup(groupAccessCode, userId);
        }

        public void CreateSynchronizedObject(int applicationId, long groupId, long userId,
            SynchronizedObjectContract synchronizedObject)
        {
            m_applicationManager.CreateSynchronizedObject(applicationId, groupId, userId, synchronizedObject);
        }

        public IList<SynchronizedObjectResponseContract> GetSynchronizedObjects(long groupId, int applicationId, string objectType, DateTime since)
        {
            return m_applicationManager.GetSynchronizedObjects(groupId, applicationId, objectType, since);
        }

        public SynchronizedObjectResponseContract GetLatestSynchronizedObject(long groupId, int applicationId, string objectType,
            DateTime since)
        {
            return m_applicationManager.GetLatestSynchronizedObject(groupId, applicationId, objectType, since);
        }

        public IList<ApplicationContract> GetAllApplication()
        {
            return m_applicationManager.GetAllApplication();
        }

        public GroupDetailContract GetGroupDetails(long groupId)
        {
            return m_groupManager.GetGroupDetails(groupId);
        }

        public IList<GroupDetailsUpdateContract> GetGroupsUpdate(IList<OldGroupDetailsContract> groups)
        {
            return m_groupManager.GetGroupsUpdate(groups);
        }

        public void AssignTaskToGroup(long groupId, long taskId)
        {
            m_groupManager.AssignTaskToGroup(groupId, taskId);
        }

        public IList<TaskDetailContract> GetTasksByApplication(int applicationId)
        {
            return m_taskManager.GetTasksByApplication(applicationId);
        }

        public IList<TaskDetailContract> GetTasksByAuthor(long userId)
        {
            return m_taskManager.GetTasksByAuthor(userId);
        }

        public void CreateTask(long userId, int applicationId, string name, string data, string description)
        {
            m_taskManager.CreateTask(userId, applicationId, name, data, description);
        }

        public TaskDataContract GetTask(long taskId)
        {
            return m_taskManager.GetTask(taskId);
        }

        public TaskDataContract GetTaskForGroup(long groupId)
        {
            return m_taskManager.GetTaskForGroup(groupId);
        }

        public GroupStateContract GetGroupState(long groupId)
        {
            return m_groupManager.GetGroupState(groupId);
        }

        public void UpdateGroupState(long groupId, GroupStateContract state)
        {
            m_groupManager.UpdateGroupState(groupId, state);
        }

        public void RemoveGroup(long groupId)
        {
            m_groupManager.RemoveGroup(groupId);
        }
    }
}