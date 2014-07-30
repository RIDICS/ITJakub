using System.Collections.Generic;
using ITJakub.MobileApps.DataContracts;

namespace ITJakub.MobileApps.Service
{
    public class MobileAppsService : IMobileAppsService
    {
        private readonly IMobileAppsService m_manager;

        public MobileAppsService()
        {
            m_manager = Container.Current.Resolve<IMobileAppsService>();
        }


        public void CreateInstitution(Institution institution)
        {
            m_manager.CreateInstitution(institution);
        }

        public InstitutionDetails GetInstitutionDetails(string institutionId)
        {
            return m_manager.GetInstitutionDetails(institutionId);
        }

        public void AddUserToInstitution(string enterCode, string userId)
        {
            m_manager.AddUserToInstitution(enterCode,userId);
        }

        public void CreateUser(string authenticationProvider, string authenticationProviderToken, User user)
        {
            m_manager.CreateUser(authenticationProvider, authenticationProviderToken, user);
        }

        public LoginUserResponse LoginUser(UserLogin userLogin)
        {
            return m_manager.LoginUser(userLogin);
        }

        public UserDetails GetUserDetails(string userId)
        {
            return m_manager.GetUserDetails(userId);
        }

        public IEnumerable<TaskDetails> GetTasksForApplication(string applicationId)
        {
            return m_manager.GetTasksForApplication(applicationId);
        }

        public void CreateTaskForApplication(string applicationId, string userId, Task task)
        {
            m_manager.CreateTaskForApplication(applicationId, userId, task);
        }

        public CreateGroupResponse CreateGroup(string userId, Group group)
        {
            return m_manager.CreateGroup(userId, group);
        }

        public void AssignTaskToGroup(string groupId, string taskId, string userId)
        {
            m_manager.AssignTaskToGroup(groupId, taskId, userId);
        }

        public void AddUserToGroup(string enterCode, string userId)
        {
            m_manager.AddUserToGroup(enterCode, userId);
        }

        public GroupDetails GetGroupDetails(string groupId)
        {
            return m_manager.GetGroupDetails(groupId);
        }

        public IEnumerable<SynchronizedObjectDetails> GetSynchronizedObjects(string groupId, string applicationId, string objectType, string since)
        {
            return m_manager.GetSynchronizedObjects(groupId, applicationId, objectType, since);
        }

        public void CreateSynchronizedObject(string groupId, string applicationId, string userId, SynchronizedObject synchronizedObject)
        {
            m_manager.CreateSynchronizedObject(groupId, applicationId, userId, synchronizedObject);
        }
    }
}